using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Only for world - to reconsider how to remove it from here.
using Varkheim;

namespace ECS
{
    class BaseSystem
    {
        private List<int> _ComponentTypes = new List<int>();
        private SystemType _Type = SystemType.None;
        protected World World;

        public enum SystemType
        {
            None = 0,
            Gameplay,
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
