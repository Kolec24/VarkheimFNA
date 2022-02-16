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
        public Jump(float InVelocity, float MaxTimer)
        {
            Velocity = InVelocity;
            _MaxTimer = MaxTimer;
        }

        public float MaxTimer()
        {
            return _MaxTimer;
        }

        public bool Jumping = false;
        public float Velocity = 0;
        public float Timer = 0;

        private float _MaxTimer = 0;
    }
}
