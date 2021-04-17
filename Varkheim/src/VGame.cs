using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

using EntityHandle = System.Object;

namespace Varkheim
{
    public class VGame : Game
    {
        EntityManager Manager = new EntityManager();

        public VGame()
        {
            new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            base.Initialize();
            
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime GameTime)
        {
            base.Update(GameTime);
            Manager.Update(GameTime);
        }

        protected override void Draw(GameTime GameTime)
        {
            base.Draw(GameTime);
            Manager.Render(GameTime);
        }
    }
}
