using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ECS;

namespace Varkheim
{
    class Animation : Component<Animation>
    {
        public Animation(Sprite NewSprite, string NewAnimation)
        {
            Sprite = NewSprite;
            CurrentAnimation = NewAnimation;
        }

        public Sprite Sprite;
        public string CurrentAnimation;
        public int AnimationIndex = 0;
        public int FrameIndex = 0;
        public float FrameCounter = 0;
    }

    
}
