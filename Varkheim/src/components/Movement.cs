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
        public bool OnGround = false;

        public Movement()
        {

        }
    }
}
