using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Damageable : Component<Damageable>
    {
        // TODO: DeathTimer should equal death animation timer!!!
        public Damageable(int InHealth, float InDeathTimer = 0.3F, bool InStopGame = false)
        {
            Health = InHealth;
            _MaxDeathTimer = InDeathTimer;
            _StopGame = InStopGame;
        }

        public float MaxDeathTimer()
        {
            return _MaxDeathTimer;
        }
        public bool StopGame()
        {
            return _StopGame;
        }

        public int Health = 0;
        public float DeathTimer = 0;
        private float _MaxDeathTimer = 0;
        private bool _StopGame = false;
    }
}
