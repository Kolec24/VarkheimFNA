using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class ResolveSystem : BaseSystem
    {
        public ResolveSystem(World InWorld) : base(SystemType.Gameplay)
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

            _AdjustMove(Position, Mover, Collider);
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
                if(Other.Mask() == Mask.Solid)
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

        // TODO: Clean up places with collision check.
        private bool _Check(Position Position, Collision Collider, Collision Other, Point Offset)
        {
            Position OtherPos = World.Manager.GetComponent<Position>(Other.Entity);
            Point OtherOffset = new Point(0, 0);
            if (OtherPos != null)
            {
                OtherOffset = OtherPos.Current;
            }

            if (Other.Shape() == Collision.ShapeType.Rect)
            {
                Rectangle R1 = Collider.Rectangle();
                R1.Offset(Position.Current + Offset - OtherOffset);
                Rectangle R2 = Other.Rectangle();
                return R1.Intersects(R2);
            }
            else
            {
                Rectangle Rect = Collider.Rectangle();
                Rect.Offset(Position.Current + Offset - OtherOffset);

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

        // Probably useless. TODO: Debug jump timer on Y hit.
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
