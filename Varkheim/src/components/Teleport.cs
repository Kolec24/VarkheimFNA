using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Teleport : Component<Teleport>
    {
        public Teleport()
        {

        }

        public Point TargetPosition;
        public bool Teleporting = false;
    }
}
