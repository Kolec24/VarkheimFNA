using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class SoulSystem : BaseSystem
    {
        public SoulSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Collision>();
            AddComponentType<Soul>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Collider = (Collision)Components[0];
            var Soul = (Soul)Components[1];

            if(Collider.Interactables.Count() == 0)
            {
                return;
            }

            Teleport Teleport = World.Manager.GetComponent<Teleport>(Soul.Owner);
            if (Teleport == null)
            {
                return;
            }

            Teleport.Soul = Soul.Entity;
            Teleport.Teleporting = true;
        }
    }
}
