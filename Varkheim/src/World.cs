using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ECS;

using EntityHandle = System.Object;

namespace Varkheim
{
    class World
    {
        public EntityManager Manager;

        public World()
        {

        }

        public void Initialize()
        {
            Manager = new EntityManager();
            _InitializeSystems();
        }

        public void Load()
        {
            _LoadLevel();
        }

        private void _InitializeSystems()
        {
            Manager.AddSystem(new RenderSystem());
            Manager.AddSystem(new MovementSystem());
        }

        private void _LoadLevel()
        {
            Factory.Player(Manager, new Vector2(0, 100));
        }

        public void Update(GameTime GameTime)
        {
            float DeltaTime = (float)GameTime.ElapsedGameTime.TotalSeconds;
            Manager.UpdateSystems(DeltaTime);
        }

        public void Render(GameTime GameTime, SpriteBatch Batch)
        {
            float DeltaTime = (float)GameTime.ElapsedGameTime.TotalSeconds;
            Manager.RenderSystems(DeltaTime, Batch);
        }

        public void Clear()
        {

        }
    }
}
