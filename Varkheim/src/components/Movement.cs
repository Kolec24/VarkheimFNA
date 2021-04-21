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
        public float MaxGroundSpeed = 0;
        public float MaxAirSpeed = 0;
        public float MaxFallingSpeed = 0;
        public float GroundAccel = 0;
        public float AirAccel = 0;
        public float Gravity = 0;
        public float GroundFriction = 0;
        public float AirFriction = 0;

        public float JumpTimer = 0;

        public int Direction = 0;

        public bool OnGround = true;
        public bool Jumping = false;

        public Vector2 Velocity;
        public Point DesiredMovement = new Point(0, 0);

        public Movement()
        {

        }
    }
}
