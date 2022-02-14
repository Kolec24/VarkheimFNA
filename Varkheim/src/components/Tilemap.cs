using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Parser;
using ECS;

namespace Varkheim
{
    class Tilemap : Component<Tilemap>
    {
        public Tilemap(Map NewMap)
        {
            _Map = NewMap;
        }

        public Map Map()
        {
            return _Map;
        }

        private Map _Map;
    }
}
