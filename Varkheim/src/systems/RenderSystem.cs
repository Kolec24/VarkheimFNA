using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ECS;


namespace Varkheim
{
    class RenderSystem : BaseSystem
    {
        public RenderSystem() : base(SystemType.Render)
        {
            AddComponentType<Position>();
            AddComponentType<Sprite>();
        }

        public override void RenderComponents(float DeltaTime, List<BaseComponent> Components, SpriteBatch Batch)
        {
            var PosComp = (Position)Components[0];
            var SpriteComp = (Sprite)Components[1];

            Batch.Draw(SpriteComp.Texture, new Vector2(PosComp.X, PosComp.Y), Color.White);
        }
    }
}
