using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby3D.Module
{

    public class ModuleContainer : IInit,IUpdate, IDestroy
    {

        public bool isPause { get; set; }
        private List<IUpdate> m_UpdateList = new List<IUpdate>();
        private List<IDestroy> m_DestroyList = new List<IDestroy>();

        public virtual void Init()
        {
            
        }
        public virtual void Update(float fTime)
        {
            if (isPause)
            {
                return;
            }
            m_UpdateList.ForEach(v =>
            {
                v.Update(fTime);
            });
        }
        public virtual void Destroy()
        {
            m_DestroyList.ForEach(v =>
            {
                v.Destroy();
            });
            m_UpdateList.Clear();
            m_DestroyList.Clear();
        }

        public virtual bool Register<T>(T module)
        {
            if (module is IInit)
            {
                IInit v = (IInit)module;
                v.Init();
            }
            if (module is IUpdate)
            {
                IUpdate v = (IUpdate)module;
                if (!m_UpdateList.Contains(v))
                {
                    m_UpdateList.Add(v);
                }
            }
            if (module is IDestroy)
            {
                IDestroy v = (IDestroy)module;
                if (!m_DestroyList.Contains(v))
                {
                    m_DestroyList.Add(v);
                }
            }
            return true;
        }

        public virtual bool Remove<T>(T module)
        {
            if (module is IUpdate)
            {
                IUpdate v = (IUpdate)module;
                m_UpdateList.Remove(v);
            }
            if (module is IDestroy)
            {
                IDestroy v = (IDestroy)module;
                m_DestroyList.Remove(v);
            }
            return true;
        }

    }
}
