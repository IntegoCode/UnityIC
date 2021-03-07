using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityIC;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private Transform m_Target = null;

    [SerializeField]
    private DragNDrop2D m_Drag = null;

    private bool m_InTrigger = false;

    private void OnEnable()
    {
        m_Drag.OnPointerUpEvent.AddListener(Call);
    }

    private void Call(DragNDrop2D arg0)
    {
        if (m_InTrigger)
        {
            m_Drag.TargetPosition.position = m_Target.position;
            m_Drag.MoveToTarget();
            //m_Drag.Interactable = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Box")
        {
            m_InTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Box")
        {
            m_InTrigger = false;
        }
    }
}