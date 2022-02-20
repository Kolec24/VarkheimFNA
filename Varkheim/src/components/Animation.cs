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
        // TODO: Check how depth works in FNA.
        public Animation(Sprite NewSprite, string NewAnimation, float NewDepth = 0)
        {
            Sprite = NewSprite;
            _CurrentAnimation = NewAnimation;
            Depth = NewDepth;
        }

        public void Play(string Animation)
        {
            _CurrentAnimation = Animation;
        }

        public string CurrentAnimation()
        {
            return _CurrentAnimation;
        }

        public Sprite Sprite;
        public int AnimationIndex = 0;
        public int FrameIndex = 0;
        public float FrameCounter = 0;
        public bool InValidState = false;

        private string _CurrentAnimation;
    }

    
}
