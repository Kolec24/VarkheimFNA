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
        public Color BGColor = new Color();
        float Scale;
        Matrix ScaleMatrix;
        Matrix CameraMatrix;
        public Point CameraPosition = new Point(0, 0);
        public Point LastCamera = new Point(0, 0);
        public Point NextCamera = new Point(0, 0);
        public float TransitionTimer;
        public float CurrentTransitionTimer;
        public float CurrentDeathTimer;

        public Point Start = new Point(0, 0);

        public VGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "content";

            BackBufferWidth = 960;//1280;
            BackBufferHeight = 720;//960;
            BufferWidth = 320;
            BufferHeight = 240;

            TileWidth = 8;
            TileHeight = 8;
            Columns = BufferWidth / TileWidth;
            Rows = BufferHeight / TileHeight;

            Scale = Math.Min((float)BackBufferWidth / BufferWidth, (float)BackBufferHeight / BufferHeight);
            ScaleMatrix = Matrix.CreateScale(Scale);

            BGColor.R = 54;
            BGColor.G = 29;
            BGColor.B = 24;
            BGColor.A = 255;

            BGColor = Color.CornflowerBlue;

            Graphics.PreferredBackBufferWidth = BackBufferWidth;
            Graphics.PreferredBackBufferHeight = BackBufferHeight;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();

            TransitionTimer = 0.75F;
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
            ContentLoader.Load(Content, GraphicsDevice);
            SetStartingPoint(new Point(0, 0));

            World.Load(Start);
            base.LoadContent();
        }

        public void SetCamera(Point TargetCamera)
        {
            CurrentTransitionTimer = TransitionTimer;
            LastCamera = NextCamera;
            NextCamera = new Point(TargetCamera.X * BufferWidth, TargetCamera.Y * BufferHeight);
        }

        public void MoveCamera(float Progress)
        {
            CameraPosition.X = (int)(Progress * LastCamera.X + (1 - Progress) * NextCamera.X);
            CameraPosition.Y = (int)(Progress * LastCamera.Y + (1 - Progress) * NextCamera.Y);
        }

        public void SetStartingPoint(Point InStart)
        {
            Start = InStart;
            CameraPosition.X = InStart.X * BufferWidth;
            CameraPosition.Y = InStart.Y * BufferHeight;
            LastCamera = CameraPosition;
            NextCamera = CameraPosition;
        }

        public void DeathFreeze(float DeathTimer)
        {
            CurrentDeathTimer = DeathTimer;
        }

        protected override void Update(GameTime GameTime)
        {
            float DeltaTime = (float)GameTime.ElapsedGameTime.TotalSeconds;

            // TODO: Check if such update block can be used.
            if (CurrentTransitionTimer > 0)
            {
                CurrentTransitionTimer = Math.Max(CurrentTransitionTimer - DeltaTime, 0);
                MoveCamera(CurrentTransitionTimer / TransitionTimer);
                base.Update(GameTime);
                return;
            }

            if(CurrentDeathTimer > 0)
            {
                CurrentDeathTimer = Math.Max(CurrentDeathTimer - DeltaTime, 0);
                if(CurrentDeathTimer == 0)
                {
                    World.DeathUnfreeze();
                }

                base.Update(GameTime);
                return;
            }

            World.Update(DeltaTime);
            base.Update(GameTime);
        }

        protected override void Draw(GameTime GameTime)
        {
            GraphicsDevice.Clear(BGColor);

            CameraMatrix = Matrix.CreateTranslation(new Vector3((-1) * CameraPosition.X, (-1) * CameraPosition.Y, 0));
            Batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, CameraMatrix * ScaleMatrix);
            World.Render(GameTime, Batch);
            Batch.End();

            base.Draw(GameTime);
        }
    }
}
