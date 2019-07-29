using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.Common
{
    public class EventSystem
    {
        static Delegate[] actions;
        static Delegate[] onesActions;
        int max = 0;
        public void init(int MaxEvent)
        {
            actions = new Delegate[MaxEvent];
            onesActions= new Delegate[MaxEvent];
            max = MaxEvent;
        }

        #region AddListener
        public void AddEventListener(int eventType, Action handler)
        {
            if (CheckCorrectType(eventType, handler))
            {
                actions[eventType] = (Action)actions[eventType] + handler;
            }
        }
        public void AddEventListener<T>(int eventType, Action<T> handler)
        {
            if (CheckCorrectType(eventType, handler))
            {
                actions[eventType] = (Action<T>)actions[eventType] + handler;
            }

        }
        public void AddEventListener<T, T1>(int eventType, Action<T, T1> handler)
        {
            if (CheckCorrectType(eventType, handler))
            {
                actions[eventType] = (Action<T, T1>)actions[eventType] + handler;
            }
        }

        public void AddEventListenerOnes(int eventType, Action handler)
        {
            if(onesActions[eventType] != null)
            {
                throw new Exception("Aready exist the event handle");
            }
            if (CheckCorrectType(eventType, handler))
            {
                onesActions[eventType] = handler;
                actions[eventType] = (Action)actions[eventType] + handler;
            }
        }
        public void AddEventListenerOnes<T>(int eventType, Action<T> handler)
        {
            if (onesActions[eventType] != null)
            {
                throw new Exception("Aready exist the event handle");
            }
            if (CheckCorrectType(eventType, handler))
            {
                onesActions[eventType] = handler;
                actions[eventType] = (Action<T>)actions[eventType] + handler;
            }

        }
        public void AddEventListenerOnes<T, T1>(int eventType, Action<T, T1> handler)
        {
            if (onesActions[eventType] != null)
            {
                throw new Exception("Aready exist the event handle");
            }
            if (CheckCorrectType(eventType, handler))
            {
                onesActions[eventType] = handler;
                actions[eventType] = (Action<T, T1>)actions[eventType] + handler;
            }
        }
        #endregion

        #region RemoveListener
        public void RemoveEventListener(int eventType, Action handler)
        {
            if (CheckCorrectType(eventType, handler))
            {
                if (actions[eventType] != null)
                {
                    actions[eventType] = (Action)actions[eventType] - handler;
                }
            }
        }
        public void RemoveEventListener<T>(int eventType, Action<T> handler)
        {
            if (CheckCorrectType(eventType, handler))
            {
                if (actions[eventType] != null)
                {
                    actions[eventType] = (Action<T>)actions[eventType] - handler;
                }
            }
        }
        public void RemoveEventListener<T, T1>(int eventType, Action<T, T1> handler)
        {
            if (CheckCorrectType(eventType, handler))
            {
                if (actions[eventType] != null)
                {
                    actions[eventType] = (Action<T, T1>)actions[eventType] - handler;
                }
            }
        }

        public void ClearEventListener(EventType eventType)
        {
            actions[(int)eventType] = null;
        }

        public void ClearAllEventListener()
        {
            for(int i=0;i< max; i++)
            {
                onesActions[i] = null;
                actions[i] = null;
            }
        }
        #endregion


        #region Trigger
        public void TriggerEvent(int eventType)
        {
            if (actions[eventType] != null)
            {
                ((Action)actions[eventType])();
                if (onesActions[eventType] != null)
                {
                    actions[eventType] = (Action)actions[eventType] - (Action)onesActions[eventType];
                }
            }


        }
        public void TriggerEvent<T>(int eventType, T arg0)
        {
            if (actions[eventType] != null)
            {
                ((Action<T>)actions[eventType])(arg0);
                if (onesActions[eventType] != null)
                {
                    actions[eventType] = (Action<T>)actions[eventType] - (Action<T>)onesActions[eventType];
                }
            }


        }
        public void TriggerEvent<T, T1>(int eventType, T arg0, T1 arg1)
        {
            if (actions[eventType] != null)
            {
                ((Action<T, T1>)actions[eventType])(arg0, arg1);

                if (onesActions[eventType] != null)
                {
                    actions[eventType] = (Action<T, T1>)actions[eventType] - (Action<T, T1>)onesActions[eventType];
                }
            }


        }
        #endregion
        static bool CheckCorrectType(int eventType, Delegate handler)
        {
            if (handler == null)
            {
                throw new Exception("Must add useful event handle");
            }
            if (actions[eventType] == null)
            {
                return true;
            }
            else if (actions[eventType].GetType() == handler.GetType())
            {
                return true;
            }
            else
            {
                throw new Exception("Event handle Type not correct");
            }
        }
    }
}

