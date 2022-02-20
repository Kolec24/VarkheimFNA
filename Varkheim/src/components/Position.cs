using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ECS;
using Microsoft.Xna.Framework;

namespace Varkheim
{
    class Position : Component<Position>
    {
        public Point Current;
        public Point Last;
        public Vector2 Remainder = Vector2.Zero;

        // TODO: Check if this is the right place.
        public Point Facing = new Point(1, 0);

        public Position(int PosX, int PosY)
        {
            Current.X = PosX;
            Current.Y = PosY;
            Last.X = PosX;
            Last.Y = PosY;
        }
    }
}
