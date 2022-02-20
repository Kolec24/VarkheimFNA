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
    class Teleport : Component<Teleport>
    {
        public Teleport()
        {

        }

        public bool Teleporting = false;
        public EntityHandle Soul;
    }
}
