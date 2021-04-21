using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ECS;

namespace Varkheim
{
    class Input : Component<Input>
    {
        public KeyboardState State;
        public KeyboardState LastState;
    }
}
