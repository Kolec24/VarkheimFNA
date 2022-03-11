using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EntityHandle = System.Object;

namespace ECS
{
    class EntityManager
    {
        public EntityManager() { }

        private int _EntityCounter = 0;

        public EntityHandle AddEntity()
        {
            List<(int, int)> NewEntity = new List<(int, int)>();
            _Entities.Add(++_EntityCounter, NewEntity);
            return (EntityHandle)_EntityCounter;
        }

        public void RemoveEntity(EntityHandle Handle)
        {
            List<(int, int)> EntityComponents = _HandleToComponents(Handle);
            for (int Index = EntityComponents.Count - 1; Index >= 0; Index--)
            {
                _DestroyComponentInternal(EntityComponents[Index].Item1, EntityComponents[Index].Item2);
                EntityComponents.RemoveAt(Index);
            }

            int EntityIndex = _HandleToIndex(Handle);
            _Entities.Remove(EntityIndex);
        }

        public Component<T> AddComponent<T>(EntityHandle Entity, Component<T> Component) where T : BaseComponent
        {
            _AddComponentInternal(Entity, Component<T>.Type(), Component);
            return Component;
        }

        private void _AddComponentInternal(EntityHandle Handle, int ComponentId, BaseComponent Component)
        {
            List<(int, int)> EntityComponents = _HandleToComponents(Handle);
            (int, int) NewTuple;
            Component.Entity = Handle;
            if (_Components.ContainsKey(ComponentId))
            {
                NewTuple = (ComponentId, _Components[ComponentId].Count);
                _Components[ComponentId].Add(Component);
            }
            else
            {
                NewTuple = (ComponentId, 0);
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
            List<(int, int)> EntityComponents = _HandleToComponents(Handle);
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
                List<(int, int)> OwnerEntityComponents = _HandleToComponents(LastComponent.Entity);
                for (int i = 0; i < OwnerEntityComponents.Count; i++)
                {
                    if (OwnerEntityComponents[i].Item1 == ComponentId && OwnerEntityComponents[i].Item2 == Components.Count - 1)
                    {
                        // Really questionable operation.
                        OwnerEntityComponents[i] = (ComponentId, Index);
                        break;
                    }
                }
                Components[Index] = LastComponent;
                Components.RemoveAt(Components.Count - 1);
            }
        }

        public T GetComponent<T>(EntityHandle Entity) where T : BaseComponent
        {
            if(!_Components.ContainsKey(Component<T>.Type()))
            {
                return null;
            }

            // Better way of casting to be taken under consideration.
            return (T)_GetComponentInternal(Entity, _Components[Component<T>.Type()], Component<T>.Type());
        }

        private BaseComponent _GetComponentInternal(EntityHandle Handle, List<BaseComponent> Components, int ComponentId)
        {
            List<(int, int)> EntityComponents = _HandleToComponents(Handle);
            for (int Index = 0; Index < EntityComponents.Count; Index++)
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

        public void UpdateSystems(float DeltaTime)
        {
            _UpdateSystemsInternal(DeltaTime, _GameplaySystems);
        }

        public void RenderSystems(float DeltaTime, SpriteBatch Batch)
        {
            _UpdateSystemsInternal(DeltaTime, _RenderSystems, Batch);
        }

        private void _UpdateSystemsInternal(float DeltaTime, List<BaseSystem> Systems, SpriteBatch Batch = null)
        {
            List<List<BaseComponent>> AllComponents = new List<List<BaseComponent>>();
            List<BaseComponent> TargetComponents = new List<BaseComponent>();
            for (int Index = 0; Index < Systems.Count; Index++)
            {
                AllComponents.Clear();
                List<int> ComponentTypes = Systems[Index].ComponentTypes();
                if (ComponentTypes.Count == 1)
                {
                    if(!_Components.ContainsKey(ComponentTypes[0]))
                    {
                        continue;
                    }

                    List<BaseComponent> Components = _Components[ComponentTypes[0]];
                    for(int i = 0; i < Components.Count; i++)
                    {
                        //TODO: Check if _Systems(..., new List<BaseComponent>{ Components[i] }); is better.
                        TargetComponents.Clear();
                        TargetComponents.Add(Components[i]);
                        if (Batch == null)
                        {
                            Systems[Index].UpdateComponents(DeltaTime, TargetComponents);
                        }
                        else
                        {
                            Systems[Index].RenderComponents(DeltaTime, TargetComponents, Batch);
                        }
                    }
                }
                else
                {
                    _UpdateMultiComponentSystem(DeltaTime, Systems, Index, ComponentTypes, TargetComponents, AllComponents, Batch);
                }
            }
        }

        private void _UpdateMultiComponentSystem(float DeltaTime, List<BaseSystem> Systems,
            int Index, List<int> ComponentTypes, List<BaseComponent> TargetComponents,
            List<List<BaseComponent>> AllComponents, SpriteBatch Batch = null)
        {
            for(int i = 0; i < ComponentTypes.Count; i++)
            {
                if (!_Components.ContainsKey(ComponentTypes[i]))
                {
                    return;
                }

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
                        continue;
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
                    if (Batch == null)
                    {
                        Systems[Index].UpdateComponents(DeltaTime, TargetComponents);
                    }
                    else
                    {
                        Systems[Index].RenderComponents(DeltaTime, TargetComponents, Batch);
                    }
                }
            }
        }

        public List<BaseSystem> SystemList(BaseSystem System)
        {
            if (System.Type() == BaseSystem.SystemType.Gameplay)
            {
                return _GameplaySystems;
            }
            else if(System.Type() == BaseSystem.SystemType.Render)
            {
                return _RenderSystems;
            }
            return default(List<BaseSystem>);
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

        // TODO: Check performance!
        private Dictionary<int, List<(int, int)>> _Entities = new Dictionary<int, List<(int, int)>>();
        private Dictionary<int, List<BaseComponent>> _Components = new Dictionary<int, List<BaseComponent>>();
        private List<BaseSystem> _GameplaySystems = new List<BaseSystem>();
        private List<BaseSystem> _RenderSystems = new List<BaseSystem>();

        public Dictionary<int, List<(int, int)>> Entities() { return _Entities;  }
        public Dictionary<int, List<BaseComponent>> Components() { return _Components; }
        public List<BaseSystem> GameplaySystems() { return _GameplaySystems; }
        public List<BaseSystem> RenderSystems() { return _RenderSystems; }

        private int _HandleToData(EntityHandle Handle)
        {
            return (int)Handle;
        }

        private int _HandleToIndex(EntityHandle Handle)
        {
            return _HandleToData(Handle);
        }

        private List<(int, int)> _HandleToComponents(EntityHandle Handle)
        {
            return _Entities[_HandleToData(Handle)];
        }
    }
}
