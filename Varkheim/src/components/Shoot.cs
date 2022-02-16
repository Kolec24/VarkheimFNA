using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ECS;

namespace Varkheim
{
    class Shoot : Component<Shoot>
    {
        public Shoot(int Projectile, Point Offset, float InVelocity)
        {
            _Projectile = Projectile;
            Velocity = InVelocity;
            _Offset = Offset;
        }

        public int Projectile()
        {
            return _Projectile;
        }

        public Point Offset()
        {
            return _Offset;
        }

        public bool Shooting = false;
        public float Velocity;
        private int _Projectile;
        private Point _Offset;
    }

    
}
