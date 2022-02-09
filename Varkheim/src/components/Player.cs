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
        // TODO: Remove this.
        public Point CurrentRoom = new Point();

        public Player(Point Room)
        {
            CurrentRoom = Room;
        }
    }
}
