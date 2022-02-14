using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Varkheim
{
    class Sprite
    {
        public struct Frame
        {
            public Texture2D Texture;
            public float Duration;
        }

        public struct Animation
        {
            public string Name;
            public List<Frame> Frames;

            public float Duration()
            {
                float Duration = 0;
                foreach (Frame Frame in Frames)
                {
                    Duration += Frame.Duration;
                }

                return Duration;
            }
        }

        public string Name;
        public Vector2 Origin;
        public List<Animation> Animations = new List<Animation>();
    }
}
