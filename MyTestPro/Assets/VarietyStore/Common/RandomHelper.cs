using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhackMole;

namespace Framework.Common
{
    public class RandomHelper
    {

        private static Dictionary<string, System.Random> m_randomDict = new Dictionary<string, System.Random>();
        private static Dictionary<string, int> m_timesDict = new Dictionary<string, int>();
        private static int m_seed = 0;

        private static string m_logKey = "";
        /// <summary>
        /// [min,max)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Range(int min, int max, string key = "defaut")
        {
            System.Random rand;
            if (!m_randomDict.TryGetValue(key, out rand))
            {
                rand = new System.Random(m_seed);
                m_randomDict[key] = rand;
                m_timesDict[key] = 0;
            }

            int result = rand.Next(min, max);

            if (key == m_logKey)
            {
                Debug.Log(key + "    RandomHelper times:" + m_timesDict[key] + "   result = " + result);
                m_timesDict[key]++;
            }
            return result;
        }

        public static float Range(float min, float max, string key = "defaut")
        {

            System.Random rand;
            if (!m_randomDict.TryGetValue(key, out rand))
            {
                rand = new System.Random(m_seed);
                m_randomDict[key] = rand;
                m_timesDict[key] = 0;
            }

            float result = (float)(rand.NextDouble() * (max - min) + min);

            if (key == m_logKey)
            {
                Debug.Log(key + "    RandomHelper times:" + m_timesDict[key] + "   result = " + result);
                m_timesDict[key]++;
            }
            return result;
        }


        public static int Next(string key = "defaut")
        {
            System.Random rand;
            if (!m_randomDict.TryGetValue(key, out rand))
            {
                rand = new System.Random(m_seed);
                m_randomDict[key] = rand;
                m_timesDict[key] = 0;
            }

            int result = rand.Next();
            if (key == m_logKey)
            {
                Debug.Log(key + "    RandomHelper times:" + m_timesDict[key] + "   result = " + result);
                m_timesDict[key]++;
            }

            return result;
        }

        public static void InitSeed(int seed)
        {
            m_seed = seed;
            m_randomDict.Clear();
            m_timesDict.Clear();
            Debug.Log("RandomHelper InitSeed:" + seed);
        }
    }
}
