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
        public EntityHandle FreezingEntity;
        // State of the game.
        private List<EntityHandle> _CurrentEntities = new List<EntityHandle>();
        private List<EntityHandle> _ValidEntities = new List<EntityHandle>();
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

        public void LoadLevel(Point Room)
        {
            LastRoom = CurrentRoom;
            CurrentRoom = Room;

            _CurrentEntities.Clear();
            _ValidEntities.Clear();

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
                    Player = Factory.Player(Manager, new Point(RelativeX + Room.X * Game.BufferWidth, RelativeY + Room.Y * Game.BufferHeight));
                }
            }

            AddEntity(Factory.Tilemap(Manager, NewMap, new Point(Room.X * Game.BufferWidth, Room.Y * Game.BufferHeight)));
        }

        public void UnloadCurrentLevel()
        {
            foreach (var Entity in _CurrentEntities)
            {
                Manager.RemoveEntity(Entity);
            }
            _CurrentEntities.Clear();
            _ValidEntities.Clear();
        }

        public void ReloadLevel()
        {
            UnloadCurrentLevel();
            LoadLevel(CurrentRoom);
        }

        public void ChangeRooms(Point NextRoom)
        {
            LoadLevel(NextRoom);
            Game.SetCamera(NextRoom);
        }

        public bool IsChangingRooms()
        {
            return Game.CurrentTransitionTimer > 0;
        }

        public void DeathFreeze(EntityHandle Entity, float DeathTimer)
        {
            FreezingEntity = Entity;
            Game.DeathFreeze(DeathTimer);
        }

        public void DeathUnfreeze()
        {
            RemoveEntity(FreezingEntity);
            FreezingEntity = null;
        }

        public List<T> GetComponents<T>() where T : BaseComponent
        {
            if (Manager.Components().ContainsKey(Component<T>.Type()))
            {
                return Manager.Components()[Component<T>.Type()].Cast<T>().ToList();
            }

            return new List<T>();
        }

        public void AddEntity(EntityHandle Entity)
        {
            _CurrentEntities.Add(Entity);
            _ValidEntities.Add(Entity);
        }

        public void RemoveEntity(EntityHandle Entity)
        {
            Manager.RemoveEntity(Entity);
            _CurrentEntities.Remove(Entity);
            _ValidEntities.Remove(Entity);
            if (Entity == Player)
            {
                Player = null;
                ReloadLevel();
            }
        }

        public void SetInvalid(EntityHandle Entity)
        {
            _ValidEntities.Remove(Entity);
        }

        public bool IsEntityValid(EntityHandle Entity)
        {
            return _ValidEntities.Contains(Entity);
        }

        public EntityHandle GetFirstValid<T>() where T : BaseComponent
        {
            var Components = GetComponents<T>();
            foreach (var Component in Components)
            {
                if(_ValidEntities.Contains(Component.Entity))
                {
                    return Component.Entity;
                }
            }
            return null;
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
            // Gameplay.
            Manager.AddSystem(new InputSystem(this));
            Manager.AddSystem(new PlayerControlSystem(this)); 
            Manager.AddSystem(new PlayerAnimationSystem(this));
            Manager.AddSystem(new PhysicsSystem(this));
            Manager.AddSystem(new JumpSystem(this));
            Manager.AddSystem(new MovementSystem(this));
            Manager.AddSystem(new CollisionSystem(this));
            Manager.AddSystem(new ShootingSystem(this));
            Manager.AddSystem(new SoulSystem(this));
            Manager.AddSystem(new TeleportSystem(this));
            Manager.AddSystem(new DamageSystem(this));
            Manager.AddSystem(new DeathSystem(this));
            Manager.AddSystem(new AnimationSystem(this));

            // Render.
            Manager.AddSystem(new SpriteSystem(this));
            Manager.AddSystem(new TilemapSystem(this));

            // Utility.
            Manager.AddSystem(new CameraSystem(this));
            Manager.AddSystem(new UnloadSystem(this));
        }
    }
}
