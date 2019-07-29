using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
using System.IO;

namespace Framework.Resource
{
	public enum BundleBuildTarget
	{
		Android,
		iOS,
		Windows,
	}

    public static class AssetsBundleBuilder
    {
		public static void BuildAssetBundles(string buildPath, BundleBuildTarget bundleBuildTarget, BundleBuildSettings settings = null)
		{
			BuildTarget buildTarget = BuildTarget.NoTarget;
			switch (bundleBuildTarget)
			{
				case BundleBuildTarget.Android:
					buildTarget = BuildTarget.Android;
					break;
				case BundleBuildTarget.iOS:
					buildTarget = BuildTarget.iOS;
					break;
				case BundleBuildTarget.Windows:
					buildTarget = BuildTarget.StandaloneWindows;
					break;
			}

			createAssetBundle(buildPath, buildTarget, settings);
			createAssetsInfo(buildPath);
		}

		private static void createAssetBundle(string buildPath, BuildTarget buildTarget, BundleBuildSettings settings)
        {
            Caching.ClearCache();
            ///默认的路径
            string Path = buildPath + "/AssetBundles";

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

			ClearDirectory(Path);

			if (settings != null)
			{
				AssetBundleBuild[] builds = settings.GetAssetBundleBuilds();
				if (builds != null)
				{
					BuildPipeline.BuildAssetBundles(Path, builds, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
				}
			}
			else
			{
				BuildPipeline.BuildAssetBundles(Path, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
			}
        }

		private static void ClearDirectory(string path)
		{
			DirectoryInfo dir = new DirectoryInfo(path);
			FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
			foreach (FileSystemInfo i in fileinfo)
			{
				if (i is DirectoryInfo)            //判断是否文件夹
				{
					DirectoryInfo subdir = new DirectoryInfo(i.FullName);
					subdir.Delete(true);          //删除子目录和文件
				}
				else
				{
					File.Delete(i.FullName);      //删除指定文件
				}
			}
		}

		private static void createAssetsInfo(string buildPath)
        {
            int formatVersion = 1;

			var assetBundlePath = buildPath + "/AssetBundles/AssetBundles";
            var rootPath = buildPath + "/AssetBundles";

            AssetBundleManifest manifest = loadManifest(assetBundlePath);

            if (manifest != null)
            {
                Framework.Common.BufferHelper bufferHelper = new Framework.Common.BufferHelper();
                bufferHelper.WriteData(formatVersion);

                var bundles = manifest.GetAllAssetBundles();
                var bundlesCount = bundles.Length;
                bufferHelper.WriteData(bundlesCount);

                foreach (var bundleName in bundles)
                {
                    Debug.Log("AssetsBundle: " + bundleName);
                    var assetsBundle = AssetBundle.LoadFromFile(rootPath + "/" + bundleName);
                    if (assetsBundle != null)
                    {
                        var assets = assetsBundle.GetAllAssetNames();
                        int assetsCount = assets.Length;

                        var scenes = assetsBundle.GetAllScenePaths();
                        int scenesCount = scenes.Length;

                        bufferHelper.WriteData(bundleName);
                        bufferHelper.WriteData(assetsCount + scenesCount);

                        foreach (var asset in assets)
                        {
                            bufferHelper.WriteData(asset);
                        }

                        foreach (var scene in scenes)
                        {
                            bufferHelper.WriteData(scene);
                        }

                        assetsBundle.Unload(true);
                    }
                }

                // write file.
                var buffer = bufferHelper.ByteBuffer;
                var size = bufferHelper.WriteIndex;

                FileStream fs = new FileStream(rootPath + "/AssetsTable", FileMode.Create);
                System.IO.BinaryWriter writer = new BinaryWriter(fs);
                writer.Write(buffer, 0, size);
                writer.Flush();
                writer.Close();
                fs.Close();
            }
        }

        private static AssetBundleManifest loadManifest(string full_name)
        {
            if (!System.IO.File.Exists(full_name))
                return null;

            AssetBundleManifest manifest = null;
            UnityEngine.AssetBundle mainfest_bundle = UnityEngine.AssetBundle.LoadFromFile(full_name);
            if (mainfest_bundle != null)
            {
                manifest = (AssetBundleManifest)mainfest_bundle.LoadAsset("AssetBundleManifest");
                mainfest_bundle.Unload(false);
            }

            return manifest;
        }

    }
}
