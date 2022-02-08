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
            AddComponentType<Collision>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Position = (Position)Components[0];
            var Collider = (Collision)Components[1];

            if (Collider.Shape() != Collision.ShapeType.Rect)
            {
                return;
            }

            List<Collision> AllCollisions = World.GetComponents<Collision>();

            Collider.Collisions.Clear();
            _CheckCollisions(Position, Collider, AllCollisions);
        }

        // TODO: Clean up places with collision check.
        private void _CheckCollisions(Position Position, Collision Collider, List<Collision> AllCollisions)
        {
            if (Collider.Shape() != Collision.ShapeType.Rect)
            {
                return;
            }

            int ColWidth = Collider.Rectangle().Width;
            int ColHeight = Collider.Rectangle().Height;
            int Left = Math.Min(Position.Last.X + Collider.Rectangle().Left, Position.Current.X + Collider.Rectangle().Left);
            int Right = Math.Max(Position.Last.X + Collider.Rectangle().Right, Position.Current.X + Collider.Rectangle().Right);
            int Up = Math.Min(Position.Last.Y + Collider.Rectangle().Top, Position.Current.Y + Collider.Rectangle().Top);
            int Down = Math.Max(Position.Last.Y + Collider.Rectangle().Bottom, Position.Current.Y + Collider.Rectangle().Bottom);

            Rectangle MovementRectangle = new Rectangle(Left, Up, Right - Left, Down - Up);
            foreach (var Other in AllCollisions)
            {
                if(Other == Collider)
                {
                    continue;
                }

                if (Other.Shape() == Collision.ShapeType.Rect)
                {
                    Position OtherPos = World.Manager.GetComponent<Position>(Other.Entity);
                    Point OtherOffset = new Point(0, 0);
                    if (OtherPos != null)
                    {
                        OtherOffset = OtherPos.Current;
                    }

                    Rectangle OtherRectangle = Other.Rectangle();
                    OtherRectangle.Offset(OtherOffset);
                    if(MovementRectangle.Intersects(OtherRectangle))
                    {
                        Collider.Collisions.Add(Other);
                    }
                }
                else
                {
                    int LeftIdx = Math.Max(MovementRectangle.Left / Other.Grid().TileSize, 0);
                    int RightIdx = (int)Math.Min(Math.Ceiling((double)MovementRectangle.Right / Other.Grid().TileSize), Other.Grid().Columns);
                    int TopIdx = Math.Max(MovementRectangle.Top / Other.Grid().TileSize, 0);
                    int BottomIdx = (int)Math.Min(Math.Ceiling((double)MovementRectangle.Bottom / Other.Grid().TileSize), Other.Grid().Rows);

                    bool Hit = false;
                    for (int x = LeftIdx; x < RightIdx; x++)
                    {
                        for (int y = TopIdx; y < BottomIdx; y++)
                        {
                            if (Other.Grid().Cells[x + y * Other.Grid().Columns])
                            {
                                Collider.Collisions.Add(Other);
                                Hit = true;
                                break;
                            }
                        }
                        if(Hit)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
