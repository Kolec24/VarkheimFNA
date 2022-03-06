using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Collectible : Component<Collectible>
    {
        public Collectible(int InItem)
        {
            _Item = InItem;
        }

        public int Item()
        {
            return _Item;
        }

        private int _Item;
        // TODO: Check if Amount is needed, for now assumed to be always 1.
    }
}
