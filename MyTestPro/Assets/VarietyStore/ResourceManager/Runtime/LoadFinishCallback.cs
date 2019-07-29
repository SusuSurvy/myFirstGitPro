using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace Framework.Resource
{
    public delegate void OnLoaded_Void();
    public delegate void OnLoaded<T>(T item) where T : UnityEngine.Object;

    public class LoadFinishCallback
    {
        public enum Type
        {
            TypeVoid = 0,
            TypeT,

            None,
        }

        private Type type;

        private Delegate callback;

        public LoadFinishCallback(Delegate _callback, Type _type)
        {
            callback = _callback;
            type = _type;
        }

        public void Invoke(UnityEngine.Object obj)
        {
            if (type == Type.TypeVoid)
                callback.DynamicInvoke();
            else if (type == Type.TypeT)
                callback.DynamicInvoke(obj);
        }
    }
}