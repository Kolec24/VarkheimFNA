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
    class SpriteSystem : BaseSystem
    {
        public SpriteSystem(World InWorld) : base(SystemType.Render)
        {
            World = InWorld;

            AddComponentType<Position>();
            AddComponentType<Animation>();
        }

        public override void RenderComponents(float DeltaTime, List<BaseComponent> Components, SpriteBatch Batch)
        {
            Position Position = (Position)Components[0];
            Animation Animator = (Animation)Components[1];

            if (!Animator.InValidState)
            {
                return;
            }

            Sprite.Animation Animation = Animator.Sprite.Animations[Animator.AnimationIndex];
            Sprite.Frame Frame = Animation.Frames[Animator.FrameIndex];

            Batch.Draw(Frame.Texture, new Vector2(Position.Current.X, Position.Current.Y)
                , new Rectangle(0, 0, Frame.Texture.Width, Frame.Texture.Height), Color.White, 0, Animator.Sprite.Origin, new Vector2(Position.Facing, 1), SpriteEffects.None, Animator.Depth);
        }
    }
}
