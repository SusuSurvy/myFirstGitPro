using System;
using UnityEngine;

namespace Framework.Common
{
    public abstract class ManualSingleton<T> where T : class
    {
        private static T m_instance = null;

        public static bool Init()
        {
            if(m_instance != null)
            {
               // Debug.LogError("Singleton Init: m_instance != null");
                return false;
            }
            else
            {
                Type type = typeof(T);
                System.Reflection.ConstructorInfo[] constructorInfoArray =
                    type.GetConstructors(System.Reflection.BindingFlags.Instance |
                                         System.Reflection.BindingFlags.NonPublic);

                foreach (System.Reflection.ConstructorInfo constructorInfo in constructorInfoArray)
                {
                    System.Reflection.ParameterInfo[] parameterInfoArray = constructorInfo.GetParameters();
                    if (0 == parameterInfoArray.Length)
                    {
                        m_instance = (T)constructorInfo.Invoke(null);
                        break;
                    }
                }

                return (m_instance != null);
            }
        }


        public static T instance
        {
            get {return m_instance;}
        }

        public static bool Release()
        {
            m_instance = null;
            return true;
        }
    }
}