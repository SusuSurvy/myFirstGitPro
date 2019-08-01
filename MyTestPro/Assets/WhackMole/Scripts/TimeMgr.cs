using System.Collections;
using System.Collections.Generic;
using Framework.Common;
using Lobby3D.Commom.Event;
using UnityEngine;
using  UnityEngine.UI;
namespace WhackMole
{
    public class TimeMgr : MonoBehaviour
    {
        private Text m_minuteText, m_secondText;

        private const int TotalTime = 300;

        private float m_time;

        private int m_minute;

        private int m_second;
        private int m_currentTime;
        // Use this for initialization
        void Start()
        {
            m_minuteText = ObjectEX.GetGameObjectByName(gameObject, "minute").GetComponent<Text>();

            m_secondText = ObjectEX.GetGameObjectByName(gameObject, "second").GetComponent<Text>();
            m_currentTime = TotalTime;
            m_time = 0;
            RefreshTime();
        }

        // Update is called once per frame
        void Update()
        {
            m_time += Time.deltaTime;
            if (m_time > 1)
            {
                m_time = 0;
                m_currentTime -= 1;
                RefreshTime();
            }

        }

        private void RefreshTime()
        {
            m_minute = m_currentTime / 60;
            m_second = m_currentTime % 60;
            m_minuteText.text = m_minute.ToString();
            m_secondText.text = m_second >= 10 ? m_second.ToString() : string.Format("0{0}", m_second);
            if (m_currentTime <= 0)
            {
                EventDispatcher.TriggerEvent(BattleEvent.GameEnd,false);
            }
        }
    }

}

