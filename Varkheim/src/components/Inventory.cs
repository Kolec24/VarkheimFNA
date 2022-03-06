using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Inventory : Component<Inventory>
    {
        public Inventory()
        {
            Items = new Dictionary<int, int>();
        }

        // Item and amount.
        public Dictionary<int, int> Items;
    }
}
