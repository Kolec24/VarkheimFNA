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
    class Shooter : Component<Shooter>
    {
        public Shooter(int Projectile, Point Offset)
        {
            _Projectile = Projectile;
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

        private int _Projectile;
        private Point _Offset;
    }

    
}
