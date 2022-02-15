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
            var Teleport = (Teleport)Components[2];

            if(!Teleport.Teleporting)
            {
                return;
            }

            Position.Current = Teleport.TargetPosition;
            Mover.Velocity = Vector2.Zero;
            // TODO: Hack!
            Mover.OnGround = false;
            Teleport.Teleporting = false;
        }
    }
}
