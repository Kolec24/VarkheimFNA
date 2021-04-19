using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;

using EntityHandle = System.Object;

namespace Varkheim
{
    class Factory
    {
        public static EntityHandle Player(EntityManager Manager, Vector2 Position)
        {
            var PosComp = new Position(Position.X, Position.Y);
            var VelComp = new Velocity(70, 0);
            var SpriteComp = new Sprite(ContentLoader.FindSprite("player.png"), 5);
            EntityHandle Player = Manager.AddEntity();
            Manager.AddComponent<Position>(Player, PosComp);
            Manager.AddComponent<Velocity>(Player, VelComp);
            Manager.AddComponent<Sprite>(Player, SpriteComp);
            return Player;
        }
    }
}
