using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using EntityHandle = System.Object;

namespace ECS
{
    class EntityManager
    {
        public EntityManager() { }

        public EntityHandle AddEntity()
        {
            Tuple<int, List<Tuple<int, int>>> NewEntity = new Tuple<int, List<Tuple<int, int>>>(_Entities.Count, new List<Tuple<int, int>>());
            _Entities.Add(NewEntity);
            return (EntityHandle)NewEntity;
        }

        public void RemoveEntity(EntityHandle Handle)
        {
            List<Tuple<int, int>> EntityComponents = HandleToComponents(Handle);
            for (int Index = 0; Index < EntityComponents.Count; Index++)
            {
                _DestroyComponentInternal(EntityComponents[Index].Item1, EntityComponents[Index].Item2);
            }

            int EntityIndex = HandleToIndex(Handle);
            int LastEntityIndex = _Entities.Count - 1;
            _Entities[EntityIndex] = _Entities[LastEntityIndex];
            _Entities.RemoveAt(LastEntityIndex);
        }

        public Component<T> AddComponent<T>(EntityHandle Entity, Component<T> Component)
        {
            _AddComponentInternal(Entity, Component<T>.Type(), Component);
            return Component;
        }

        private void _AddComponentInternal(EntityHandle Handle, int ComponentId, BaseComponent Component)
        {
            List<Tuple<int, int>> EntityComponents = HandleToComponents(Handle);
            Tuple<int, int> NewTuple;
            Component.Entity = Handle;
            if (_Components.ContainsKey(ComponentId))
            {
                NewTuple = new Tuple<int, int>(ComponentId, _Components[ComponentId].Count);
                _Components[ComponentId].Add(Component);
            }
            else
            {
                NewTuple = new Tuple<int, int>(ComponentId, 0);
                _Components.Add(ComponentId, new List<BaseComponent>() { Component });
            }

            EntityComponents.Add(NewTuple);
        }

        public void RemoveComponent<T>(EntityHandle Entity)
        {
            _RemoveComponentInternal(Entity, Component<T>.Type());
        }

        private void _RemoveComponentInternal(EntityHandle Handle, int ComponentId)
        {
            List<Tuple<int, int>> EntityComponents = HandleToComponents(Handle);
            for (int Index = 0; Index < EntityComponents.Count; Index++)
            {
                if (EntityComponents[Index].Item1 == ComponentId)
                {
                    _DestroyComponentInternal(ComponentId, Index);
                    EntityComponents[Index] = EntityComponents[EntityComponents.Count - 1];
                    EntityComponents.RemoveAt(EntityComponents.Count - 1);
                    break;
                }
            }
        }

        private void _DestroyComponentInternal(int ComponentId, int Index)
        {
            List<BaseComponent> Components = _Components[ComponentId];
            BaseComponent LastComponent = Components[Components.Count - 1];
            if (Index == Components.Count - 1)
            {
                Components.RemoveAt(Index);
            }
            else
            {
                List<Tuple<int, int>> OwnerEntityComponents = HandleToComponents(LastComponent.Entity);
                for (int i = 0; i < OwnerEntityComponents.Count; i++)
                {
                    if (OwnerEntityComponents[i].Item1 == ComponentId && OwnerEntityComponents[i].Item2 == Components.Count - 1)
                    {
                        // Really questionable operation.
                        OwnerEntityComponents[i] = new Tuple<int, int>(ComponentId, Index);
                        break;
                    }
                }
                Components[Index] = LastComponent;
                Components.RemoveAt(Index);
            }
        }

        public T GetComponent<T>(EntityHandle Entity)
        {
            // Better way of casting to be taken under consideration.
            return (T)(Object)_GetComponentInternal(Entity, Component<T>.Type());
        }

        public BaseComponent _GetComponentInternal(EntityHandle Handle, int ComponentId)
        {
            List<Tuple<int, int>> EntityComponents = HandleToComponents(Handle);
            for(int Index = 0; Index < EntityComponents.Count; Index++)
            {
                if(EntityComponents[Index].Item1 == ComponentId)
                {
                    return _Components[ComponentId][EntityComponents[Index].Item2];
                }
            }
            return null;
        }

        public void Clear()
        {

        }

        public void Update()
        {

        }

        public void Render()
        {

        }

        private List<Tuple<int, List<Tuple<int, int>>>> _Entities = new List<Tuple<int, List<Tuple<int, int>>>>();
        private Dictionary<int, List<BaseComponent>> _Components = new Dictionary<int, List<BaseComponent>>();
        private List<BaseSystem> _Systems = new List<BaseSystem>();

        public List<Tuple<int, List<Tuple<int, int>>>> Entities() { return _Entities; }
        public Dictionary<int, List<BaseComponent>> Components() { return _Components; }
        public List<BaseSystem> Systems() { return _Systems; }

        private Tuple<int, List<Tuple<int, int>>> HandleToData(EntityHandle Handle)
        {
            return (Tuple<int, List<Tuple<int, int>>>)Handle;
        }

        private int HandleToIndex(EntityHandle Handle)
        {
            return HandleToData(Handle).Item1;
        }

        private List<Tuple<int, int>> HandleToComponents(EntityHandle Handle)
        {
            return HandleToData(Handle).Item2;
        }
    }
}
