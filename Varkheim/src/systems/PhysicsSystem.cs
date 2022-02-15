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
            AddComponentType<Collision>();
            AddComponentType<Physics>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Mover = (Movement)Components[0];
            var Collider = (Collision)Components[1];
            var Physics = (Physics)Components[2];

            if (Collider.Shape() != Collision.ShapeType.Rect)
            {
                return;
            }

            _CalculateVelocityX(DeltaTime, Mover, Physics);
            _CalculateVelocityY(DeltaTime, Mover, Physics);
        }

        private void _CalculateVelocityX(float DeltaTime, Movement Mover, Physics Physics)
        {
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
            if (Mover.Velocity.X != 0 && Physics.Direction != Math.Sign(Mover.Velocity.X))
            {
                DesiredVel = Math.Sign(Mover.Velocity.X) * Math.Max((Math.Abs(Mover.Velocity.X) - Friction * DeltaTime), 0);
            }
            else
            {
                DesiredVel = Mover.Velocity.X + Physics.Direction * Acceleration * DeltaTime;
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
                Mover.Velocity.Y = Physics.Jumping ? -1 * Physics.JumpVelocity : 0;
                Physics.JumpTimer = Physics.Jumping ? 0.2F : 0;
            }
            else
            {
                Mover.Velocity.Y = Math.Min(Mover.Velocity.Y + Physics.Gravity * DeltaTime, Physics.MaxFallingSpeed);
            }

            if (Physics.JumpTimer > 0)
            {
                Mover.Velocity.Y = -1 * Physics.JumpVelocity;
                Physics.JumpTimer -= DeltaTime;
            }
        }
    }
}
