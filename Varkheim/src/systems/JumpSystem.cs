using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class JumpSystem : BaseSystem
    {
        public JumpSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Movement>();
            AddComponentType<Jump>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Mover = (Movement)Components[0];
            var Jumper = (Jump)Components[1];

            if(!Jumper.Jumping)
            {
                return;
            }

            _ApplyJump(Mover, Jumper, DeltaTime);
        }

        private void _ApplyJump(Movement Mover, Jump Jumper, float DeltaTime)
        {
            if (Mover.OnGround)
            {
                Jumper.Timer = Jumper.MaxTimer();
            }

            if (Jumper.Timer > 0)
            {
                Mover.Velocity.Y = -1 * Jumper.Velocity;
                Jumper.Timer -= DeltaTime;
            }
            else
            {
                Jumper.Jumping = false;
            }
        }
    }
}
