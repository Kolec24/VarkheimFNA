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
    class Soul : Component<Soul>
    {
        public Soul(EntityHandle InOwner)
        {
            Owner = InOwner;
        }

        public EntityHandle Owner;
    }
}
