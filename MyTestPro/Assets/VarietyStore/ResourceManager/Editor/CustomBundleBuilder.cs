using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;

namespace Framework.Resource
{
	public class CustomBundleBuilder : EditorWindow
	{
		[MenuItem("AssetBundle/Custom Bundle Builder")]
		private static void ShowCustomBundleBuilder()
		{
			EditorWindow.GetWindow(typeof(CustomBundleBuilder), true, "Custom Bundle Builder");
		}

		private BundleBuildSettings m_settings;
		private Vector2 m_scrollPos = Vector2.zero;
		private string m_buildPath = "";
		private BundleBuildTarget m_buildTarget = BundleBuildTarget.Android;

		private bool IsEnglish = false;

		private void OnEnable()
		{
			LoadConfig();
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
			EditorGUILayout.LabelField(IsEnglish ? "Build assetbundles by using custom assetbundle build settings" : "自定义打包AssetBundle");
			EditorGUILayout.LabelField(IsEnglish ? "Custom assetbundle build settings allows you choose a directory, and build files in that directory into assetbundle." : "不使用Unity为Asset设置的AssetBundle标签");
			EditorGUILayout.LabelField(IsEnglish ? "Custom bundle builder will ignore the assetbundle settings you set in Unity Editor" : "采用遍历文件夹的方式决定将哪些Asset打到哪个bundle里");

			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(IsEnglish ? "Bundle Count" : "Bundle数量", m_settings.BundleCount.ToString());
			if (GUILayout.Button(IsEnglish ? "Add Bundle" : "添加"))
			{
				m_settings.AddItem();
			}
			EditorGUILayout.EndHorizontal();

			m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, GUILayout.Height(400));
			if (m_settings.BuildItems != null)
			{
				int deleteIndex = -1;
				for (int i = 0; i < m_settings.BuildItems.Length; i++)
				{
					var item = m_settings.BuildItems[i];
					EditorGUILayout.BeginHorizontal("box");
					EditorGUILayout.BeginVertical();
					item.Name = EditorGUILayout.TextField("BundleName", item.Name);
					EditorGUILayout.BeginHorizontal();
					item.RootPath = EditorGUILayout.TextField("RootPath", item.RootPath);
					if (GUILayout.Button(IsEnglish ? "Browse" : "浏览"))
					{
						string dir = EditorUtility.OpenFolderPanel(IsEnglish ? "Choose Bundle Resource Root Path" : "选择Bundle资源根目录", Application.dataPath, "");
						if (!string.IsNullOrEmpty(dir))
						{
							if (dir.StartsWith(Application.dataPath))
							{
								item.RootPath = "Assets" + dir.Substring(Application.dataPath.Length);
							}
						}
					}
					EditorGUILayout.EndHorizontal();
					item.DrawIncludeExt(IsEnglish);
					EditorGUILayout.Space();
					item.DrawExcludeExt(IsEnglish);
					EditorGUILayout.Space();
					item.DrawExcludeDir(IsEnglish);
					EditorGUILayout.Space();
					EditorGUILayout.EndVertical();
					if (GUILayout.Button(IsEnglish ? "Delete" : "删除"))
					{
						deleteIndex = i;
					}
					EditorGUILayout.EndHorizontal();
				}
				if (deleteIndex != -1)
				{
					m_settings.DeleteItem(deleteIndex);
					deleteIndex = -1;
				}
			}
			EditorGUILayout.EndScrollView();

			EditorGUILayout.Space();

			if (GUILayout.Button(IsEnglish ? "Save Build Settings" : "保存配置"))
			{
				SaveConfig();
			}

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
				AssetsBundleBuilder.BuildAssetBundles(m_buildPath, m_buildTarget, m_settings);
			}
		}

		private void LoadConfig()
		{
			m_settings = new BundleBuildSettings();

			string fullpath = "BundleBuildSettings.xml";
			if (File.Exists(fullpath))
			{
				string content = File.ReadAllText(fullpath);
				XmlReaderSettings readerSettings = new XmlReaderSettings();
				readerSettings.CheckCharacters = false;
				readerSettings.CloseInput = true;

				using (var cr = XmlReader.Create(new StringReader(content), readerSettings))
				{
					XmlSerializer serialize = new XmlSerializer(typeof(BundleBuildSettings));
					m_settings = serialize.Deserialize(cr) as BundleBuildSettings;
				}
			}
		}

		private void SaveConfig()
		{
			string filepath = "BundleBuildSettings.xml";
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
			{
				Encoding = new System.Text.UTF8Encoding(false),
				ConformanceLevel = ConformanceLevel.Auto,
				Indent = true,
				IndentChars = "\t",
				NewLineChars = "\r\n",
				NewLineOnAttributes = false,
			};

			using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				XmlWriter xw = XmlWriter.Create(fs, xmlWriterSettings);
				XmlSerializer serialize = new XmlSerializer(typeof(BundleBuildSettings));

				serialize.Serialize(xw, m_settings);
			}
		}
	}
}
