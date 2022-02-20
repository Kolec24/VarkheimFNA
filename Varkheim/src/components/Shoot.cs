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
        public Shoot(int Projectile, float InVelocity, Point HorizontalOffset, Point VerticalOffset)
        {
            _Projectile = Projectile;
            Velocity = InVelocity;
            _HorizontalOffset = HorizontalOffset;
            _VerticalOffset = VerticalOffset;
        }

        public int Projectile()
        {
            return _Projectile;
        }

        public Point HorizontalOffset()
        {
            return _HorizontalOffset;
        }

        public Point VerticalOffset()
        {
            return _VerticalOffset;
        }

        public bool Shooting = false;
        public float Velocity;
        private int _Projectile;
        private Point _HorizontalOffset;
        private Point _VerticalOffset;
    }

    
}
