using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class DeathSystem : BaseSystem
    {
        public DeathSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Animation>();
            AddComponentType<Damageable>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Animator = (Animation)Components[0];
            var Damageable = (Damageable)Components[1];

            if(Damageable.Health <= 0)
            {
                _Kill(Damageable, Animator, DeltaTime);
            }
        }

        private void _Kill(Damageable Damageable, Animation Animator, float DeltaTime)
        {
            if (Animator.CurrentAnimation() != "Death")
            {
                Damageable.DeathTimer = Damageable.MaxDeathTimer();
                Animator.Play("Death");
                World.SetInvalid(Damageable.Entity);
            }

            if(Damageable.StopGame())
            {
                World.DeathFreeze(Damageable.Entity, Damageable.MaxDeathTimer());
                return;
            }
            else if(Damageable.DeathTimer > 0)
            {
                Damageable.DeathTimer = Math.Max(Damageable.DeathTimer - DeltaTime, 0);
                return;
            }

            World.RemoveEntity(Damageable.Entity);
        }
    }
}
