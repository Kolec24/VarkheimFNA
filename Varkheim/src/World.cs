using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ECS;

using EntityHandle = System.Object;

namespace Varkheim
{
    class World
    {
        public VGame Game;
        public EntityManager Manager;
        public EntityHandle Player;
        // Temporary unloading.
        public List<EntityHandle> CurrentEntities = new List<EntityHandle>();
        public List<EntityHandle> LastEntities = new List<EntityHandle>();
        public Point CurrentRoom = new Point(0, 0);
        public Point LastRoom = new Point(0, 0);

        public World(VGame InGame)
        {
            Game = InGame;
        }

        public void Initialize()
        {
            Manager = new EntityManager();
            _InitializeSystems();
        }

        public void Load(Point StartingRoom)
        {
            LoadLevel(StartingRoom);
        }
        
        public List<T> GetComponents<T>() where T : BaseComponent
        {
            return Manager.Components()[Component<T>.Type()].Cast<T>().ToList();
        }

        // TODO: Messy.
        public void LoadLevel(Point Room)
        {
            LastRoom = CurrentRoom;
            CurrentRoom = Room;

            // Mark which entities should be removed.
            LastEntities.Clear();
            foreach(EntityHandle Entity in CurrentEntities)
            {
                LastEntities.Add(Entity);
            }
            CurrentEntities.Clear();

            // Not sure if using ContentLoader in the world is OK.
            Parser.Map NewMap = ContentLoader.FindMap(Room.X, Room.Y);
            if (NewMap == null)
            {
                Console.WriteLine("InvalidMap");
                return;
            }

            IList<Parser.Object> GameObjects = NewMap.ObjectGroups["Objects"].Objects.Values;
            foreach(Parser.Object Object in GameObjects)
            {
                if(Object.Name == "Player" && Player == null)
                {
                    int RelativeX = Object.X + (int)(0.5 * Object.Width);
                    int RelativeY = Object.Y + Object.Height;
                    Player = Factory.Player(Manager, Room, new Point(RelativeX + Room.X * Game.BufferWidth, RelativeY + Room.Y * Game.BufferHeight));
                }
            }

            CurrentEntities.Add(Factory.Tilemap(Manager, NewMap, Room));
        }

        public void UnloadPreviousLevel()
        {
            foreach(var Entity in LastEntities)
            {
                Manager.RemoveEntity(Entity);
            }

            LastEntities.Clear();
        }

        public void ChangeRooms(Point NextRoom)
        {
            LoadLevel(NextRoom);
            Game.SetCamera(NextRoom);
        }

        public void Update(float DeltaTime)
        {
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

        private void _InitializeSystems()
        {
            Manager.AddSystem(new InputSystem(this));
            Manager.AddSystem(new PlayerControlSystem(this));
            Manager.AddSystem(new PhysicsSystem(this));
            Manager.AddSystem(new MovementSystem(this));
            Manager.AddSystem(new CollisionSystem(this));
            Manager.AddSystem(new ShootingSystem(this));
            Manager.AddSystem(new AnimationSystem(this));

            Manager.AddSystem(new SpriteSystem(this));
            Manager.AddSystem(new TilemapSystem(this));

            Manager.AddSystem(new CameraSystem(this));
        }
    }
}
