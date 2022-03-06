using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class CollisionSystem : BaseSystem
    {
        public CollisionSystem(World InWorld) : base(SystemType.Gameplay)
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

            if (Collider.Shape() != Collision.ShapeType.Rect)
            {
                return;
            }

            Collider.CollisionClear();

            // Movement.
            if (Position.Current != Position.Last)
            {
                List<Collision> AllCollisions = World.GetComponents<Collision>();
                _CheckMovement(Position, Collider, AllCollisions);
                _AdjustMovement(Position, Mover, Collider);
                _SetOnGround(Position, Mover, Collider, AllCollisions);
                _SetOnWall(Position, Mover, Collider, AllCollisions);
            }

            // Teleport.
            if (Mover.Teleported)
            {
                List<Collision> AllCollisions = World.GetComponents<Collision>();
                _SetOnGround(Position, Mover, Collider, AllCollisions);
                _SetOnWall(Position, Mover, Collider, AllCollisions);
                Mover.Teleported = false;
            }
        }

        private void _CheckMovement(Position Position, Collision Collider, List<Collision> AllCollisions)
        {
            int Left = Math.Min(Position.Last.X + Collider.Rectangle().Left, Position.Current.X + Collider.Rectangle().Left);
            int Right = Math.Max(Position.Last.X + Collider.Rectangle().Right, Position.Current.X + Collider.Rectangle().Right);
            int Up = Math.Min(Position.Last.Y + Collider.Rectangle().Top, Position.Current.Y + Collider.Rectangle().Top);
            int Down = Math.Max(Position.Last.Y + Collider.Rectangle().Bottom, Position.Current.Y + Collider.Rectangle().Bottom);

            Rectangle MovementRectangle = new Rectangle(Left, Up, Right - Left, Down - Up);
            Point MovementOffset = new Point(0, 0);
            foreach (var Other in AllCollisions)
            {
                if (Other == Collider)
                {
                    continue;
                }

                Position OtherPos = World.Manager.GetComponent<Position>(Other.Entity);
                if (Other.Shape() == Collision.ShapeType.Rect)
                {
                    if (_RectToRect(MovementRectangle, Other.Rectangle(), MovementOffset - OtherPos.Current))
                    {
                        _AddHitCollision(Collider, Other);
                        _AddHitCollision(Other, Collider);
                    }
                }
                else
                {
                    if (_RectToGrid(MovementRectangle, Other, MovementOffset - OtherPos.Current))
                    {
                        _AddHitCollision(Collider, Other);
                        _AddHitCollision(Other, Collider);
                    }
                }
            }
        }

        private void _AdjustMovement(Position Position, Movement Mover, Collision Collider)
        {
            if(Collider.CollisionCount() == 0)
            {
                return;
            }

            int DistanceX = Math.Abs(Position.Current.X - Position.Last.X);
            int DistanceY = Math.Abs(Position.Current.Y - Position.Last.Y);
            int DirectionX = Math.Sign(Position.Current.X - Position.Last.X);
            int DirectionY = Math.Sign(Position.Current.Y - Position.Last.Y);
            Point OffsetX = new Point(DirectionX, 0);
            Point OffsetY = new Point(0, DirectionY);
            Point NoOffset = new Point(0, 0);

            Position.Current = Position.Last;
            List<Collision> HitInteractables = new List<Collision>();
            List<Collision> HitDamagers = new List<Collision>();
            bool Moved = true; // This method is not called if movement did not occur.
            while (Moved)
            {
                Moved = false;

                // Interactable.
                foreach(var Other in Collider.Interactables)
                {
                    // TODO: Remove components from Collisions instead.
                    if(HitInteractables.Contains(Other))
                    {
                        continue;
                    }

                    if (_Check(Position, Collider, Other, NoOffset))
                    {
                        HitInteractables.Add(Other);
                    }
                }

                // Damaging.
                foreach (var Other in Collider.Damagers)
                {
                    // TODO: Remove components from Collisions instead.
                    if (HitDamagers.Contains(Other))
                    {
                        continue;
                    }

                    if (_Check(Position, Collider, Other, NoOffset))
                    {
                        HitDamagers.Add(Other);
                        Collider.Interactables = HitInteractables;
                        Collider.Damagers = HitDamagers;
                        return;
                    }
                }

                if (DistanceX == 0 && DistanceY == 0)
                {
                    break;
                }

                // X adjustment.
                foreach (var Other in Collider.Blockers)
                {
                    if (DistanceX > 0 && _Check(Position, Collider, Other, OffsetX))
                    {
                        // TODO: HACK: Blocking interactables!
                        if(Collider.Interactables.Contains(Other) && !HitInteractables.Contains(Other))
                        {
                            HitInteractables.Add(Other);
                        }

                        _StopX(Position, Mover);
                        DistanceX = 0;
                    }
                }
                if (DistanceX > 0)
                {
                    Position.Current.X += DirectionX;
                    DistanceX -= 1;
                    Moved = true;
                }

                // Y adjustment
                foreach (var Other in Collider.Blockers)
                {
                    if (DistanceY > 0 && _Check(Position, Collider, Other, OffsetY))
                    {
                        // TODO: HACK: Blocking interactables!
                        if (Collider.Interactables.Contains(Other) && !HitInteractables.Contains(Other))
                        {
                            HitInteractables.Add(Other);
                        }

                        _StopY(Position, Mover);
                        DistanceY = 0;
                    }
                }
                if (DistanceY > 0)
                {
                    Position.Current.Y += DirectionY;
                    DistanceY -= 1;
                    Moved = true;
                }
            }
            Collider.Interactables = HitInteractables;
            Collider.Damagers = HitDamagers;
        }

        private void _SetOnGround(Position Position, Movement Mover, Collision Collider, List<Collision> AllCollisions)
        {
            foreach (Collision Other in AllCollisions)
            {
                if (Other.Mask() != Mask.Solid)
                {
                    continue;
                }

                if (_Check(Position, Collider, Other, new Point(0, 1)))
                {
                    Mover.OnGround = true;
                    return;
                }
            }
            Mover.OnGround = false;
        }

        private void _SetOnWall(Position Position, Movement Mover, Collision Collider, List<Collision> AllCollisions)
        {
            if(Mover.OnGround)
            {
                Mover.OnWall = 0;
                return;
            }

            foreach (Collision Other in AllCollisions)
            {
                if (Other.Mask() != Mask.Solid)
                {
                    continue;
                }

                if (_Check(Position, Collider, Other, new Point(1, 0)))
                {
                    Mover.OnWall = 1;
                    return;
                }
                else if (_Check(Position, Collider, Other, new Point(-1, 0)))
                {
                    Mover.OnWall = -1;
                    return;
                }
            }
            Mover.OnWall = 0;
        }

        private void _AddHitCollision(Collision Collider, Collision Other)
        {
            // TODO: Blocking mask won't have damage!
            if(Collider.InteractableMasks.Contains(Other.Mask()))
            {
                Collider.Interactables.Add(Other);
            }

            if (Collider.DamagingMasks.Contains(Other.Mask()))
            {
                Collider.Damagers.Add(Other);
            }

            if (Collider.BlockingMasks.Contains(Other.Mask()))
            {
                Collider.Blockers.Add(Other);
            }
        }

        private bool _Check(Position Position, Collision Collider, Collision Other, Point Offset)
        {
            Position OtherPos = World.Manager.GetComponent<Position>(Other.Entity);
            if (Other.Shape() == Collision.ShapeType.Rect)
            {
                return _RectToRect(Collider.Rectangle(), Other.Rectangle(), Offset + Position.Current - OtherPos.Current);
            }
            else
            {
                return _RectToGrid(Collider.Rectangle(), Other, Offset + Position.Current - OtherPos.Current);
            }
        }

        private bool _RectToRect(Rectangle R1, Rectangle R2, Point Offset)
        {
            R1.Offset(Offset);
            return R1.Intersects(R2);
        }

        private bool _RectToGrid(Rectangle R, Collision Other, Point Offset)
        {
            R.Offset(Offset);

            int LeftIdx = Math.Max(R.Left / Other.Grid().TileSize, 0);
            int RightIdx = (int)Math.Min(Math.Ceiling((double)R.Right / Other.Grid().TileSize), Other.Grid().Columns);
            int TopIdx = Math.Max(R.Top / Other.Grid().TileSize, 0);
            int BottomIdx = (int)Math.Min(Math.Ceiling((double)R.Bottom / Other.Grid().TileSize), Other.Grid().Rows);

            for (int x = LeftIdx; x < RightIdx; x++)
            {
                for (int y = TopIdx; y < BottomIdx; y++)
                {
                    if (Other.Grid().Cells[x + y * Other.Grid().Columns])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void _StopX(Position Position, Movement Mover)
        {
            Mover.Velocity.X = 0;
            Position.Remainder.X = 0;
        }

        private void _StopY(Position Position, Movement Mover)
        {
            Mover.Velocity.Y = 0;
            Position.Remainder.Y = 0;
        }
    }
}
