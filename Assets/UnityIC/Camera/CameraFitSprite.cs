using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityIC
{
    [RequireComponent(typeof(Camera))]
    public class CameraFitSprite : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_Sprite = null;

        [SerializeField]
        private FitType m_FitType = default;

        private Camera m_Camera = null;

        private void Awake()
        {
            m_Camera = GetComponent<Camera>();

            FitSprite();
        }

        private void FitSprite()
        {
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = m_Sprite.bounds.size.x / m_Sprite.bounds.size.y;

            float differenceInSize = targetRatio / screenRatio;

            switch (m_FitType)
            {
                case FitType.Full:
                    if (differenceInSize > 1)
                    {
                        m_Camera.orthographicSize = m_Sprite.bounds.size.y / 2;
                    }
                    else
                    {
                        m_Camera.orthographicSize = (m_Sprite.bounds.size.y / 2) * differenceInSize;
                    }
                    break;
                case FitType.Horizontal:
                    m_Camera.orthographicSize = (m_Sprite.bounds.size.y / 2) * differenceInSize;
                    break;
                case FitType.Vertical:
                    m_Camera.orthographicSize = m_Sprite.bounds.size.y / 2;
                    break;
                default:
                    break;
            }
        }

        private enum FitType
        {
            Full = 0,
            Horizontal = 1,
            Vertical = 2
        }
    }
}