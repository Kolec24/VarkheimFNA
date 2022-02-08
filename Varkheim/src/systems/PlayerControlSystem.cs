using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;
using Microsoft.Xna.Framework.Input;

namespace Varkheim
{
    class PlayerControlSystem : BaseSystem
    {
        public PlayerControlSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Input>();
            AddComponentType<Physics>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Input Input = (Input)Components[0];
            Physics Physics = (Physics)Components[1];

            if(_IsKeyDown(Input, Keys.Right))
            {
                Physics.Direction = 1;
            }
            else if (_IsKeyDown(Input, Keys.Left))
            {
                Physics.Direction = -1;
            }
            else
            {
                Physics.Direction = 0;
            }

            if(_IsKeyPressed(Input, Keys.Z))
            {
                Physics.Jumping = true;
            }
            else if(_IsKeyReleased(Input, Keys.Z))
            {
                Physics.Jumping = false;
                Physics.JumpTimer = 0;
            }
        }

        private bool _IsKeyDown(Input Input, Keys Key)
        {
            return Input.State.IsKeyDown(Key);
        }

        private bool _IsKeyUp(Input Input, Keys Key)
        {
            return Input.State.IsKeyUp(Key);
        }

        private bool _IsKeyPressed(Input Input, Keys Key)
        {
            return Input.State.IsKeyDown(Key) && !Input.LastState.IsKeyDown(Key);
        }

        private bool _IsKeyReleased(Input Input, Keys Key)
        {
            return !Input.State.IsKeyDown(Key) && Input.LastState.IsKeyDown(Key);
        }
    }
}
