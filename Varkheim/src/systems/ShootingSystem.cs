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
            AddComponentType<Shooter>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Position Position = (Position)Components[0];
            Shooter Shooter = (Shooter)Components[1];

            if(Shooter.Shooting)
            {
                Point ProjectilePos = new Point(Position.Current.X + Position.Facing * Shooter.Offset().X, Position.Current.Y + Shooter.Offset().Y);
                switch(Shooter.Projectile())
                {
                    case Projectile.Spirit:
                        World.CurrentEntities.Add(Factory.Spirit(World.Manager, ProjectilePos, Position.Facing, new Vector2(Position.Facing * 100, 0)));
                        break;
                    default:
                        break;
                }
                Shooter.Shooting = false;
            }
        }
    }
}
