using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace UnityIC
{
    [RequireComponent(typeof(EventTrigger))]
    public class DragNDrop2D : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private static DragNDrop2D m_DraggedItem = null;

        private Camera m_Camera = null;

        [SerializeField, Header("Main Settings")]
        private bool m_Interactable = true;

        public bool Interactable
        {
            get => m_Interactable;
            set => m_Interactable = value;
        }

        [SerializeField]
        private bool m_BackToDefaultPosition = true;

        [SerializeField]
        public bool BackToDefaultPosition
        {
            get => m_BackToDefaultPosition;
            set => m_BackToDefaultPosition = value;
        }

        public Vector2 Offset = Vector2.zero;

        [Header("Transforms")]
        public Transform DefaultPosition = null;

        public Transform TargetPosition = null;

        public bool IsDragged { get; private set; } = false;

        [SerializeField, Header("Movement Animation"), Space(10)]
        private MovementAnimation m_MovementAnimation = null;

        private Coroutine m_MovementCoroutine = null;

        [Header("Events"), Space(10)]
        public DragEvent OnBeginDrag = null;

        public DragEvent OnEndDrag = null;

        public DragEvent OnPointerUpEvent = null;

        public static event Action<DragNDrop2D> OnBeginDragStatic = null;

        public static event Action<DragNDrop2D> OnEndDragStatic = null;
        
        public static event Action<DragNDrop2D> OnPointerUpStatic = null;

        private void OnValidate()
        {
            if (!m_Camera)
            {
                m_Camera = Camera.main;
            }
        }

        private void Awake()
        {
            if (!DefaultPosition)
            {
                DefaultPosition = new GameObject($"{gameObject.name} Default Position").transform;
                DefaultPosition.position = transform.position;
            }

            if (!TargetPosition)
            {
                TargetPosition = new GameObject($"{gameObject.name} Target Position").transform;
                TargetPosition.position = transform.position;
            }
        }

        private void Update()
        {
            if (m_DraggedItem == this && IsDragged)
            {
                Vector3 pos = m_Camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3) Offset;
                pos.z = transform.position.z;

                transform.position = pos;
            }
        }

        private IEnumerator Movement(Movements movement)
        {
            float t = 0;
            Vector3 initialPosition = transform.position;
            Vector3 newPosition = default;

            while (t < 1)
            {
                t += Time.deltaTime / m_MovementAnimation.Duration;

                switch (movement)
                {
                    case Movements.ToTouch:
                        newPosition = m_Camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3) Offset;
                        newPosition.z = transform.position.z;
                        transform.position = Vector3.Lerp(initialPosition, newPosition,
                            m_MovementAnimation.Curve.Evaluate(t));
                        break;
                    case Movements.ToDefault:
                        transform.position = Vector3.Lerp(initialPosition, DefaultPosition.position,
                            m_MovementAnimation.Curve.Evaluate(t));
                        break;
                    case Movements.ToTarget:
                        transform.position = Vector3.Lerp(initialPosition, TargetPosition.position,
                            m_MovementAnimation.Curve.Evaluate(t));
                        break;
                }

                yield return null;
            }

            switch (movement)
            {
                case Movements.ToTouch:
                    m_DraggedItem = this;
                    break;
                default:
                    OnEndDrag?.Invoke(this);
                    OnEndDragStatic?.Invoke(this);
                    break;
            }
        }

        private void EndDrag()
        {
            IsDragged = false;

            StopMovementCoroutine();
            m_DraggedItem = null;
        }

        private void StopMovementCoroutine()
        {
            if (m_MovementCoroutine != null)
            {
                StopCoroutine(m_MovementCoroutine);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_Interactable && !m_DraggedItem)
            {
                IsDragged = true;

                OnBeginDrag?.Invoke(this);
                OnBeginDragStatic?.Invoke(this);

                StopMovementCoroutine();

                m_MovementCoroutine = StartCoroutine(Movement(Movements.ToTouch));
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsDragged)
            {
                EndDrag();

                if (m_BackToDefaultPosition)
                {
                    m_MovementCoroutine = StartCoroutine(Movement(Movements.ToDefault));
                }
                
                OnPointerUpEvent?.Invoke(this);
                OnPointerUpStatic?.Invoke(this);
            }
        }

        public void MoveToTarget()
        {
            EndDrag();

            m_MovementCoroutine = StartCoroutine(Movement(Movements.ToTarget));
        }

        [System.Serializable]
        public class DragEvent : UnityEvent<DragNDrop2D>
        {
        }

        [System.Serializable]
        private class MovementAnimation
        {
            public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            public float Duration = 0.5f;
        }

        private enum Movements
        {
            ToTouch = 0,
            ToDefault = 1,
            ToTarget = 2
        }
    }
}