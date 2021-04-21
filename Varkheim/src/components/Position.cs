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
        public Point Pos;
        public Vector2 Remainder = Vector2.Zero; 

        public Position(int PosX, int PosY)
        {
            Pos.X = PosX;
            Pos.Y = PosY;
        }
    }
}
