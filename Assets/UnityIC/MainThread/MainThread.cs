#define UNITY_IC_MAIN_THREAD

using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityIC
{
    public class MainThread : MonoBehaviour
    {
        private static MainThread m_Instance = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;

            m_Instance = new GameObject(nameof(MainThread)).AddComponent(typeof(MainThread)).GetComponent<MainThread>();

            DontDestroyOnLoad(m_Instance);
        }

        private static void SceneManagerOnSceneUnloaded(Scene arg0)
        {
            Debug.Log("1131");
        }

        private void Awake()
        {
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        private void FixedUpdate()
        {
        }

        private void LateUpdate()
        {
        }
    }

    public enum ExecutionType
    {
        None = 0,
        Awake = 1,
        Start = 2,
        Update = 3,
        FixedUpdate = 4,
        LateUpdate = 5
    }

    [System.Serializable]
    public class ActionMT
    {
        //[SerializeField]
        //private int m_Order = 0;

        //[SerializeField]
        //private ExecutionType m_ExecutionType = default;
    }
}