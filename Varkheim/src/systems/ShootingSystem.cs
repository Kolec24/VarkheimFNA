using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

using EntityHandle = System.Object;

namespace Varkheim
{
    class ShootingSystem : BaseSystem
    {
        public ShootingSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Position>();
            AddComponentType<Shoot>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Position Position = (Position)Components[0];
            Shoot Shooter = (Shoot)Components[1];

            if(!Shooter.Shooting)
            {
                return;
            }

            Point Offset;
            Vector2 Velocity;
            Point ProjectilePosition;
            if(Position.Facing.Y == -1)
            {
                Offset = Shooter.VerticalOffset();
                Velocity = new Vector2(0, Position.Facing.Y * Shooter.Velocity);
                ProjectilePosition = new Point(Position.Current.X + Position.Facing.X * Offset.X, Position.Current.Y - Position.Facing.Y * Offset.Y);
            }
            else
            {
                Offset = Shooter.HorizontalOffset();
                Velocity = new Vector2(Position.Facing.X * Shooter.Velocity, 0);
                ProjectilePosition = new Point(Position.Current.X + Position.Facing.X * Offset.X, Position.Current.Y + Offset.Y);
            }

            switch(Shooter.Projectile())
            {
                case Projectile.Soul:
                    EntityHandle Soul = Factory.Soul(World.Manager, ProjectilePosition, Position.Facing.X, Velocity, Shooter.Entity);
                    World.AddEntity(Soul);
                    break;
                default:
                    break;
            }
            Shooter.Shooting = false;
        }
    }
}
