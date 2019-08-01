using UnityEngine;

namespace Lobby3D.Common
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T m_instance = null;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = GameObject.FindObjectOfType(typeof (T)) as T;

                    if (m_instance == null)
                    {
                        Debug.LogWarning("Problem during the find of " + typeof (T).ToString());

                        m_instance = new GameObject("Singleton_" + typeof (T).ToString(), typeof (T)).GetComponent<T>();
                        if (m_instance == null)
                        {
                            Debug.LogError("Problem during the creation of " + typeof (T).ToString());
                        }
                    }
                }

                return m_instance;
            }
        }

        void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this as T;

                m_instance.Init();
            }
        }

        void OnApplicationQuit()
        {
            m_instance.Uninit();
            m_instance = null;
        }

        protected virtual void Init()
        {
        }

        protected virtual void Uninit()
        {
        }
    }
}