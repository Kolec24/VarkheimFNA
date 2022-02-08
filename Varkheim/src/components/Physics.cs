using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Physics : Component<Physics>
    {
        public float MaxGroundSpeed = 0;
        public float MaxAirSpeed = 0;
        public float MaxFallingSpeed = 0;
        public float Gravity = 0;

        public float GroundAccel = 0;
        public float AirAccel = 0;
        public float GroundFriction = 0;
        public float AirFriction = 0;

        public int Direction = 0;

        public bool OnGround = true;
        public bool Jumping = false;
        public float JumpVelocity = 0;
        public float JumpTimer = 0;

        public Physics()
        {

        }
    }
}
