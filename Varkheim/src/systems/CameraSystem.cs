using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class CameraSystem : BaseSystem
    {
        public CameraSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Position>();
            AddComponentType<Player>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Position = (Position)Components[0];
            var Player = (Player)Components[1];

            int Width = World.Game.BufferWidth;
            int Height = World.Game.BufferHeight;
            Point Room = World.CurrentRoom;
            Rectangle Bounds = new Rectangle(Room.X * Width, Room.Y * Height, Width, Height);
            if(Bounds.Contains(Position.Current))
            {
                return;
            }

            int XCoord = Position.Current.X / Width;
            int YCoord = Position.Current.Y / Height;
            if (Position.Current.X < 0)
            {
                XCoord--;
            }
            if (Position.Current.Y < 0)
            {
                YCoord--;
            }

            World.ChangeRooms(new Point(XCoord, YCoord));
        }
    }
}
