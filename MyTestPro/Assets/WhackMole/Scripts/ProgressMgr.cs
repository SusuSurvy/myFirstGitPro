using System.Collections;
using System.Collections.Generic;
using Framework.Common;
using UnityEngine;
using UnityEngine.UI;
namespace WhackMole
{
   

    public class ProgressMgr : MonoBehaviour
    {
        public static int TotalProgress = 1000;//必须是1000的整数倍，下列增加项没限制

        public static int SleepAddProgress = 1;

        public static int InternetAddProgress = 1;

        public static int WorkAddProgress = 1;

        public static int PlayPhoneAddProgress = 1;

        private Slider m_progressSlider;

        private Text m_progressText;

        private int m_currentProgress;

        private float multiple;
        private void Start()
        {
            multiple = TotalProgress/100.0f;
            m_progressSlider = gameObject.GetComponent<Slider>();
            m_progressSlider.maxValue = 100;
            m_progressText = ObjectEX.GetGameObjectByName(gameObject, "Amount").GetComponent<Text>();
            m_currentProgress = 0;
            EventDispatcher.AddEventListener<int>(BattleEvent.AddProgress, RefreshProgress);
        }

        private float m_value;
        private void RefreshProgress(int progress)
        {
           
            m_currentProgress += progress;
            m_value = m_currentProgress / multiple;
            m_progressSlider.value = m_value;
            m_progressText.text = string.Format("{0}%", m_value);
            if (m_currentProgress >= TotalProgress)
            {
                EventDispatcher.TriggerEvent(BattleEvent.GameEnd,true);
            }


        }

        private void OnDestroy()
        {
            EventDispatcher.RemoveEventListener<int>(BattleEvent.AddProgress, RefreshProgress);
        }

    }

}

