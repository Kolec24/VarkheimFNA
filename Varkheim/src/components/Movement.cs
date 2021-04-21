using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Movement : Component<Movement>
    {
        public Vector2 Velocity;
        public Point DesiredMovement = new Point(0, 0);

        public Movement(float VelX, float VelY)
        {
            Velocity.X = VelX;
            Velocity.Y = VelY;
        }
    }
}
