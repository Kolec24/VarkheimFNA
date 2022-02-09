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

            AddComponentType<Position>();
            AddComponentType<Tilemap>();
        }

        public override void RenderComponents(float DeltaSystem, List<BaseComponent> Components, SpriteBatch Batch)
        {
            var Position = (Position)Components[0];
            var Tilemap = (Tilemap)Components[1];

            Tilemap.Map().Draw(Batch, new Rectangle(Position.Current.X, Position.Current.Y, World.Game.BackBufferWidth, World.Game.BackBufferHeight), new Vector2(0, 0));
        }
    }
}
