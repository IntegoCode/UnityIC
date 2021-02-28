using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityIC
{
    public class Pool<T> where T : MonoBehaviour 
    {
        private List<T> m_Objects = new List<T>();

        private Predicate<T> m_ActiveObjectCondition = item => item.gameObject.activeInHierarchy;

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

        public T GetObject(ObjecState state = ObjecState.NotActive)
        {
            T poolObject = null;

            switch (state)
            {
                case ObjecState.Active:
                    poolObject = m_Objects.Where(item => m_ActiveObjectCondition.Invoke(item)).FirstOrDefault();
                    break;
                case ObjecState.NotActive:
                    poolObject = m_Objects.Where(item => !m_ActiveObjectCondition.Invoke(item)).FirstOrDefault();
                    break;
                case ObjecState.All:
                    poolObject = m_Objects.FirstOrDefault();
                    break;
                default:
                    break;
            }

            return poolObject;
        }

        public IEnumerable<T> GetObjects(ObjecState state = ObjecState.NotActive)
        {
            IEnumerable<T> objects = null;

            switch (state)
            {
                case ObjecState.Active:
                    objects = m_Objects.Where(item => m_ActiveObjectCondition.Invoke(item));
                    break;
                case ObjecState.NotActive:
                    objects = m_Objects.Where(item => !m_ActiveObjectCondition.Invoke(item));
                    break;
                case ObjecState.All:
                    objects = m_Objects;
                    break;
                default:
                    break;
            }

            return objects;
        }

        private void Instantiate(T originalObject, int number = 1, Action<T> action = null)
        {
            for (int i = 0; i < number; i++)
            {
                T newObject = UnityEngine.Object.Instantiate(originalObject);
                action?.Invoke(newObject);
                m_Objects.Add(newObject);
            }
        }

        public enum ObjecState
        {
            Active = 0,
            NotActive = 1,
            All = 2,
        }
    }
}