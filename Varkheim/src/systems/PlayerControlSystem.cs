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
            AddComponentType<Position>();
            AddComponentType<Movement>();
            AddComponentType<Jump>();
            AddComponentType<Shoot>();
            AddComponentType<Teleport>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            Input Input = (Input)Components[0];
            Position Position = (Position)Components[1];
            Movement Mover = (Movement)Components[2];
            Jump Jumper = (Jump)Components[3];
            Shoot Shooter = (Shoot)Components[4];
            Teleport Teleporter = (Teleport)Components[5];

            if (_IsKeyDown(Input, Keys.Right))
            {
                Mover.Direction = 1;
                Position.Facing.X = 1;
            }
            else if (_IsKeyDown(Input, Keys.Left))
            {
                Mover.Direction = -1;
                Position.Facing.X = -1;
            }
            else
            {
                Mover.Direction = 0;
            }

            if (_IsKeyDown(Input, Keys.Up))
            {
                Position.Facing.Y = -1;
            }
            else if (_IsKeyDown(Input, Keys.Down))
            {
                Position.Facing.Y = 1;
            }
            else
            {
                Position.Facing.Y = 0;
            }

            if (_IsKeyPressed(Input, Keys.Z))
            {
                Jumper.Jumping = true;
            }
            else if(_IsKeyReleased(Input, Keys.Z))
            {
                Jumper.Reset();
            }

            if (_IsKeyPressed(Input, Keys.X))
            {
                // TODO: Hack!
                var Soul = World.GetFirstValid<Soul>();
                if (Soul != null)
                {
                    Teleporter.Soul = Soul;
                    Teleporter.Teleporting = true;
                }
                else
                {
                    Shooter.Shooting = true;
                }
            }
            else if (_IsKeyReleased(Input, Keys.X))
            {
                Shooter.Shooting = false;
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
