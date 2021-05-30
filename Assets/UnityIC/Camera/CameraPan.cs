using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityIC
{
    public class CameraPan : MonoBehaviour
    {
        [SerializeField, Header("Camera")]
        private Camera m_Camera = null;

        [SerializeField, Header("Bounds - World Points")]
        private Vector2 m_LeftBottomCorner = new Vector2(-15, -10);

        [SerializeField]
        private Vector2 m_RightTopCorner = new Vector2(15, 10);

        private bool m_IsTouching = false;

        private Vector2 m_PrevTouchPosition = Vector2.zero;

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            MouseUpdate();
#elif UNITY_ANDROID || UNITY_IOS
            TouchUpdate();
#endif
        }

        private void LateUpdate()
        {
            FitCameraInBounds();
        }

        private void MouseUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_PrevTouchPosition = Input.mousePosition;

                m_IsTouching = true;
            }

            if (Input.GetMouseButton(0) && m_IsTouching)
            {
                Vector2 move = (Vector2) Input.mousePosition - m_PrevTouchPosition;
                m_PrevTouchPosition = Input.mousePosition;

                OnSwipe(move);
            }

            if (Input.GetMouseButtonUp(0) && m_IsTouching)
            {
                m_IsTouching = false;
            }
        }

        private void TouchUpdate()
        {
            int touchCount = Input.touches.Length;

            if (touchCount == 1)
            {
                Touch touch = Input.touches[0];

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                    {
                        m_PrevTouchPosition = touch.position;

                        m_IsTouching = true;

                        break;
                    }
                    case TouchPhase.Moved:
                    {
                        m_PrevTouchPosition = touch.position;

                        if (touch.deltaPosition != Vector2.zero && m_IsTouching)
                        {
                            OnSwipe(touch.deltaPosition);
                        }

                        break;
                    }
                    case TouchPhase.Ended:
                    {
                        m_IsTouching = false;
                        break;
                    }
                }
            }
            else
            {
                if (m_IsTouching)
                {
                    m_IsTouching = false;
                }
            }
        }

        private void OnSwipe(Vector2 deltaPosition)
        {
            m_Camera.transform.position -=
                (m_Camera.ScreenToWorldPoint(deltaPosition) - m_Camera.ScreenToWorldPoint(Vector2.zero));
        }

        private void FitCameraInBounds()
        {
            float halftWidth = transform.position.x - m_Camera.ViewportToWorldPoint(Vector2.zero).x;
            float halfHeight = m_Camera.orthographicSize;

            m_Camera.transform.position = new Vector3(
                Mathf.Clamp(m_Camera.transform.position.x, m_LeftBottomCorner.x + halftWidth,
                    m_RightTopCorner.x - halftWidth),
                Mathf.Clamp(m_Camera.transform.position.y, m_LeftBottomCorner.y + halfHeight,
                    m_RightTopCorner.y - halfHeight),
                m_Camera.transform.position.z);
        }
    }
}