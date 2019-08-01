using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework.Common;

namespace Framework.Common
{
    public class TestGUI : MonoBehaviour
    {
        private Dictionary<string, string> labels;
        private GUIStyle testGuiStyle;
        private bool showTag;
        private bool enableTag;

#if UNITY_EDITOR
        private int lastWidth = 0;
        private int lastHeight = 0;
#endif

        public bool ShowTag
        {
            set { showTag = value; }
        }

        public bool EnableTag
        {
            set { enableTag = value; }
        }

        void Awake()
        {
            labels = new Dictionary<string, string>();
            showTag = true;
            testGuiStyle = new GUIStyle();
            testGuiStyle.normal.background = null; //这是设置背景填充
            testGuiStyle.normal.textColor = Color.white; //设置字体颜色的
            testGuiStyle.fontSize = 40; //字体颜色\
            enableTag = false;
        }

        void OnGUI()
        {
            if (!enableTag)
            {
                return;
            }
#if UNITY_EDITOR
            if (lastWidth != Screen.width || lastHeight != Screen.height)
            {
                lastWidth = Screen.width;
                lastHeight = Screen.height;
                ResolutionAdapter.NeedUpdateScaler = true;
            }
#endif

            //GUI.skin.label.normal.textColor = Color ( 0, 0, 0, 1.0 );

            int tmp = 0;
            foreach (var i in labels)
            {
                string str;
                if (showTag)
                {
                    str = i.Key + " : " + i.Value;
                }
                else
                {
                    str = i.Value;
                }
                GUI.Label(new Rect(10, 10 + 30*tmp, 500, 200), str, testGuiStyle);
                ++tmp;
            }
        }

        public void AddLabel(string tag, string labelContent)
        {
            if (labels.ContainsKey(tag))
            {
                labels[tag] = labelContent;
            }
            else
            {
                labels.Add(tag, labelContent);
            }
        }

        public void DelLabel(string tag)
        {
            if (labels.ContainsKey(tag))
            {
                labels[tag] = null;
                labels.Remove(tag);
            }
        }
    }

    public static class TestScene
    {
        private static TestGUI m_obj;

        static TestScene()
        {
            m_obj = ObjectEX.CreatGOWithBehaviour<TestGUI>("TestScene");
        }

        public static void ShowLabel(string tag, string labelContent)
        {
            m_obj.AddLabel(tag, labelContent);
        }

        public static void DisShowLabel(string tag)
        {
            m_obj.DelLabel(tag);
        }

        public static void SetShowTag(bool isShow)
        {
            m_obj.ShowTag = isShow;
        }

        public static void SetEnable(bool isEnable)
        {
            m_obj.EnableTag = isEnable;
        }
    }
}

