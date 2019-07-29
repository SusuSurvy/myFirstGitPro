/*
 * TODO: 
 * 1. Remove DynamicInvoke; 
 * 2. Use a pool to store AsyncRequest and LoadFinishCallback
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace Framework.Resource
{
    public class ResourceManager
    {
        public enum LoadMode
        {
            Original = 0,
            AssetBundle,
#if UNITY_EDITOR
			Editor,
#endif
			All,
        }
  

        private BundleResourceLoader bundleResourceLoader = null;
        private LoadMode loadMode = LoadMode.All;
        private string gameId = string.Empty;
        private string resourcePath = string.Empty;

        private Dictionary<AsyncRequest, LoadFinishCallback> onLoadedTables;
        private List<AsyncRequest> keysToDelete;
        private Dictionary<AsyncRequest, LoadFinishCallback> keysToAdd;


        public ResourceManager(string _gameId, LoadMode _loadMode = LoadMode.Original)
        {
            gameId = _gameId;
            resourcePath = String.Format("Assets/{0}/Resources/", gameId);

			if (_loadMode == LoadMode.AssetBundle)
			{
				bundleResourceLoader = new BundleResourceLoader(gameId);
			}

            onLoadedTables = new Dictionary<AsyncRequest, LoadFinishCallback>();
            keysToAdd = new Dictionary<AsyncRequest, LoadFinishCallback>();
            keysToDelete = new List<AsyncRequest>();

            AsyncRequestUpdater.Init(this);

            loadMode = _loadMode;
        }

        public void Destroy()
        {
            if(bundleResourceLoader != null)
            {
                bundleResourceLoader.Destroy();
                bundleResourceLoader = null;
            }

            AsyncRequestUpdater.Destroy();
        }


        public T Load<T>(string asset) where T : UnityEngine.Object
        {
            T result = null;

            if(loadMode == LoadMode.Original)
            {
                result = OriginalResourceLoader.loadResource<T>(resourcePath, asset);
            }
            else if(loadMode == LoadMode.AssetBundle)
            {
                result = bundleResourceLoader.LoadAsset<T>(asset);
            }
#if UNITY_EDITOR
			else if (loadMode == LoadMode.Editor)
			{
				result = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(asset);
			}
#endif

			return result;
        }


        public void LoadAsync<T>(string asset, OnLoaded<T> onLoaded) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(asset))
                onLoaded(null);

            if(loadMode == LoadMode.Original)
            {
                var request = OriginalResourceLoader.loadResourceAsync<T>(resourcePath, asset);
                keysToAdd.Add(new AsyncRequest(request), new LoadFinishCallback(onLoaded, LoadFinishCallback.Type.TypeT));
            }
            else if(loadMode == LoadMode.AssetBundle)
            {
                var request = bundleResourceLoader.LoadAssetAsync<T>(asset);
                keysToAdd.Add(new AsyncRequest(request), new LoadFinishCallback(onLoaded, LoadFinishCallback.Type.TypeT));
            }
#if UNITY_EDITOR
			else if (loadMode == LoadMode.Editor)
			{
				var result = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(asset);
				onLoaded(result);
			}
#endif
		}


		public void LoadScene(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("ResourceManager LoadScene parameter path is empty.");
                return;
            }
             

            if (loadMode == LoadMode.Original)
            {
                OriginalResourceLoader.LoadScene(path, mode);
            }
            else if (loadMode == LoadMode.AssetBundle)
            {
                bundleResourceLoader.LoadScene(path, mode);
            }
#if UNITY_EDITOR
			else if (loadMode == LoadMode.Editor)
			{
				OriginalResourceLoader.LoadScene(path, mode);
			}
#endif
		}


		public void LoadSceneAsync(string path, OnLoaded_Void onLoaded, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (string.IsNullOrEmpty(path))
                onLoaded();

            if(loadMode == LoadMode.Original)
            {
                var request = OriginalResourceLoader.LoadSceneAsync(path, mode);
                keysToAdd.Add(new AsyncRequest(request), new LoadFinishCallback(onLoaded, LoadFinishCallback.Type.TypeVoid));
            }
            else if(loadMode == LoadMode.AssetBundle)
            {
                var request = bundleResourceLoader.LoadSceneAsync(path, mode);
                keysToAdd.Add(new AsyncRequest(request), new LoadFinishCallback(onLoaded, LoadFinishCallback.Type.TypeVoid));
            }
#if UNITY_EDITOR
			else if (loadMode == LoadMode.Editor)
			{
				var request = OriginalResourceLoader.LoadSceneAsync(path, mode);
				keysToAdd.Add(new AsyncRequest(request), new LoadFinishCallback(onLoaded, LoadFinishCallback.Type.TypeVoid));
			}
#endif
		}

		public AssetBundle GetAssetBundle(string assetBundleName)
		{
			AssetBundle result = null;

			if (loadMode == LoadMode.AssetBundle)
			{
				result = bundleResourceLoader.GetAssetBundle(assetBundleName);
			}

			return result;
		}

		public void UpdateAsyncRequest()
        {

            foreach (var item in keysToAdd)
            {
                onLoadedTables.Add(item.Key, item.Value);
            }
            keysToAdd.Clear();


            if (onLoadedTables.Count <= 0)
                return;
            
            keysToDelete.Clear();
            foreach(var item in onLoadedTables)
            {
                if (item.Key.isDone)
                {
                    item.Value.Invoke(item.Key.asset);
                    keysToDelete.Add(item.Key);
                }
            }

            if(keysToDelete.Count > 0)
            {
                foreach(var key in keysToDelete)
                {
                    onLoadedTables.Remove(key);
                }
            }
        }

    }

}