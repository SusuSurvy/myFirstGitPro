using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Framework.Common;


    public class SceneLoader : MonoBehaviour
    {
        void Update()
        {
        }
    }


    public static class SceneHelper
    {
        private static SceneLoader m_levelScript;
        private static Action<bool> m_callback = null;
        private static string m_sceneName = string.Empty;

        static SceneHelper()
        {
            m_levelScript = ObjectEX.CreatGOWithBehaviour<SceneLoader>("LevelHelperObj");
        }

        public static void LoadSceneAsync(string SceneName, Action<bool> callback, LoadSceneMode mode = LoadSceneMode.Single)
        {
            m_levelScript.StartCoroutine(DoLoadScene(SceneName, callback, mode));
        }

        private static IEnumerator DoLoadScene(string SceneName, Action<bool> callback, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (Application.CanStreamedLevelBeLoaded(SceneName))
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName, mode);
                while (!operation.isDone && operation.progress < 1f)
                {
                    yield return new WaitForEndOfFrame();
                }
                if (callback != null)
                {
                    callback(true);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback(false);
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public static void LoadTransformSceneAsync(string SceneName, Action<bool> callback)
        {
            m_callback = callback;
            m_sceneName = SceneName;
            m_levelScript.StartCoroutine(DoLoadScene("TransformLevel", LoadSceneAsyncIntenal));
        }

        private static void LoadSceneAsyncIntenal(bool bLoad)
        {
            m_levelScript.StartCoroutine(DoLoadScene(m_sceneName, m_callback));
        }
    }
