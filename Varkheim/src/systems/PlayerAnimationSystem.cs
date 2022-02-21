using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;
using Microsoft.Xna.Framework.Input;

namespace Varkheim
{
    class PlayerAnimationSystem : BaseSystem
    {
        // TODO: Probably need to think about different way of playing animations, since this works only for player! Possibly can merge this to PlayerControlSystem.
        public PlayerAnimationSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Movement>();
            // TODO: Add shotting and teleporting animation.
            AddComponentType<Shoot>();
            AddComponentType<Teleport>();
            AddComponentType<Animation>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Movement Mover = (Movement)Components[0];
            Shoot Shooter = (Shoot)Components[1];
            Teleport Teleporter = (Teleport)Components[2];
            Animation Animator = (Animation)Components[3];

            if (!Mover.OnGround)
            {
                if (Mover.Velocity.Y > 50)
                {
                    Animator.Play("Fall");
                }
                else
                {
                    Animator.Play("Jump");
                }
                return;
            }

            if(Mover.Direction != 0)
            {
                Animator.Play("Walk");
            }
            else
            {
                Animator.Play("Idle");
            }
        }
    }
}
