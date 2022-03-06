using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class Openable : Component<Openable>
    {
        public Openable(int InKey)
        {
            _Key = InKey;
        }

        public int Key()
        {
            return _Key;
        }

        private int _Key;
        // TODO: Check if Amount is needed, for now assumed to be always 1.
    }
}
