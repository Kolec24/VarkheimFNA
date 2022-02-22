using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class DamageSystem : BaseSystem
    {
        public DamageSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Collision>();
            AddComponentType<Damageable>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Collider = (Collision)Components[0];
            var Damageable = (Damageable)Components[1];

            if(Collider.Damagers.Count() == 0)
            {
                return;
            }

            _ApplyDamage(Collider, Damageable);
        }

        private void _ApplyDamage(Collision Collider, Damageable Damageable)
        {
            Damageable.Health--;
        }
    }
}
