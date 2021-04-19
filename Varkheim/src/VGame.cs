using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ECS;

using EntityHandle = System.Object;

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
        float Scale;
        Matrix ScaleMatrix;

        public VGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "content";

            BackBufferWidth = 1280;
            BackBufferHeight = 720;
            BufferWidth = 320;
            BufferHeight = 240;

            Scale = Math.Min((float)BackBufferWidth / BufferWidth, (float)BackBufferHeight / BufferHeight);
            ScaleMatrix = Matrix.CreateScale(Scale);

            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            World = new World();
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
