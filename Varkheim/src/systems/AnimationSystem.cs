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
        public AnimationSystem(World InWorld) : base(SystemType.Render)
        {
            World = InWorld;

            AddComponentType<Position>();
            AddComponentType<Animation>();
        }

        public override void RenderComponents(float DeltaTime, List<BaseComponent> Components, SpriteBatch Batch)
        {
            Position Position = (Position)Components[0];
            Animation Animator = (Animation)Components[1];

            if (!_IsInValidState(Animator))
            {
                return;
            }

            Sprite.Animation Animation = Animator.Sprite.Animations[Animator.AnimationIndex];
            Sprite.Frame Frame = Animation.Frames[Animator.FrameIndex];

            _UpdateAnimation(Animator, DeltaTime);
            Batch.Draw(Frame.Texture, new Vector2(Position.Current.X, Position.Current.Y)
                , new Rectangle(0, 0, Frame.Texture.Width, Frame.Texture.Height), Color.White, 0, Animator.Sprite.Origin, new Vector2(Position.Facing, 1), SpriteEffects.None, 0);
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
