using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Framework.Resource
{
    public class BundleResourceLoader
    {
        private string assetRootPath = string.Empty;
        private Dictionary<string, string> assetsTable = null;
        private Dictionary<string, AssetBundle> assetBundleCache;
        private AssetBundleManifest manifest;

        public BundleResourceLoader(string _gameId)
        {
			assetRootPath = Application.persistentDataPath + "/" + _gameId + "/AssetBundles";
			string assetsTablePath = assetRootPath + "/AssetsTable";
			if (!File.Exists(assetsTablePath))
			{
				if (Application.platform == RuntimePlatform.Android)
				{
					assetRootPath = Application.dataPath + "!assets/" + _gameId + "/AssetBundles";
				}
				else
				{
					assetRootPath = Application.streamingAssetsPath + "/" + _gameId + "/AssetBundles";
				}
				assetsTablePath = Application.streamingAssetsPath + "/" + _gameId + "/AssetBundles/AssetsTable";
			}

            assetBundleCache = new Dictionary<string, AssetBundle>();

            initAssetsTable(assetsTablePath);
            initManifest(assetRootPath);
        }

        public void Destroy()
        {
            var itr = assetBundleCache.Values.GetEnumerator();
            while (itr.MoveNext())
            {
                itr.Current.Unload(false); // TODO.
            }
            itr.Dispose();

            assetBundleCache.Clear();
        }

        public T LoadAsset<T>(string asset) where T : Object
        {
            try
            {
                T result = null;

                asset = asset.ToLower();

                string assetBundleName = findAssetBundleNameByAsset(asset);
                if (string.IsNullOrEmpty(assetBundleName) == false)
                {
                    AssetBundle ab = loadAssetBundleAndDependencies(assetBundleName);
                    if (ab != null)
                    {
                        result = ab.LoadAsset<T>(asset);
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AssetBundleManager.LoadAsset is falid!\n" + ex.Message);
            }

            return null;
        }

        public AssetBundleRequest LoadAssetAsync<T>(string asset) where T : Object
        {
            try
            {
                asset = asset.ToLower();

                AssetBundleRequest result = null;
                string assetBundleName = findAssetBundleNameByAsset(asset);
                if(string.IsNullOrEmpty(assetBundleName) == false)
                {
                    AssetBundle ab = loadAssetBundleAndDependencies(assetBundleName);
                    if(ab != null)
                    {
                        result = ab.LoadAssetAsync<T>(asset);
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("LoadSceneAsync.LoadAssetAsync is falid!\n" + ex.Message);
            }

            return null;
        }


        public void LoadScene(string scene, LoadSceneMode mode)
        {
            try
            {
                string assetBundleName = findAssetBundleNameByAsset(scene);
                if (string.IsNullOrEmpty(assetBundleName) == false)
                {
                    AssetBundle ab = loadAssetBundleAndDependencies(assetBundleName);
                    if (ab == null)
                    {
                        Debug.LogWarning("BundleResourceLoader.LoadSceneAsyncoadScene() - Can't Load AssetBundle(" + assetBundleName + ")");
                    }

                    scene = Utils.GetPathWithoutExtension(scene);

                    SceneManager.LoadScene(scene, mode);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("BundleResourceLoader.LoadAsset is falid!\n" + ex.Message);
            }
        }


        public AsyncOperation LoadSceneAsync(string scene, LoadSceneMode mode)
        {
            try
            {
                string assetBundleName = findAssetBundleNameByAsset(scene);
                if(string.IsNullOrEmpty(assetBundleName) == false)
                {
                    AssetBundle ab = loadAssetBundleAndDependencies(assetBundleName);
                    if(ab == null)
                    {
                        Debug.LogWarning("BundleResourceLoader.LoadSceneAsyncoadScene() - Can't Load AssetBundle(" + assetBundleName + ")");
                        return null;
                    }

                    scene = Utils.GetPathWithoutExtension(scene);

                    AsyncOperation result = SceneManager.LoadSceneAsync(scene, mode);
                    return result;
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogError("BundleResourceLoader.LoadSceneAsync is falid!\n" + ex.Message);
            }

            return null;
        } 

		public AssetBundle GetAssetBundle(string assetBundleName)
		{
			return loadAssetBundleAndDependencies(assetBundleName);
		}

        private AssetBundle loadAssetBundleAndDependencies(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetBundleName))
                return null;

            if (manifest == null)
                return null;

            string[] deps = manifest.GetAllDependencies(assetBundleName);
            for(int i = 0; i < deps.Length; ++i)
            {
                if(loadAssetBundle(deps[i]) == null)
                {
                    Debug.LogWarning(assetBundleName + "'s Dependencie AssetBundle can't find. Name is " + deps[i] + "!");
                    return null;
                }
            }

            return loadAssetBundle(assetBundleName);
        }

        private AssetBundle loadAssetBundle(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetBundleName))
                return null;

            if (manifest == null)
                return null;

            AssetBundle ab = null;
            if (assetBundleCache.ContainsKey(assetBundleName))
            {
                ab = assetBundleCache[assetBundleName];
            }
            else
            {
                string assetbundlePath = assetRootPath + "/" + assetBundleName;

				ab = AssetBundle.LoadFromFile(assetbundlePath);
                assetBundleCache.Add(assetBundleName, ab);
            }

            return ab;
        }

        private string findAssetBundleNameByAsset(string asset)
        {
            if (assetsTable == null)
                return string.Empty;

            if (!assetsTable.ContainsKey(asset))
                return string.Empty;

            return assetsTable[asset];
        }

        private bool initAssetsTable(string _assetsPath)
        {
            assetsTable = new Dictionary<string, string>();

			byte[] buffer = null;
			if (_assetsPath.Contains("://"))   // Android APK
			{
				WWW www = new WWW(_assetsPath);
				while (!www.isDone)
				{
				}
				buffer = www.bytes;
			}
			else
			{
				buffer = System.IO.File.ReadAllBytes(_assetsPath);
			}

			if (buffer == null || buffer.Length < 8)
			{
				return false;
			}

            var bufferhelper = new Framework.Common.BufferHelper(buffer);

            var versionNum = bufferhelper.ReadInt32();
            var bundleCount = bufferhelper.ReadInt32();

            for (int i = 0; i < bundleCount; ++i)
            {
                var bundleName = bufferhelper.ReadStr();
                var assetsCount = bufferhelper.ReadInt32();

                for (int j = 0; j < assetsCount; ++j)
                {
                    var asset = bufferhelper.ReadStr();
                    assetsTable.Add(asset, bundleName);
                }
            }

            return true;
        }

        private bool initManifest(string assetPath)
        {
            string path = assetPath + "/AssetBundles";

            UnityEngine.AssetBundle mainfest_bundle = UnityEngine.AssetBundle.LoadFromFile(path);
            if (mainfest_bundle != null)
            {
                manifest = (AssetBundleManifest)mainfest_bundle.LoadAsset("AssetBundleManifest");
                mainfest_bundle.Unload(false);
            }

            return (manifest != null);
        }
    }

}