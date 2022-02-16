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

            _CheckJump(Mover, Jumper);
            _ApplyJump(Mover, Jumper, DeltaTime);
        }

        private void _CheckJump(Movement Mover, Jump Jumper)
        {
            if(!Jumper.Jumping)
            {
                return;
            }

            if (Mover.OnGround)
            {
                Jumper.GroundJumping = true;
                Jumper.WallJumping = false;
            }
            else if (Mover.OnWall != 0)
            {
                Jumper.GroundJumping = false;
                Jumper.WallJumping = true;
            }

            Jumper.Jumping = false;
        }

        private void _ApplyJump(Movement Mover, Jump Jumper, float DeltaTime)
        {
            if (Mover.OnGround && Jumper.GroundJumping)
            {
                Jumper.GroundTimer = Jumper.MaxTimer();
            }

            if (Jumper.GroundTimer > 0)
            {
                Mover.Velocity.Y = -1 * Jumper.Velocity.Y;
                Jumper.GroundTimer -= DeltaTime;
            }
            else
            {
                Jumper.GroundJumping = false;
            }

            if (Mover.OnWall != 0 && Jumper.WallJumping)
            {
                Jumper.WallTimer = Jumper.MaxTimer();
                Jumper.Direction = -1 * Mover.OnWall;
            }

            if (Jumper.WallTimer > 0)
            {
                Mover.Velocity.Y = -1 * Jumper.Velocity.Y;
                Mover.Velocity.X = Jumper.Direction * Jumper.Velocity.X;
                Jumper.WallTimer -= DeltaTime;
            }
            else
            {
                Jumper.WallJumping = false;
            }
        }
    }
}
