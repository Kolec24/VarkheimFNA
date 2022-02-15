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
    class Spirit : Component<Spirit>
    {
        public Spirit(EntityHandle InOwner, Point InOffset)
        {
            Owner = InOwner;
            Offset = InOffset;
        }

        public EntityHandle Owner;
        public Point Offset;
    }
}
