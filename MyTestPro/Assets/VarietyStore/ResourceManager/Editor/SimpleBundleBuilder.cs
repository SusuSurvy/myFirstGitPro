using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
using System.IO;

namespace Framework.Resource
{
    public class SimpleBundleBuilder : EditorWindow
    {
		[MenuItem("AssetBundle/Simple Bundle Builder")]
		private static void ShowSimpleBundleBuilder()
		{
			EditorWindow.GetWindow(typeof(SimpleBundleBuilder), true, "Simple Bundle Builder");
		}

		private string m_buildPath = "";
		private BundleBuildTarget m_buildTarget = BundleBuildTarget.Android;

		private bool IsEnglish = false;

		private void OnEnable()
		{
			m_buildPath = Application.streamingAssetsPath;
			IsEnglish = EditorPrefs.GetBool("BundleBuilderToolLanguage", false);
		}

		private void OnGUI()
		{
			if (GUILayout.Button(IsEnglish ? "切换为中文" : "Switch to English"))
			{
				IsEnglish = !IsEnglish;
				EditorPrefs.SetBool("BundleBuilderToolLanguage", IsEnglish);
			}
			EditorGUILayout.Space();

			EditorGUILayout.LabelField(IsEnglish ? "Introduction:" : "说明：");
			EditorGUILayout.LabelField(IsEnglish ? "Build assetbundles by using Unity's assetbundle build settings" : "简单打包AssetBundle");
			EditorGUILayout.LabelField(IsEnglish ? "You need to assign each asset a assetbundle name in Unity Editor before you use this tool" : "使用前需要在Unity中为Asset设置好AssetBundle标签");

			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			m_buildPath = EditorGUILayout.TextField(IsEnglish ? "Bundle Export Path" : "Bundle包导出路径", m_buildPath);
			if (GUILayout.Button(IsEnglish ? "Browse" : "浏览"))
			{
				string dir = EditorUtility.OpenFolderPanel(IsEnglish ? "Choose Export Path" : "选择导出目录", "", "");
				if (!string.IsNullOrEmpty(dir))
				{
					m_buildPath = dir;
				}
			}
			EditorGUILayout.EndHorizontal();
			m_buildTarget = (BundleBuildTarget)EditorGUILayout.EnumPopup(IsEnglish ? "Build Platform" : "导出平台", m_buildTarget);

			EditorGUILayout.Space();

			if (GUILayout.Button(IsEnglish ? "Start Building" : "打包"))
			{
				AssetsBundleBuilder.BuildAssetBundles(m_buildPath, m_buildTarget);
			}
		}
    }
}
