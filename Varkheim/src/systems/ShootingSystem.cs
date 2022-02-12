using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class ShootingSystem : BaseSystem
    {
        public ShootingSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Position>();
            AddComponentType<Player>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Position Position = (Position)Components[0];
            Player Player = (Player)Components[1];

            if(Player.Shooting)
            {
                Point SpiritPos = new Point(Position.Current.X + Position.Facing * Player.ShootingOffset.X, Position.Current.Y + Player.ShootingOffset.Y);
                World.CurrentEntities.Add(Factory.Spirit(World.Manager, SpiritPos, new Vector2(Position.Facing * 100, 0)));
                Player.Shooting = false;
            }
        }
    }
}
