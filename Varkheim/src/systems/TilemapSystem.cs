using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ECS;


namespace Varkheim
{
    class TilemapSystem : BaseSystem
    {
        public TilemapSystem(World InWorld) : base(SystemType.Render)
        {
            World = InWorld;

            AddComponentType<Tilemap>();
            AddComponentType<Sprite>();
        }

        public override void RenderComponents(float DeltaSystem, List<BaseComponent> Components, SpriteBatch Batch)
        {
            var Tilemap = (Tilemap)Components[0];
            var Sprite = (Sprite)Components[1];

            for (int x = 0; x < Tilemap.Columns(); x++)
            {
                for (int y = 0; y < Tilemap.Rows(); y++)
                {
                    if (Tilemap.Cells()[x + y * Tilemap.Columns()])
                    {
                        Batch.Draw(Sprite.Texture, new Vector2(x * Tilemap.TileWidth(), y * Tilemap.TileHeight()), Color.White);
                    }
                }
            }
        }
    }
}
