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
    class UnloadSystem : BaseSystem
    {
        public UnloadSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Position>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Position Position = (Position)Components[0];

            int Width = World.Game.BufferWidth;
            int Height = World.Game.BufferHeight;
            Point Room = World.CurrentRoom;
            Rectangle Bounds = new Rectangle(Room.X * Width, Room.Y * Height, Width, Height);
            if (!Bounds.Contains(Position.Current) && !World.IsChangingRooms())
            {
                World.RemoveEntity(Position.Entity);
            }
        }
    }
}
