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
            AddComponentType<Physics>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Position = (Position)Components[0];
            var Mover = (Movement)Components[1];
            var Collider = (Collision)Components[2];
            var Physics = (Physics)Components[3];

            if (Collider.Shape() != Collision.ShapeType.Rect)
            {
                return;
            }

            List<Collision> AllCollisions = World.GetComponents<Collision>();

            Physics.OnGround = _Check(Mask.Solid, Position, Collider, AllCollisions, new Point(0, 1));
            _CalculateVelocityX(DeltaTime, Mover, Physics);
            _CalculateVelocityY(DeltaTime, Mover, Physics);
        }

        private void _CalculateVelocityX(float DeltaTime, Movement Mover, Physics Physics)
        {
            float MaxSpeed;
            float Acceleration;
            float Friction;

            if(Physics.OnGround)
            {
                MaxSpeed = Physics.MaxGroundSpeed;
                Acceleration = Physics.GroundAccel;
                Friction = Physics.GroundFriction;
            }
            else
            {
                MaxSpeed = Physics.MaxAirSpeed;
                Acceleration = Physics.AirAccel;
                Friction = Physics.AirFriction;
            }

            float DesiredVel = 0;
            if (Mover.Velocity.X != 0 && Physics.Direction != Math.Sign(Mover.Velocity.X))
            {
                DesiredVel = Math.Sign(Mover.Velocity.X) * Math.Max((Math.Abs(Mover.Velocity.X) - Friction * DeltaTime), 0);
            }
            else
            {
                DesiredVel = Mover.Velocity.X + Physics.Direction * Acceleration * DeltaTime;
            }

            if (Math.Abs(DesiredVel) > MaxSpeed)
            {
                // TODO: Change to approaching instead of instant
                DesiredVel = Math.Sign(Mover.Velocity.X) * MaxSpeed;
            }

            Mover.Velocity.X = DesiredVel;
        }

        private void _CalculateVelocityY(float DeltaTime, Movement Mover, Physics Physics)
        {
            if (Physics.OnGround)
            {
                Mover.Velocity.Y = Physics.Jumping ? -1 * Physics.JumpVelocity : 0;
                Physics.JumpTimer = Physics.Jumping ? 0.2F : 0;
            }
            else
            {
                Mover.Velocity.Y = Math.Min(Mover.Velocity.Y + Physics.Gravity * DeltaTime, Physics.MaxFallingSpeed);
            }

            if (Physics.JumpTimer > 0)
            {
                Mover.Velocity.Y = -1 * Physics.JumpVelocity;
                Physics.JumpTimer -= DeltaTime;
            }
        }

        // TODO: Clean up places with collision check.
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
                    OtherOffset = OtherPos.Current;
                }
                Rectangle R1 = Collider.Rectangle();
                R1.Offset(Position.Current + Offset);
                Rectangle R2 = Other.Rectangle();
                R2.Offset(OtherOffset);
                return R1.Intersects(R2);
            }
            else
            {
                Rectangle Rect = Collider.Rectangle();
                Rect.Offset(Position.Current + Offset);
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
