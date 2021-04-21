using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ECS;

namespace Varkheim
{
    class InputSystem : BaseSystem
    {
        public InputSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Input>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Input Input = (Input)Components[0];

            Input.LastState = Input.State;
            Input.State = Keyboard.GetState();
        }
    }
}
