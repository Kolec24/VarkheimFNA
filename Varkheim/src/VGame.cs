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

        public int BackBufferWidth;
        public int BackBufferHeight;
        public int BufferWidth;
        public int BufferHeight;
        public int Columns;
        public int Rows;
        public int TileWidth;
        public int TileHeight;
        float Scale;
        Matrix ScaleMatrix;
        Matrix CameraMatrix;
        public Point CameraPosition = new Point(0, 0);
        public Point LastCamera = new Point(0, 0);
        public Point NextCamera = new Point(0, 0);
        public float TransitionTime;
        public float CurrentTransitionTime;


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

            TransitionTime = 0.75F;
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

            // To change starting room one needs to change camera!
            World.Load(new Point(0, 0));

            base.LoadContent();
        }

        public void SetCamera(Point TargetCamera)
        {
            CurrentTransitionTime = TransitionTime;
            LastCamera = NextCamera;
            NextCamera = new Point(TargetCamera.X * BufferWidth, TargetCamera.Y * BufferHeight);
        }

        public void MoveCamera(float Progress)
        {
            CameraPosition.X = (int)(Progress * LastCamera.X + (1 - Progress) * NextCamera.X);
            CameraPosition.Y = (int)(Progress * LastCamera.Y + (1 - Progress) * NextCamera.Y);
        }

        protected override void Update(GameTime GameTime)
        {
            float DeltaTime = (float)GameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Check if such update block can be used.
            if (CurrentTransitionTime > 0)
            {
                CurrentTransitionTime = Math.Max(CurrentTransitionTime - DeltaTime, 0);
                MoveCamera(CurrentTransitionTime / TransitionTime);
                base.Update(GameTime);
                return;
            }

            World.Update(DeltaTime);

            base.Update(GameTime);
        }

        protected override void Draw(GameTime GameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            CameraMatrix = Matrix.CreateTranslation(new Vector3((-1) * CameraPosition.X, (-1) * CameraPosition.Y, 0));
            Batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, CameraMatrix * ScaleMatrix);
            World.Render(GameTime, Batch);
            Batch.End();

            base.Draw(GameTime);
        }
    }
}
