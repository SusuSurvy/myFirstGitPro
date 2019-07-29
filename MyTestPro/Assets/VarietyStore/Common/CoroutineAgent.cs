using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Common
{
    public class CoroutineAgent : MonoBehaviour
    {

        private static CoroutineAgent m_instance;
        public static CoroutineAgent Instance
        {
            get
            {
                if (m_instance == null)
                {
                    GameObject obj = new GameObject("CoroutineAgent");
                    m_instance = obj.AddComponent<CoroutineAgent>();
                    GameObject.DontDestroyOnLoad(obj);
                }
                return m_instance;
            }
        }

        public static void Destroy()
        {
            GameObject.Destroy(m_instance.gameObject);
        }
    }
}
