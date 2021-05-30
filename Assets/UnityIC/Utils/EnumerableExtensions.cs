using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UnityIC
{
    public static class EnumerableExtensions
    {
        private static Random m_Random = new Random();
        
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            List<T> tempList = source.ToList();

            for (int i = 0; i < tempList.Count; i++)
            {
                int j = m_Random.Next(i, tempList.Count);
                yield return tempList[j];

                tempList[j] = tempList[i];
            }
        }
    }
}