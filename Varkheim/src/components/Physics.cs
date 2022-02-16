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
        public float WallGravity = 0;

        public float GroundAccel = 0;
        public float AirAccel = 0;
        public float GroundFriction = 0;
        public float AirFriction = 0;


        public Physics()
        {

        }
    }
}
