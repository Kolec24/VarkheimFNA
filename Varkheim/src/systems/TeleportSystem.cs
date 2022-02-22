using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class TeleportSystem : BaseSystem
    {
        public TeleportSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Position>();
            AddComponentType<Movement>();
            AddComponentType<Teleport>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Position = (Position)Components[0];
            var Mover = (Movement)Components[1];
            var Teleporter = (Teleport)Components[2];

            if(!Teleporter.Teleporting)
            {
                return;
            }

            Position SoulPosition = World.Manager.GetComponent<Position>(Teleporter.Soul);
            if(SoulPosition == null)
            {
                Console.WriteLine("Invalid soul!");
                return;
            }

            Point Adjustment = new Point(0, 0);
            if(SoulPosition.Facing.Y != 0)
            {
                Adjustment.Y = SoulPosition.Facing.Y;
            }
            else
            {
                Adjustment.X = SoulPosition.Facing.X;
            }

            Position.Current = SoulPosition.Current - Adjustment;
            // TODO: Should stop all movement - stopping X, Y and jumping.
            Mover.Velocity = Vector2.Zero;
            Mover.Teleported = true;

            World.RemoveEntity(Teleporter.Soul);
            Teleporter.Teleporting = false;
        }
    }
}
