using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;
using Microsoft.Xna.Framework.Graphics;

namespace Varkheim
{
    class Sprite : Component<Sprite>
    {
        public Texture2D Texture;

        public Sprite(Texture2D Sprite, int DepthVal)
        {
            Texture = Sprite;
            Depth = DepthVal;
        }
    }
}
