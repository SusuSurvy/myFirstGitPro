using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby3D.Common
{
    /// <summary>
    /// 简单的双键字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> m_keyDic;
        private readonly Dictionary<TValue, TKey> m_valueDic;

        public int Count
        {
            get { return m_keyDic.Count; }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get { return m_keyDic.Keys; }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get { return m_keyDic.Values; }
        }

        public KeyDictionary()
        {
            m_keyDic = new Dictionary<TKey, TValue>();
            m_valueDic = new Dictionary<TValue, TKey>();
        }

        public virtual bool ContainsKey(TKey key)
        {
            return m_keyDic.ContainsKey(key);
        }

        public virtual bool ContainsValue(TValue value)
        {
            return m_valueDic.ContainsKey(value);
        }

        public virtual void Add(TKey key, TValue value)
        {
            if (ContainsKey(key) || ContainsValue(value))
            {
                throw new System.ArgumentException("Key or value already exists!");
            }

            m_keyDic.Add(key, value);
            m_valueDic.Add(value, key);
        }

        public virtual void EditKey(TKey key, TValue newValue)
        {
            if (!ContainsKey(key))
            {
                throw new System.ArgumentException("Key Non-existent!");
            }
            m_valueDic.Remove(m_keyDic[key]);
            m_keyDic[key] = newValue;
            m_valueDic.Add(newValue, key);
        }

        public virtual void EditValue(TKey newKey, TValue value)
        {
            if (!ContainsValue(value))
            {
                throw new System.ArgumentException("Value Non-existent!");
            }
            m_keyDic.Remove(m_valueDic[value]);
            m_valueDic[value] = newKey;
            m_keyDic.Add(newKey, value);
        }

        public virtual void RemoveKey(TKey key)
        {
            if(!ContainsKey(key))
            {
                throw new System.ArgumentException("Key Non-existent!");
            }
            m_valueDic.Remove(m_keyDic[key]);
            m_keyDic.Remove(key);
        }

        public virtual void RemoveValue(TValue value)
        {
            if(!ContainsValue(value))
            {
                throw new System.ArgumentException("Value Non-existent!");
            }
            m_keyDic.Remove(m_valueDic[value]);
            m_valueDic.Remove(value);
        }

        public virtual TValue GetValue(TKey key)
        {
            if(!ContainsKey(key))
            {
                throw new System.ArgumentException("Key Non-existent!");
            }

            return m_keyDic[key];
        }

        public virtual TKey GetKey(TValue value)
        {
            if(!ContainsValue(value))
            {
                throw new System.ArgumentException("Value Non-existent!");
            }

            return m_valueDic[value];
        }

        public virtual void Clear()
        {
            m_keyDic.Clear();
            m_valueDic.Clear();
        }
    }
}

