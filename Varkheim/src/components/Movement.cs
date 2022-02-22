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
        public int Direction = 0;
        public bool OnGround = false;
        public int OnWall = 0;

        public bool Teleported = false;

        public Movement(Vector2 NewVelocity)
        {
            Velocity = NewVelocity;
        }

        public Movement()
        {

        }
    }
}
