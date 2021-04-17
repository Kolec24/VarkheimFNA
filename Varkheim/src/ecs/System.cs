using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ECS
{
    class BaseSystem
    {
        public enum SystemType
        {
            Gameplay = 0,
            Render
        }

        BaseSystem(List<int> ComponentTypes, SystemType Type)
        {
            _ComponentTypes = ComponentTypes;
            _Type = Type;
        }

        public virtual void UpdateComponents(GameTime GameTime, List<BaseComponent> Components)
        {

        }

        public List<int> ComponentTypes()
        {
            return _ComponentTypes;
        }
        public SystemType Type()
        {
            return _Type;
        }

        private List<int> _ComponentTypes;
        private SystemType _Type;
    }
}
