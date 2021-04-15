using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;

namespace Varkheim
{
    class Velocity : Component<Velocity>
    {
        public float X;
        public float Y;

        public Velocity(float VelX, float VelY)
        {
            X = VelX;
            Y = VelY;
        }
    }
}
