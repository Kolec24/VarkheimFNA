using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class OpenSystem : BaseSystem
    {
        public OpenSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Collision>();
            AddComponentType<Inventory>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Collider = (Collision)Components[0];
            var Inventory = (Inventory)Components[1];

            List<Collision> OpenedColliders = new List<Collision>();
            foreach(var Other in Collider.Interactables)
            {
                var Open = World.Manager.GetComponent<Openable>(Other.Entity);
                if(Open == null)
                {
                    continue;
                }

                if(Inventory.Items.ContainsKey(Open.Key()) && Inventory.Items[Open.Key()] > 0)
                {
                    Inventory.Items[Open.Key()]--;
                    OpenedColliders.Add(Other);
                }
            }

            // TODO: Think of cleaner solution.
            for (int i = OpenedColliders.Count() - 1; i >= 0; i--)
            {
                Collider.Interactables.Remove(OpenedColliders[i]);
                World.RemoveEntity(OpenedColliders[i].Entity);
            }
        }
    }
}
