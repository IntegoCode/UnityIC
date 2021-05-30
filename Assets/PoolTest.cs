using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityIC;

public class PoolTest : MonoBehaviour
{
    [SerializeField]
    private PoolObject m_PoolObject = null;

    [SerializeField]
    private Pool<PoolObject> m_Pool = new Pool<PoolObject>();
    
    private IEnumerator Start()
    {
        m_Pool.InstantiateObjects(m_PoolObject, 2)
            .SetAutoCreation(true);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            m_Pool.GetObject(PoolObjecState.NotActive)?.gameObject.SetActive(true);
        }
    }
}
