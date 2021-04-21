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
    public class VGame : Game
    {
        GraphicsDeviceManager Graphics;
        SpriteBatch Batch;
        World World;

        int BackBufferWidth;
        int BackBufferHeight;
        int BufferWidth;
        int BufferHeight;
        public int Columns;
        public int Rows;
        public int TileWidth;
        public int TileHeight;
        float Scale;
        Matrix ScaleMatrix;

        public VGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "content";

            BackBufferWidth = 1280;
            BackBufferHeight = 960;
            BufferWidth = 320;
            BufferHeight = 240;

            TileWidth = 8;
            TileHeight = 8;
            Columns = BufferWidth / TileWidth;
            Rows = BufferHeight / TileHeight;

            Scale = Math.Min((float)BackBufferWidth / BufferWidth, (float)BackBufferHeight / BufferHeight);
            ScaleMatrix = Matrix.CreateScale(Scale);

            Graphics.PreferredBackBufferWidth = BackBufferWidth;
            Graphics.PreferredBackBufferHeight = BackBufferHeight;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            World = new World(this);
            World.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Batch = new SpriteBatch(GraphicsDevice);
            ContentLoader.Load(Content);
            World.Load();

            base.LoadContent();
        }

        protected override void Update(GameTime GameTime)
        {
            World.Update(GameTime);

            base.Update(GameTime);
        }

        protected override void Draw(GameTime GameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, ScaleMatrix);
            World.Render(GameTime, Batch);
            Batch.End();

            base.Draw(GameTime);
        }
    }
}
