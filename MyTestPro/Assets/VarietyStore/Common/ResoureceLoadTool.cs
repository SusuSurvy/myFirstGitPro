using UnityEngine;
using System.Collections.Generic;
using System;

namespace Framework.Common
{
    public sealed class ResourceLoader : ManualSingleton<ResourceLoader>
    {
        //public static ResourceLoader Instance
        //{
        //    get { return Nested.instance; }
        //}

        //private static class Nested
        //{
        //    static Nested() { }
        //    internal static readonly ResourceLoader instance = new ResourceLoader();
        //}

        private Dictionary<ResourceRequest, Delegate> m_mapRequest2Delegate;
        private Dictionary<ResourceRequest, Delegate> m_mapRequest2DelegateToAdd;
        private List<ResourceRequest> m_keysToDelete;


        private ResourceLoader()
        {
            m_mapRequest2DelegateToAdd = new Dictionary<ResourceRequest, Delegate>();
            m_mapRequest2Delegate = new Dictionary<ResourceRequest, Delegate>();
            m_keysToDelete = new List<ResourceRequest>();
        }

        public void Destroy()
        {
            m_mapRequest2Delegate.Clear();
            m_keysToDelete.Clear();
        }

        public void Update(float fDeltaTime)
        {
            
            foreach (var item in m_mapRequest2DelegateToAdd)
            {
                m_mapRequest2Delegate.Add(item.Key, item.Value);
            }          


            if (m_mapRequest2Delegate.Count <= 0)
                return;

            m_mapRequest2DelegateToAdd.Clear();
            m_keysToDelete.Clear();

            foreach (var item in m_mapRequest2Delegate)
            {
                if (item.Key.isDone)
                {
                    item.Value.DynamicInvoke(item.Key.asset);
                    m_keysToDelete.Add(item.Key);
                }
            }

            if (m_keysToDelete.Count > 0)
            {
                foreach (var key in m_keysToDelete)
                {
                    m_mapRequest2Delegate.Remove(key);
                }
            }
        }

        public delegate void OnLoadFinish<T>(T item) where T:UnityEngine.Object;

        public static void LoadResourceAsync<T>(string path, OnLoadFinish<T> onFinish) where T : UnityEngine.Object
        {
            instance.OnLoadResourceAsync<T>(path, onFinish);
        }

        private void OnLoadResourceAsync<T>(string path, OnLoadFinish<T> onFinish) where T : UnityEngine.Object
        {
            var request = Resources.LoadAsync<T>(path);
            m_mapRequest2DelegateToAdd.Add(request, onFinish);
        }

        public static T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            T result = Resources.Load<T>(path);
            return result;
        }
    }
}
