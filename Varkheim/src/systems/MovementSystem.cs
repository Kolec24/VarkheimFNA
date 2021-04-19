using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;

namespace Varkheim
{
    class MovementSystem : BaseSystem
    {
        public MovementSystem() : base(SystemType.Gameplay)
        {
            AddComponentType<Position>();
            AddComponentType<Velocity>();
        }

        public override void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {
            var PosComp = (Position)Components[0];
            var VelComp = (Velocity)Components[1];

            PosComp.X += VelComp.X * DeltaTime;
            PosComp.Y += VelComp.Y * DeltaTime;
        }
    }
}
