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
        private VGame _Game;
        public EntityManager Manager;
        public EntityHandle Player;

        public World(VGame Game)
        {
            _Game = Game;
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
        
        public List<T> GetComponents<T>() where T : BaseComponent
        {
            return Manager.Components()[Component<T>.Type()].Cast<T>().ToList();
        }

        private void _InitializeSystems()
        {
            Manager.AddSystem(new InputSystem(this));
            Manager.AddSystem(new PlayerControlSystem(this));
            Manager.AddSystem(new PhysicsSystem(this));

            Manager.AddSystem(new SpriteSystem(this));
            Manager.AddSystem(new TilemapSystem(this));
        }

        private void _LoadLevel()
        {
            Player = Factory.Player(Manager, new Point(160, 208));
            bool[] Cells = new bool[_Game.Columns * _Game.Rows];
            for(int x = 0; x < _Game.Columns; x ++)
            {
                for(int y = _Game.Rows - 10; y < _Game.Rows; y ++)
                {
                    if(y >= _Game.Rows - 7 && x > _Game.Columns - 15)
                        Cells[x + y * _Game.Columns] = true;
                    else if (y >= _Game.Rows - 3)
                        Cells[x + y * _Game.Columns] = true;
                }
            }
            Factory.Tilemap(Manager, _Game.Columns, _Game.Rows, _Game.TileWidth, _Game.TileHeight, new List<bool>(Cells));
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
