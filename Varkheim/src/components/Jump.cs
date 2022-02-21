using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

using EntityHandle = System.Object;

namespace Varkheim
{
    class Jump : Component<Jump>
    {
        public Jump(Vector2 InVelocity, float MaxTimer)
        {
            Velocity = InVelocity;
            _MaxTimer = MaxTimer;
        }

        public float MaxTimer()
        {
            return _MaxTimer;
        }

        public void Reset()
        {
            Jumping = false;
            GroundJumping = false;
            WallJumping = false;
            GroundTimer = 0;
            WallTimer = 0;
        }

        public bool Jumping = false;
        public int Direction = 0;
        public Vector2 Velocity;
        public bool GroundJumping = false;
        public bool WallJumping = false;
        public float GroundTimer = 0;
        public float WallTimer = 0;

        private float _MaxTimer = 0;
    }
}
