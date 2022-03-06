using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class PickupSystem : BaseSystem
    {
        public PickupSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Collision>();
            AddComponentType<Inventory>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Collider = (Collision)Components[0];
            var Inventory = (Inventory)Components[1];

            List<Collision> PickedColliders = new List<Collision>();
            foreach(var Other in Collider.Interactables)
            {
                var Collectible = World.Manager.GetComponent<Collectible>(Other.Entity);
                if(Collectible == null)
                {
                    continue;
                }

                if(Inventory.Items.ContainsKey(Collectible.Item()))
                {
                    Inventory.Items[Collectible.Item()]++;
                }
                else
                {
                    // TODO: Add amount if needed.
                    Inventory.Items.Add(Collectible.Item(), 1);
                }

                PickedColliders.Add(Other);
            }

            // TODO: Think of cleaner solution.
            for(int i = PickedColliders.Count() - 1; i >= 0; i--)
            {
                Collider.Interactables.Remove(PickedColliders[i]);
                World.RemoveEntity(PickedColliders[i].Entity);
            }
        }
    }
}
