using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            AddComponentType<Sprite>();
        }

        public override void RenderComponents(float DeltaTime, List<BaseComponent> Components, SpriteBatch Batch)
        {
            var PosComp = (Position)Components[0];
            var SpriteComp = (Sprite)Components[1];

            Batch.Draw(SpriteComp.Texture, new Vector2(PosComp.Pos.X, PosComp.Pos.Y) - SpriteComp.Origin, Color.White);
        }
    }
}
