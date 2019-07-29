using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Framework.Common
{
    public delegate void UIToggleDelegate(GameObject go, bool bValue);

    public delegate void UISliderDelegate(float fValue);

    public static class ObjectEX
    {
        static ObjectEX()
        {
        }

        public static T CreatGOWithBehaviour<T>(string goName, bool bHide = true, bool bNotDestroyOnLoad = true)
            where T : MonoBehaviour
        {
            T retMono = null;
            GameObject gameObj = new GameObject(goName);

            if (bHide)
            {
                gameObj.hideFlags = UnityEngine.HideFlags.HideInHierarchy;
            }

            if (bNotDestroyOnLoad)
            {
                GameObject.DontDestroyOnLoad(gameObj);
            }

            retMono = gameObj.AddComponent<T>();
            return retMono;
        }

        public static T AddSingleComponent<T>(GameObject gameObject)
            where T : Component
        {
            T ret = gameObject.GetComponent<T>();
            if (ret == null)
                ret = AddComponent<T>(gameObject);

            return ret;
        }

        public static T AddComponent<T>(GameObject gameObject)
            where T : Component
        {
            return gameObject.AddComponent<T>();
        }

        public static void RemoveAllComponents<T>(GameObject gameObject)
            where T : Component
        {
            if (gameObject != null)
            {
                T[] ret = gameObject.GetComponentsInChildren<T>();
                foreach (T o in ret)
                {
                    Object.DestroyImmediate(o);
                }
            }
        }

        public static T FindComponentInChildren<T>(GameObject go, string name)
            where T : Component
        {
            T[] buffer = go.GetComponentsInChildren<T>(true);

            foreach (T o in buffer)
            {
                if (o != null && o.name == name)
                {
                    return o;
                }
            }

            return null;
        }

        public static T[] FindComponentsInChildren<T>(Component go, string name)
            where T : Component
        {
            T[] buffer = go.GetComponentsInChildren<T>(true);

            List<T> ret = new List<T>();

            foreach (T o in buffer)
            {
                if (o != null && o.name == name)
                {
                    ret.Add(o);
                }
            }

            return ret.ToArray();
        }

        public static GameObject GetGameObjectByName(GameObject objInput, string strFindName)
        {
            GameObject ret = null;
            Transform[] objChildren = objInput.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < objChildren.Length; ++i)
            {
                if ((objChildren[i].name == strFindName))
                {
                    ret = objChildren[i].gameObject;
                    break;
                }
            }

            return ret;
        }

        public static List<GameObject> GetGameObjectsByName(GameObject objInput, string strFindName)
        {
            List<GameObject> goList = new List<GameObject>();
            Transform[] objChildren = objInput.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < objChildren.Length; ++i)
            {
                if (objChildren[i].name.Contains(strFindName))
                {
                    goList.Add(objChildren[i].gameObject);
                }
            }

            return goList;
        }

        public static void AddBtnMsg(GameObject go, VoidDelegate UICallback, UIMSGType eType = UIMSGType.OnClick)
        {
            if (eType == UIMSGType.OnClick)
            {
                Button btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(delegate()
                    {
                        UICallback(btn.gameObject);
                    });
                }
                return;
            }

            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null)
            {
                listener = go.AddComponent<UIEventListener>();
            }

            if (eType == UIMSGType.OnPointDown)
            {
                listener.m_onPointDown = UICallback;
            }
            else if (eType == UIMSGType.OnPointDrag)
            {
                listener.m_onPointDrag = UICallback;
            }
            else if (eType == UIMSGType.OnPointUp)
            {
                listener.m_onPointUp = UICallback;
            }
        }

        public static void AddSiderMsg(GameObject go, UISliderDelegate SliderCallback)
        {
            Slider btn = go.GetComponent<Slider>();
            if (btn != null)
            {
                btn.onValueChanged.AddListener(delegate(float fValue)
                {
                    SliderCallback(fValue);
                });
            }
        }

        public static void AddToggleMsg(GameObject go, UIToggleDelegate toggleCallback)
        {
            Toggle tog = go.GetComponent<Toggle>();
            if (tog != null)
            {
                tog.onValueChanged.AddListener(delegate(bool bValue)
                {
                    toggleCallback(tog.gameObject, bValue);
                });
            }
        }
    }
}