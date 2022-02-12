using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Player : Component<Player>
    {
        public bool Shooting = false;
        // TODO: Move to shooting component if it exists.
        public Point ShootingOffset = new Point(5, -8);

        public Player()
        {

        }
    }
}
