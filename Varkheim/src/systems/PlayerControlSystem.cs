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
            AddComponentType<Movement>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Input Input = (Input)Components[0];
            Movement Mover = (Movement)Components[1];

            if(_IsKeyDown(Input, Keys.Right))
            {
                Mover.Direction = 1;
            }
            else if (_IsKeyDown(Input, Keys.Left))
            {
                Mover.Direction = -1;
            }
            else
            {
                Mover.Direction = 0;
            }

            if(_IsKeyPressed(Input, Keys.Z))
            {
                if(Mover.OnGround)
                    Mover.Jumping = true;
            }
            else if(_IsKeyReleased(Input, Keys.Z))
            {
                Mover.JumpTimer = 0;
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
