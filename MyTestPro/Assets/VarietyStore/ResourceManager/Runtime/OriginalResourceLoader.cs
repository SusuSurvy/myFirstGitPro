using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace Framework.Resource
{
    public static class OriginalResourceLoader
    {

        public static T loadResource<T>(string resourcePath, string asset) where T : UnityEngine.Object
        {
            asset = Utils.GetPathWithoutExtension(asset);
            asset = Utils.AbsoluteToRelativePath(resourcePath, asset);

            T a = Resources.Load<T>(asset);
            return a;
        }

        public static ResourceRequest loadResourceAsync<T>(string resourcePath, string asset) where T : UnityEngine.Object
        {
            asset = Utils.GetPathWithoutExtension(asset);
            asset = Utils.AbsoluteToRelativePath(resourcePath, asset);

            var request = Resources.LoadAsync<T>(asset);
            return request;
        }


        public static void LoadScene(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            path = Utils.GetPathWithoutExtension(path);
            path = Utils.AbsoluteToRelativePath("Assets", path);
            SceneManager.LoadScene(path, mode);
        }

        public static AsyncOperation LoadSceneAsync(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            path = Utils.GetPathWithoutExtension(path);
            path = Utils.AbsoluteToRelativePath("Assets", path);
            var request = SceneManager.LoadSceneAsync(path, mode);
            return request;
        }
    }
}