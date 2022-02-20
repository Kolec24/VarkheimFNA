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
        public int Health = 0;

        public Damageable(int NewHealth)
        {
            Health = NewHealth;
        }
    }
}
