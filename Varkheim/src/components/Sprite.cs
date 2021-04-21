using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Varkheim
{
    class Sprite : Component<Sprite>
    {
        public Texture2D Texture;
        public Vector2 Origin;

        public Sprite(Texture2D Sprite, Vector2 SpriteOrigin)
        {
            Texture = Sprite;
            Origin = SpriteOrigin;
        }
    }
}
