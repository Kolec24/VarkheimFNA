using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;

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
            List<Tuple<int, int>> EntityComponents = _HandleToComponents(Handle);
            for (int Index = 0; Index < EntityComponents.Count; Index++)
            {
                _DestroyComponentInternal(EntityComponents[Index].Item1, EntityComponents[Index].Item2);
            }

            int EntityIndex = _HandleToIndex(Handle);
            int LastEntityIndex = _Entities.Count - 1;
            _Entities[EntityIndex] = new Tuple<int, List<Tuple<int, int>>>(EntityIndex, _Entities[LastEntityIndex].Item2);
            _Entities.RemoveAt(LastEntityIndex);
        }

        public Component<T> AddComponent<T>(EntityHandle Entity, Component<T> Component) where T : BaseComponent
        {
            _AddComponentInternal(Entity, Component<T>.Type(), Component);
            return Component;
        }

        private void _AddComponentInternal(EntityHandle Handle, int ComponentId, BaseComponent Component)
        {
            List<Tuple<int, int>> EntityComponents = _HandleToComponents(Handle);
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

        public void RemoveComponent<T>(EntityHandle Entity) where T : BaseComponent
        {
            _RemoveComponentInternal(Entity, Component<T>.Type());
        }

        private void _RemoveComponentInternal(EntityHandle Handle, int ComponentId)
        {
            List<Tuple<int, int>> EntityComponents = _HandleToComponents(Handle);
            for (int Index = 0; Index < EntityComponents.Count; Index++)
            {
                if (EntityComponents[Index].Item1 == ComponentId)
                {
                    _DestroyComponentInternal(ComponentId, EntityComponents[Index].Item2);
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
                List<Tuple<int, int>> OwnerEntityComponents = _HandleToComponents(LastComponent.Entity);
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

        public T GetComponent<T>(EntityHandle Entity) where T : BaseComponent
        {
            // Better way of casting to be taken under consideration.
            return (T)_GetComponentInternal(Entity, _Components[Component<T>.Type()], Component<T>.Type());
        }

        private BaseComponent _GetComponentInternal(EntityHandle Handle, List<BaseComponent> Components, int ComponentId)
        {
            List<Tuple<int, int>> EntityComponents = _HandleToComponents(Handle);
            for(int Index = 0; Index < EntityComponents.Count; Index++)
            {
                if(EntityComponents[Index].Item1 == ComponentId)
                {
                    return Components[EntityComponents[Index].Item2];
                }
            }
            return null;
        }


        public void AddSystem(BaseSystem System)
        {
            List<BaseSystem> Systems = SystemList(System);
            Systems.Add(System);
        }

        public void RemoveSystem(BaseSystem System)
        {
            List<BaseSystem> Systems = SystemList(System);
            for (int Index = 0; Index < Systems.Count; Index++)
            {
                if(System == Systems[Index])
                {
                    Systems.RemoveAt(Index);
                    return;
                }
            }
        }

        public void UpdateSystems(GameTime GameTime, List<BaseSystem> Systems)
        {
            List<List<BaseComponent>> AllComponents = new List<List<BaseComponent>>();
            List<BaseComponent> TargetComponents = new List<BaseComponent>();
            for (int Index = 0; Index < Systems.Count; Index++)
            {
                List<int> ComponentTypes = Systems[Index].ComponentTypes();
                if (ComponentTypes.Count == 1)
                {
                    List<BaseComponent> Components = _Components[ComponentTypes[0]];
                    for(int i = 0; i < Components.Count; i++)
                    {
                        //TODO: Check if _Systems(..., new List<BaseComponent>{ Components[i] }); is better.
                        TargetComponents.Clear();
                        TargetComponents.Add(Components[i]);
                        Systems[Index].UpdateComponents(GameTime, TargetComponents);
                    }
                }
                else
                {
                    _UpdateMultiComponentSystem(GameTime, Systems, Index, ComponentTypes, TargetComponents, AllComponents);
                }
            }
        }

        private void _UpdateMultiComponentSystem(GameTime GameTime, List<BaseSystem> Systems,
            int Index, List<int> ComponentTypes, List<BaseComponent> TargetComponents,
            List<List<BaseComponent>> AllComponents)
        {
            for(int i = 0; i < ComponentTypes.Count; i++)
            {
                AllComponents.Add(_Components[ComponentTypes[i]]);
            }

            int LeastNumComponents = FindLeastCommonComponent(ComponentTypes);

            for (int i = 0; i < AllComponents[LeastNumComponents].Count; i++)
            {
                TargetComponents.Clear();
                bool EntityValid = true;

                for(int j = 0; j < ComponentTypes.Count; j++)
                {
                    if(j == LeastNumComponents)
                    {
                        TargetComponents.Add(AllComponents[LeastNumComponents][i]);
                    }

                    TargetComponents.Add(_GetComponentInternal(AllComponents[LeastNumComponents][i].Entity, AllComponents[j], ComponentTypes[j]));
                    if(TargetComponents[j] == null)
                    {
                        EntityValid = false;
                        break;
                    }
                }

                if(EntityValid)
                {
                    Systems[Index].UpdateComponents(GameTime, TargetComponents);
                }
            }
        }

        public List<BaseSystem> SystemList(BaseSystem System)
        {
            if (System.Type() == BaseSystem.SystemType.Gameplay)
            {
                return _GameplaySystems;
            }
            else
            {
                return _RenderSystems;
            }
        }

        private int FindLeastCommonComponent(List<int> ComponentTypes)
        {
            int Counter = _Components[ComponentTypes[0]].Count;
            int ResultType = 0;
            for(int Index = 0; Index < ComponentTypes.Count; Index++)
            {
                int Number = _Components[ComponentTypes[Index]].Count;
                if(Number < Counter)
                {
                    Counter = Number;
                    ResultType = Index; //ComponentTypes[Index];
                }
            }
            return ResultType;
        }

        public void Clear()
        {

        }

        public void Update(GameTime GameTime)
        {
            UpdateSystems(GameTime, _GameplaySystems);
        }

        public void Render(GameTime GameTime)
        {
            UpdateSystems(GameTime, _RenderSystems);
        }

        private List<Tuple<int, List<Tuple<int, int>>>> _Entities = new List<Tuple<int, List<Tuple<int, int>>>>();
        private Dictionary<int, List<BaseComponent>> _Components = new Dictionary<int, List<BaseComponent>>();
        private List<BaseSystem> _GameplaySystems = new List<BaseSystem>();
        private List<BaseSystem> _RenderSystems = new List<BaseSystem>();

        public List<Tuple<int, List<Tuple<int, int>>>> Entities() { return _Entities; }
        public Dictionary<int, List<BaseComponent>> Components() { return _Components; }
        public List<BaseSystem> GameplaySystems() { return _GameplaySystems; }
        public List<BaseSystem> RenderSystems() { return _RenderSystems; }

        private Tuple<int, List<Tuple<int, int>>> _HandleToData(EntityHandle Handle)
        {
            return (Tuple<int, List<Tuple<int, int>>>)Handle;
        }

        private int _HandleToIndex(EntityHandle Handle)
        {
            return _HandleToData(Handle).Item1;
        }

        private List<Tuple<int, int>> _HandleToComponents(EntityHandle Handle)
        {
            return _HandleToData(Handle).Item2;
        }
    }
}
