using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using EntityHandle = System.Object;

namespace ECS
{
    class BaseComponent
    {
        private static int _TypeCounter = 0;
        public EntityHandle Entity = null;

        public static int Id<T>()
        {
            return _TypeCounter++;
        }
    }

    class Component<T> : BaseComponent
    {
        private static int _Type = Id<T>();

        public bool Active = true;
        public bool Visible = true;
        public int Depth = 0;

        public static int Type()
        {
            return _Type;
        }
    }
}
