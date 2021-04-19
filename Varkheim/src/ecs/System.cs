using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS
{
    class BaseSystem
    {
        private List<int> _ComponentTypes = new List<int>();
        private SystemType _Type;

        public enum SystemType
        {
            Gameplay = 0,
            Render
        }

        public BaseSystem(SystemType Type)
        {
            _Type = Type;
        }

        public List<int> ComponentTypes()
        {
            return _ComponentTypes;
        }
        public SystemType Type()
        {
            return _Type;
        }

        public virtual void UpdateComponents(float DeltaTime, List<BaseComponent> Components)
        {

        }

        public virtual void RenderComponents(float DeltaTime, List<BaseComponent> Components, SpriteBatch Batch)
        {

        }

        protected void AddComponentType<T>()
        {
            _ComponentTypes.Add(Component<T>.Type());
        }
    }
}
