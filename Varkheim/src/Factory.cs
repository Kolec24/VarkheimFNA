using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Squared.Tiled;
using ECS;

using EntityHandle = System.Object;

namespace Varkheim
{
    static class Factory
    {
        public static EntityHandle Player(EntityManager Manager, Point NewPosition)
        {
            var Input = new Input();
            var Position = new Position(NewPosition.X, NewPosition.Y);
            var Mover = new Movement();
            var Collider = new Collision(Mask.Player, new Rectangle(-4, -16, 8, 16));
            var Physics = new Physics();
            {
                Physics.MaxGroundSpeed = 70;
                Physics.MaxAirSpeed = 60;
                Physics.MaxFallingSpeed = 300;
                Physics.GroundAccel = 300;
                Physics.AirAccel = 200;
                Physics.Gravity = 700;
                Physics.GroundFriction = 1000;
                Physics.AirFriction = 700;
                Physics.JumpVelocity = 120;
            }
            var SpriteComp = new Sprite(ContentLoader.FindSprite("player.png"), new Vector2(4, 16));

            EntityHandle Player = Manager.AddEntity();
            {
                Manager.AddComponent<Input>(Player, Input);
                Manager.AddComponent<Position>(Player, Position);
                Manager.AddComponent<Movement>(Player, Mover);
                Manager.AddComponent<Sprite>(Player, SpriteComp);
                Manager.AddComponent<Collision>(Player, Collider);
                Manager.AddComponent<Physics>(Player, Physics);
            }
            return Player;
        }

        public static EntityHandle Spirit(EntityManager Manager, Point NewPosition)
        {
            var Position = new Position(NewPosition.X, NewPosition.Y);
            var Mover = new Movement();
            return Manager.AddEntity();
        }

        public static EntityHandle Tilemap(EntityManager Manager, int Horizontal, int Vertical)
        {
            Map NewMap = ContentLoader.FindMap(Horizontal, Vertical);
            var Tilemap = new Tilemap(NewMap);
            var SolidCells = _SolidCells(NewMap);
            var Collider = new Collision(Mask.Solid, NewMap.Width, NewMap.Height, 8, SolidCells);
            EntityHandle Terrain = Manager.AddEntity();
            Manager.AddComponent<Collision>(Terrain, Collider);
            Manager.AddComponent<Tilemap>(Terrain, Tilemap);
            return Terrain;
        }

        private static List<bool> _SolidCells(Map Map)
        {
            int Width = Map.Width;
            int Height = Map.Height;
            bool[] Cells = new bool[Width * Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Map.Layers["Solid"].Tiles[x + y * Width] > 0)
                        Cells[x + y * Width] = true;
                    else
                        Cells[x + y * Width] = false;
                }
            }

            return new List<bool>(Cells);
        }
    }
}
