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

            List<Collision> AllCollisions = World.GetComponents<Collision>();

            Collider.Collisions.Clear();

            _CheckMovement(Position, Collider, AllCollisions);
            _AdjustMove(Position, Mover, Collider);
            _SetOnGround(Position, Mover, Collider, AllCollisions);
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
                        Collider.Collisions.Add(Other);
                    }
                }
                else
                {
                    if (_RectToGrid(MovementRectangle, Other, MovementOffset - OtherPos.Current))
                    {
                        Collider.Collisions.Add(Other);
                    }
                }
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

        private void _AdjustMove(Position Position, Movement Mover, Collision Collider)
        {
            int DistanceX = Math.Abs(Position.Current.X - Position.Last.X);
            int DistanceY = Math.Abs(Position.Current.Y - Position.Last.Y);
            if (DistanceX == 0 && DistanceY == 0)
            {
                return;
            }

            int DirectionX = Math.Sign(Position.Current.X - Position.Last.X);
            int DirectionY = Math.Sign(Position.Current.Y - Position.Last.Y);
            Point OffsetX = new Point(DirectionX, 0);
            Point OffsetY = new Point(0, DirectionY);

            bool Adjust = false;

            foreach (var Other in Collider.Collisions)
            {
                // TODO: Add more conditions.
                if (Other.Mask() == Mask.Solid)
                {
                    Adjust = true;
                    Position.Current = Position.Last;
                }
            }

            while (Adjust && (DistanceX > 0 || DistanceY > 0))
            {
                // TODO: Find better solution than two separate loops.
                foreach (var Other in Collider.Collisions)
                {
                    if (DistanceX != 0 && _Check(Position, Collider, Other, OffsetX))
                    {
                        switch (Other.Mask())
                        {
                            case Mask.Solid:
                                _StopX(Position, Mover);
                                DistanceX = 0;
                                break;

                            default:
                                break;
                        }
                    }
                }
                if (DistanceX > 0)
                {
                    Position.Current.X += DirectionX;
                    DistanceX -= 1;
                }

                // TODO: Find better solution than two separate loops.
                foreach (var Other in Collider.Collisions)
                {
                    if (DistanceY != 0 && _Check(Position, Collider, Other, OffsetY))
                    {
                        switch (Other.Mask())
                        {
                            case Mask.Solid:
                                _StopY(Position, Mover);
                                DistanceY = 0;
                                break;

                            default:
                                break;
                        }
                    }
                }
                if (DistanceY > 0)
                {
                    Position.Current.Y += DirectionY;
                    DistanceY -= 1;
                }
            }
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

        private void _SetOnGround(Position Position, Movement Mover, Collision Collider, List<Collision> AllCollisions)
        {
            foreach (Collision Other in AllCollisions)
            {
                if(Other.Mask() != Mask.Solid)
                {
                    continue;
                }

                if(_Check(Position, Collider, Other, new Point(0, 1)))
                {
                    Mover.OnGround = true;
                    return;
                }
            }
            Mover.OnGround = false;
        }
    }
}
