using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ECS;

namespace Varkheim
{
    class Position : Component<Position>
    {
        public float X;
        public float Y;

        public Position(float PosX, float PosY)
        {
            X = PosX;
            Y = PosY;
        }
    }
}
