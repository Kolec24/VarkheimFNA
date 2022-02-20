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

            AddComponentType<Position>();
            AddComponentType<Collision>();
            AddComponentType<Soul>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Position = (Position)Components[0];
            var Collider = (Collision)Components[1];
            var Soul = (Soul)Components[2];

            if(Collider.Collisions.Count() == 0)
            {
                return;
            }

            Teleport Teleport = World.Manager.GetComponent<Teleport>(Soul.Owner);
            if (Teleport == null)
            {
                return;
            }

            Teleport.TargetPosition = Position.Current;
            Teleport.Teleporting = true;

            World.RemoveEntity(Soul.Entity);
        }
    }
}
