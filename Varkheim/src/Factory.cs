using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;

using EntityHandle = System.Object;

namespace Varkheim
{
    static class Factory
    {
        public static EntityHandle Player(EntityManager Manager, Point NewPosition)
        {
            var Position = new Position(NewPosition.X, NewPosition.Y);
            var Mover = new Movement();
            Mover.MaxGroundSpeed = 70;
            Mover.MaxAirSpeed = 60;
            Mover.MaxFallingSpeed = 300;
            Mover.GroundAccel = 200;
            Mover.AirAccel = 100;
            Mover.Gravity = 500;
            Mover.GroundFriction = 300;
            Mover.AirFriction = 70;
            var SpriteComp = new Sprite(ContentLoader.FindSprite("player.png"), new Vector2(4, 16));
            var Collider = new Collision(Mask.Player, new Rectangle(-4, -16, 8, 16));
            EntityHandle Player = Manager.AddEntity();
            Manager.AddComponent<Position>(Player, Position);
            Manager.AddComponent<Movement>(Player, Mover);
            Manager.AddComponent<Sprite>(Player, SpriteComp);
            Manager.AddComponent<Collision>(Player, Collider);
            Manager.AddComponent<Input>(Player, new Input());
            return Player;
        }

        public static EntityHandle Tilemap(EntityManager Manager, int Columns, int Rows, int TileWidth, int TileHeight, List<bool> Cells)
        {
            var Tilemap = new Tilemap(Columns, Rows, TileWidth, TileHeight, Cells);
            var SpriteComp = new Sprite(ContentLoader.FindTileset("tileset.png"), new Vector2(0, 0));
            var Collider = new Collision(Mask.Solid, Columns, Rows, 8, Cells);
            EntityHandle Terrain = Manager.AddEntity();
            Manager.AddComponent<Sprite>(Terrain, SpriteComp);
            Manager.AddComponent<Collision>(Terrain, Collider);
            Manager.AddComponent<Tilemap>(Terrain, Tilemap);
            return Terrain;
        }
    }
}
