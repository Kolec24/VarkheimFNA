using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class SpiritSystem : BaseSystem
    {
        public SpiritSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Position>();
            AddComponentType<Collision>();
            AddComponentType<Spirit>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Position = (Position)Components[0];
            var Collider = (Collision)Components[1];
            var Spirit = (Spirit)Components[2];

            if(Collider.Collisions.Count() == 0)
            {
                return;
            }

            Teleport Teleport = World.Manager.GetComponent<Teleport>(Spirit.Owner);
            if (Teleport == null)
            {
                return;
            }

            Teleport.TargetPosition = Position.Current - Spirit.Offset;
            Teleport.Teleporting = true;

            World.RemoveEntity(Spirit.Entity);
        }
    }
}
