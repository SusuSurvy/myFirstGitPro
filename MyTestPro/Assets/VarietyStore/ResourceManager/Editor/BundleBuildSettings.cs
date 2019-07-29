using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;

namespace Framework.Resource
{
	public class BundleBuildSettings
	{
		[XmlElement]
		public BundleBuildItem[] BuildItems;

		[XmlIgnore]
		public int BundleCount
		{
			get
			{
				return BuildItems != null ? BuildItems.Length : 0;
			}
		}

		public void AddItem()
		{
			BundleBuildItem[] items = new BundleBuildItem[BundleCount + 1];
			for (int i = 0; i < BundleCount; i++)
			{
				items[i] = BuildItems[i];
			}
			items[BundleCount] = new BundleBuildItem();
			BuildItems = items;
		}

		public void DeleteItem(int index)
		{
			if (index >= 0 && index < BundleCount)
			{
				if (BundleCount - 1 > 0)
				{
					BundleBuildItem[] items = new BundleBuildItem[BundleCount - 1];
					for (int i = 0; i < index; i++)
					{
						items[i] = BuildItems[i];
					}
					for (int i = index + 1; i < BundleCount; i++)
					{
						items[i - 1] = BuildItems[i];
					}
					BuildItems = items;
				}
				else
				{
					BuildItems = null;
				}
			}
		}

		public AssetBundleBuild[] GetAssetBundleBuilds()
		{
			if (BuildItems != null && BuildItems.Length > 0)
			{
				AssetBundleBuild[] builds = new AssetBundleBuild[BuildItems.Length];
				for (int i = 0; i < BuildItems.Length; i++)
				{
					builds[i] = BuildItems[i].GetAssetBundleBuild();
				}

				return builds;
			}

			return null;
		}
	}

	public class BundleBuildItem
	{
		[XmlAttribute]
		public string Name;

		[XmlAttribute]
		public string RootPath;

		[XmlElement]
		public string[] IncludeExt;

		[XmlElement]
		public string[] ExcludeExt;

		[XmlElement]
		public string[] ExcludeDir;

		[XmlIgnore]
		private List<string> IncludeExtList
		{
			get
			{
				List<string> list = new List<string>();
				if (IncludeExt != null)
				{
					foreach (var s in IncludeExt)
					{
						list.Add(s);
					}
				}
				return list;
			}
		}

		[XmlIgnore]
		private List<string> ExcludeExtList
		{
			get
			{
				List<string> list = new List<string>();
				if (ExcludeExt != null)
				{
					foreach (var s in ExcludeExt)
					{
						list.Add(s);
					}
				}
				return list;
			}
		}

		[XmlIgnore]
		private List<string> ExcludeDirList
		{
			get
			{
				List<string> list = new List<string>();
				if (ExcludeDir != null)
				{
					foreach (var s in ExcludeDir)
					{
						list.Add(s);
					}
				}
				return list;
			}
		}

		public void DrawIncludeExt(bool isEnglish)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(isEnglish ? "Include File Type (eg: \".unity\")" : "只搜索文件类型（如\".unity\"）");
			if (GUILayout.Button(isEnglish ? "Add" : "添加"))
			{
				AddIncludeExt();
			}
			EditorGUILayout.EndHorizontal();
			if (IncludeExt != null)
			{
				int deleteIndex = -1;
				for (int i = 0; i < IncludeExt.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					IncludeExt[i] = EditorGUILayout.TextField(IncludeExt[i]);
					if (GUILayout.Button(isEnglish ? "Delete" : "删除"))
					{
						deleteIndex = i;
					}
					EditorGUILayout.EndHorizontal();
				}
				if (deleteIndex != -1)
				{
					DeleteIncludeExt(deleteIndex);
					deleteIndex = -1;
				}
			}
		}

		public void DrawExcludeExt(bool isEnglish)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(isEnglish ? "Exclude File Type (If you have set the Include File Type, no need to set this field. \".meta\" file will always be excluded)" : "过滤文件类型（若设置了只搜索文件类型，则该项无效，.meta不需要设置）");
			if (GUILayout.Button(isEnglish ? "Add" : "添加"))
			{
				AddExcludeExt();
			}
			EditorGUILayout.EndHorizontal();
			if (ExcludeExt != null)
			{
				int deleteIndex = -1;
				for (int i = 0; i < ExcludeExt.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					ExcludeExt[i] = EditorGUILayout.TextField(ExcludeExt[i]);
					if (GUILayout.Button(isEnglish ? "Delete" : "删除"))
					{
						deleteIndex = i;
					}
					EditorGUILayout.EndHorizontal();
				}
				if (deleteIndex != -1)
				{
					DeleteExcludeExt(deleteIndex);
					deleteIndex = -1;
				}
			}
		}

		public void DrawExcludeDir(bool isEnglish)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(isEnglish ? "Exclude Directory" : "过滤目录");
			if (GUILayout.Button(isEnglish ? "Add" : "添加"))
			{
				AddExcludeDir();
			}
			EditorGUILayout.EndHorizontal();
			if (ExcludeDir != null)
			{
				int deleteIndex = -1;
				for (int i = 0; i < ExcludeDir.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					ExcludeDir[i] = EditorGUILayout.TextField(ExcludeDir[i]);
					if (GUILayout.Button(isEnglish ? "Browse" : "浏览"))
					{
						string dir = EditorUtility.OpenFolderPanel(isEnglish ? "Choose Exclude Directory" : "选择过滤目录", RootPath, "");
						if (!string.IsNullOrEmpty(dir))
						{
							if (dir.StartsWith(Application.dataPath))
							{
								ExcludeDir[i] = "Assets" + dir.Substring(Application.dataPath.Length);
							}
						}
					}
					if (GUILayout.Button(isEnglish ? "Delete" : "删除"))
					{
						deleteIndex = i;
					}
					EditorGUILayout.EndHorizontal();
				}
				if (deleteIndex != -1)
				{
					DeleteExcludeDir(deleteIndex);
					deleteIndex = -1;
				}
			}
		}

		private void AddIncludeExt()
		{
			List<string> list = IncludeExtList;
			list.Add("");
			IncludeExt = list.ToArray();
		}

		private void DeleteIncludeExt(int index)
		{
			List<string> list = IncludeExtList;
			if (index >= 0 && index < list.Count)
			{
				list.RemoveAt(index);
			}
			IncludeExt = list.ToArray();
		}

		private void AddExcludeExt()
		{
			List<string> list = ExcludeExtList;
			list.Add("");
			ExcludeExt = list.ToArray();
		}

		private void DeleteExcludeExt(int index)
		{
			List<string> list = ExcludeExtList;
			if (index >= 0 && index < list.Count)
			{
				list.RemoveAt(index);
			}
			ExcludeExt = list.ToArray();
		}

		private void AddExcludeDir()
		{
			List<string> list = ExcludeDirList;
			list.Add("");
			ExcludeDir = list.ToArray();
		}

		private void DeleteExcludeDir(int index)
		{
			List<string> list = ExcludeDirList;
			if (index >= 0 && index < list.Count)
			{
				list.RemoveAt(index);
			}
			ExcludeDir = list.ToArray();
		}

		public AssetBundleBuild GetAssetBundleBuild()
		{
			AssetBundleBuild build = new AssetBundleBuild();
			build.assetBundleName = Name;
			List<string> assetNames = new List<string>();

			SearchDirectory(RootPath, assetNames);

			build.assetNames = assetNames.ToArray();

			return build;
		}

		private void SearchDirectory(string path, List<string> list)
		{
			DirectoryInfo folder = new DirectoryInfo(path);
			FileSystemInfo[] files = folder.GetFileSystemInfos();
			int length = files.Length;
			for (int i = 0; i < length; i++)
			{
				if (files[i] is DirectoryInfo)
				{
					if (FilterDir(path + "/" + files[i].Name))
					{
						SearchDirectory(path + "/" + files[i].Name, list);
					}
				}
				else
				{
					if (FilterFile(files[i].Name))
					{
						list.Add(path + "/" + files[i].Name);
					}
				}
			}
		}

		private bool FilterFile(string filename)
		{
			if (filename.EndsWith(".meta"))
			{
				return false;
			}
			if (IncludeExt != null)
			{
				foreach (var s in IncludeExt)
				{
					if (filename.EndsWith(s))
					{
						return true;
					}
				}

				return false;
			}
			else
			{
				if (ExcludeExt != null)
				{
					foreach (var s in ExcludeExt)
					{
						if (filename.EndsWith(s))
						{
							return false;
						}
					}

					return true;
				}
			}

			return true;
		}

		private bool FilterDir(string dir)
		{
			if (ExcludeDir != null)
			{
				foreach (var s in ExcludeDir)
				{
					if (s == dir)
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
