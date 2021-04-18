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
        EntityManager Manager;

        public VGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "content";
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
            Manager = new EntityManager();
        }

        protected override void LoadContent()
        {
            Batch = new SpriteBatch(GraphicsDevice);
            ContentLoader.Load(Content);

            base.LoadContent();
        }

        protected override void Update(GameTime GameTime)
        {
            Manager.Update(GameTime);

            base.Update(GameTime);
        }

        protected override void Draw(GameTime GameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Manager.Render(GameTime);

            base.Draw(GameTime);
        }
    }
}
