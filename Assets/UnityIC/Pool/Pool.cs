using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityIC
{
    public enum PoolObjecState
    {
        Active = 0,
        NotActive = 1,
        All = 2,
    }
    
    public class Pool<T> where T : Component 
    {
        private List<T> m_Objects = new List<T>();

        private Predicate<T> m_ActiveObjectCondition = item => item.gameObject.activeInHierarchy;

        private bool m_AutoCreation = false;

        private Action<T> m_ActionCreation = null;

        private T m_OriginalObject = null;

        public Pool() { }

        public Pool(T originalObject, int number = 1, Action<T> action = null)
        {
            InstantiateObjects(originalObject, number, action);
        }

        public Pool<T> InstantiateObjects(T originalObject, int number = 1, Action<T> action = null)
        {
            Instantiate(originalObject, number, action);

            return this;
        }

        public Pool<T> InstantiateObjects(IEnumerable<T> originalObjects, int number = 1, Action<T> action = null)
        {
            List<T> originalObjectList = originalObjects.ToList();

            for (int j = 0; j < originalObjectList.Count; j++)
            {
                Instantiate(originalObjectList[j], number, action);
            }

            return this;
        }

        public Pool<T> SetActiveObjectCondition(Predicate<T> predicate)
        {
            m_ActiveObjectCondition = predicate;

            return this;
        }
        
        public Pool<T> SetAutoCreation(bool value)
        {
            m_AutoCreation = value;

            return this;
        }

        public T GetObject(PoolObjecState state = PoolObjecState.NotActive)
        {
            T poolObject = null;

            switch (state)
            {
                case PoolObjecState.Active:
                    poolObject = m_Objects.FirstOrDefault(item => m_ActiveObjectCondition.Invoke(item));
                    break;
                case PoolObjecState.NotActive:
                    poolObject = m_Objects.FirstOrDefault(item => !m_ActiveObjectCondition.Invoke(item));

                    if (poolObject == null && m_AutoCreation)
                    {
                        Instantiate(m_OriginalObject, 1, m_ActionCreation);
                        poolObject = m_Objects.FirstOrDefault(item => !m_ActiveObjectCondition.Invoke(item));
                    }
                    
                    break;
                case PoolObjecState.All:
                    poolObject = m_Objects.FirstOrDefault();
                    break;
            }

            return poolObject;
        }

        public IEnumerable<T> GetObjects(PoolObjecState state = PoolObjecState.NotActive)
        {
            IEnumerable<T> objects = null;

            switch (state)
            {
                case PoolObjecState.Active:
                    objects = m_Objects.Where(item => m_ActiveObjectCondition.Invoke(item));
                    break;
                case PoolObjecState.NotActive:
                    objects = m_Objects.Where(item => !m_ActiveObjectCondition.Invoke(item));
                    break;
                case PoolObjecState.All:
                    objects = m_Objects;
                    break;
            }

            return objects;
        }

        private void Instantiate(T originalObject, int number = 1, Action<T> action = null)
        {
            (m_OriginalObject, m_ActionCreation) = (originalObject, action);

            for (int i = 0; i < number; i++)
            {
                T newObject = UnityEngine.Object.Instantiate(originalObject);
                action?.Invoke(newObject);
                m_Objects.Add(newObject);
            }
        }
    }
}