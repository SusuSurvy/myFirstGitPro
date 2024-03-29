﻿using System;
using UnityEngine;
using System.Collections;

namespace Lobby3D.Common
{
    // 什么类可以是单例, 什么类继承这个，有限制： 全局的，无隶属关系的，生命期是整个游戏的，大家经常用的... 
    public abstract class Singleton<T> where T : class
    {
        private static T m_intance;

        public static T Instance
        {
            get
            {
                if (null == m_intance)
                {
                    m_intance = null;
                    Type type = typeof (T);
                    System.Reflection.ConstructorInfo[] constructorInfoArray =
                        type.GetConstructors(System.Reflection.BindingFlags.Instance |
                                             System.Reflection.BindingFlags.NonPublic);

                    foreach (System.Reflection.ConstructorInfo constructorInfo in constructorInfoArray)
                    {
                        System.Reflection.ParameterInfo[] parameterInfoArray = constructorInfo.GetParameters();
                        if (0 == parameterInfoArray.Length)
                        {
                            m_intance = (T) constructorInfo.Invoke(null);
                            break;
                        }
                    }

                    if (null == m_intance)
                    {
                        throw new NotSupportedException("No NonPublic constructor without 0 parameter");
                    }
                }

                return m_intance;
            }
        }

        protected Singleton()
        {
        }

        public static void Destroy()
        {
            m_intance = null;
        }
    }
}