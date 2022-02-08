using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class MovementSystem : BaseSystem
    {
        public MovementSystem(World InWorld) : base(SystemType.Gameplay)
        {
            World = InWorld;

            AddComponentType<Position>();
            AddComponentType<Movement>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var Position = (Position)Components[0];
            var Mover = (Movement)Components[1];

            _Move(Position, Mover, DeltaTime);
        }

        private void _Move(Position Position, Movement Mover, float DeltaTime)
        {
            Position.Last = Position.Current;

            Vector2 FullMove = Position.Remainder + Mover.Velocity * DeltaTime;
            Position.Current.X += (int)FullMove.X;
            Position.Current.Y += (int)FullMove.Y;
            Position.Remainder.X = FullMove.X - (int)FullMove.X;
            Position.Remainder.Y = FullMove.Y - (int)FullMove.Y;
        }
    }
}
