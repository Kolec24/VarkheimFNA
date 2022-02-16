using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class PhysicsSystem : BaseSystem
    {
        public PhysicsSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Movement>();
            AddComponentType<Physics>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Mover = (Movement)Components[0];
            var Physics = (Physics)Components[1];

            _CalculateVelocityX(DeltaTime, Mover, Physics);
            _CalculateVelocityY(DeltaTime, Mover, Physics);
        }

        private void _CalculateVelocityX(float DeltaTime, Movement Mover, Physics Physics)
        {
            // Friction is only horizontal!
            float MaxSpeed;
            float Acceleration;
            float Friction;

            if(Mover.OnGround)
            {
                MaxSpeed = Physics.MaxGroundSpeed;
                Acceleration = Physics.GroundAccel;
                Friction = Physics.GroundFriction;
            }
            else
            {
                MaxSpeed = Physics.MaxAirSpeed;
                Acceleration = Physics.AirAccel;
                Friction = Physics.AirFriction;
            }

            float DesiredVel;
            if (Mover.Velocity.X != 0 && Mover.Direction != Math.Sign(Mover.Velocity.X))
            {
                DesiredVel = Math.Sign(Mover.Velocity.X) * Math.Max((Math.Abs(Mover.Velocity.X) - Friction * DeltaTime), 0);
            }
            else
            {
                DesiredVel = Mover.Velocity.X + Mover.Direction * Acceleration * DeltaTime;
            }

            if (Math.Abs(DesiredVel) > MaxSpeed)
            {
                // TODO: Change to approaching instead of instant
                DesiredVel = Math.Sign(Mover.Velocity.X) * MaxSpeed;
            }

            Mover.Velocity.X = DesiredVel;
        }

        private void _CalculateVelocityY(float DeltaTime, Movement Mover, Physics Physics)
        {
            if (Mover.OnGround)
            {
                Mover.Velocity.Y = 0;
            }
            else
            {
                Mover.Velocity.Y = Math.Min(Mover.Velocity.Y + Physics.Gravity * DeltaTime, Physics.MaxFallingSpeed);
            }
        }
    }
}
