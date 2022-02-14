using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ECS;

namespace Varkheim
{
    class AnimationSystem : BaseSystem
    {
        public AnimationSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Animation>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Animation Animator = (Animation)Components[0];

            int CurrentAnimationIndex = _GetAnimationIndex(Animator, Animator.CurrentAnimation);
            if (Animator.AnimationIndex != CurrentAnimationIndex)
            {
                Animator.AnimationIndex = CurrentAnimationIndex;
                Animator.FrameIndex = 0;
                Animator.FrameCounter = 0;
            }

            Animator.InValidState = _IsInValidState(Animator);
            if (!Animator.InValidState)
            {
                return;
            }

            _UpdateAnimation(Animator, DeltaTime);
        }

        private void _UpdateAnimation(Animation Animator, float DeltaTime)
        {
            Sprite.Animation Animation = Animator.Sprite.Animations[Animator.AnimationIndex];
            Sprite.Frame Frame= Animation.Frames[Animator.FrameIndex];

            Animator.FrameCounter += DeltaTime;
            if(Animator.FrameCounter > Frame.Duration)
            {
                Animator.FrameCounter -= Frame.Duration;
                Animator.FrameIndex++;

                if(Animator.FrameIndex >= Animation.Frames.Count())
                {
                    Animator.FrameIndex = 0;
                }
            }
        }

        private bool _IsInValidState(Animation Animator)
        {
            return
                Animator.Sprite != null &&
                Animator.AnimationIndex >= 0 &&
                Animator.AnimationIndex < Animator.Sprite.Animations.Count() &&
                Animator.FrameIndex >= 0 &&
                Animator.FrameIndex < Animator.Sprite.Animations[Animator.AnimationIndex].Frames.Count();
        }

        private int _GetAnimationIndex(Animation Animator, string Name)
        {
            for(int i = 0; i < Animator.Sprite.Animations.Count(); i++)
            {
                if(Animator.Sprite.Animations[i].Name == Name)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
