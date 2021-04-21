using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class PhysicsSystem : BaseSystem
    {
        public PhysicsSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Position>();
            AddComponentType<Movement>();
            AddComponentType<Collision>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Position = (Position)Components[0];
            var Mover = (Movement)Components[1];
            var Collider = (Collision)Components[2];

            if(Collider.Shape() != Collision.ShapeType.Rect)
            {
                return;
            }

            List<Collision> AllCollisions = World.GetComponents<Collision>();

            Mover.OnGround = _Check(Mask.Solid, Position, Collider, AllCollisions, new Point(0, 1));
            _CalculateVelocityX(DeltaTime, Mover);
            _CalculateVelocityY(DeltaTime, Mover);
            _PrepareMovement(DeltaTime, Position, Mover);
            _MoveX(Position, Mover, Collider, AllCollisions);
            _MoveY(Position, Mover, Collider, AllCollisions);
        }

        private void _CalculateVelocityX(float DeltaTime, Movement Mover)
        {
            float MaxSpeed;
            float Acceleration;
            float Friction;
            if (Mover.OnGround)
            {
                MaxSpeed = Mover.MaxGroundSpeed;
                Acceleration = Mover.GroundAccel;
                Friction = Mover.GroundFriction;
            }
            else
            {
                MaxSpeed = Mover.MaxAirSpeed;
                Acceleration = Mover.AirAccel;
                Friction = Mover.AirFriction;
            }

            Mover.Velocity.X = Mover.Velocity.X + Mover.Direction * Acceleration * DeltaTime;
            if (Math.Abs(Mover.Velocity.X) > MaxSpeed)
            {
                // TODO: Change to approaching instead of instant
                Mover.Velocity.X = Math.Sign(Mover.Velocity.X) * MaxSpeed;
            }

            if(Mover.Direction == 0)
            {
                Mover.Velocity.X = Math.Sign(Mover.Velocity.X) * Math.Max((Math.Abs(Mover.Velocity.X) - Friction * DeltaTime), 0);
            }
        }

        private void _CalculateVelocityY(float DeltaTime, Movement Mover)
        {
            if (Mover.OnGround && Mover.Jumping)
            {
                Mover.Jumping = false;
                Mover.JumpTimer = 0.3F;
            }

            if(Mover.JumpTimer > 0)
            {
                Mover.Velocity.Y = -100;
                Mover.JumpTimer -= DeltaTime;
            }

            if(!Mover.OnGround)
            {
                Mover.Velocity.Y = Math.Min(Mover.Velocity.Y + Mover.Gravity * DeltaTime, Mover.MaxFallingSpeed);
            }
        }

        private void _PrepareMovement(float DeltaTime, Position Pos, Movement Mover)
        {
            Vector2 FullMove = Pos.Remainder + Mover.Velocity * DeltaTime;
            Mover.DesiredMovement.X = (int)FullMove.X;
            Mover.DesiredMovement.Y = (int)FullMove.Y;
            Pos.Remainder.X = FullMove.X - Mover.DesiredMovement.X;
            Pos.Remainder.Y = FullMove.Y - Mover.DesiredMovement.Y;
        }

        private void _MoveX(Position Position, Movement Mover, Collision Collider, List<Collision> AllCollisions)
        {
            int Distance = Mover.DesiredMovement.X;
            if(Distance == 0)
            {
                return;
            }

            int Sign = Math.Sign(Distance);
            Point Offset = new Point(Sign, 0);
            while (Distance != 0)
            {
                if(_Check(Mask.Solid, Position, Collider, AllCollisions, Offset))
                {
                    _StopX(Position, Mover);
                    return;
                }

                Position.Pos.X += Sign;
                Distance -= Sign;
            }
        }

        private void _MoveY(Position Position, Movement Mover, Collision Collider, List<Collision> AllCollisions)
        {
            int Distance = Mover.DesiredMovement.Y;
            if (Distance == 0)
            {
                return;
            }

            int Sign = Math.Sign(Distance);
            Point Offset = new Point(0, Sign);
            while (Distance != 0)
            {
                if (_Check(Mask.Solid, Position, Collider, AllCollisions, Offset))
                {
                    _StopY(Position, Mover);
                    return;
                }

                Position.Pos.Y += Sign;
                Distance -= Sign;
            }
        }

        private void _StopX(Position Position, Movement Mover)
        {
            Mover.Velocity.X = 0; ;
            Position.Remainder.X = 0;
        }

        private void _StopY(Position Position, Movement Mover)
        {
            Mover.Velocity.Y = 0; ;
            Position.Remainder.Y = 0;
        }

        private bool _Check(int Mask, Position Position, Collision Collider, List<Collision> AllCollisions, Point Offset)
        {
            foreach (var Other in AllCollisions)
            {
                if (Other == Collider || Other.Mask() != Mask)
                {
                    continue;
                }

                if (_CheckCollision(Position, Collider, Other, Offset))
                {
                    return true;
                }
            }
            return false;
        }

        private bool _CheckCollision(Position Position, Collision Collider, Collision Other, Point Offset)
        {
            if (Other.Shape() == Collision.ShapeType.Rect)
            {
                Position OtherPos = World.Manager.GetComponent<Position>(Other.Entity);
                Point OtherOffset = new Point(0, 0);
                if (OtherPos != null)
                {
                    OtherOffset = OtherPos.Pos;
                }
                Rectangle R1 = Collider.Rectangle();
                R1.Offset(Position.Pos + Offset);
                Rectangle R2 = Other.Rectangle();
                R2.Offset(OtherOffset);
                return R1.Intersects(R2);
            }
            else
            {
                Rectangle Rect = Collider.Rectangle();
                Rect.Offset(Position.Pos + Offset);
                int Left = Math.Max(Rect.Left / Other.Grid().TileSize, 0);
                int Right = (int)Math.Min(Math.Ceiling((double)Rect.Right / Other.Grid().TileSize), Other.Grid().Columns);
                int Top = Math.Max(Rect.Top / Other.Grid().TileSize, 0);
                int Bottom = (int)Math.Min(Math.Ceiling((double)Rect.Bottom / Other.Grid().TileSize), Other.Grid().Rows);
                for (int x = Left; x < Right; x++)
                {
                    for (int y = Top; y < Bottom; y++)
                    {
                        if (Other.Grid().Cells[x + y * Other.Grid().Columns])
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
