using System.Collections;
using System.Collections.Generic;
using Framework.Common;
using UnityEngine;
using UnityEngine.UI;
namespace WhackMole
{
   

    public class ProgressMgr : MonoBehaviour
    {
        public static float TotalProgress = 1000;//必须是1000的整数倍，下列增加项没限制



        private Slider m_progressSlider;

        private Text m_progressText, m_tartgetProgress;

        private float m_currentProgress;

      
        private void Start()
        {
         
            m_progressSlider = gameObject.GetComponent<Slider>();
            m_tartgetProgress = ObjectEX.GetGameObjectByName(gameObject, "TargetProgress").GetComponent<Text>();
            m_tartgetProgress.text = TotalProgress.ToString();
            m_progressSlider.maxValue = TotalProgress;
            m_progressText = ObjectEX.GetGameObjectByName(gameObject, "Amount").GetComponent<Text>();
            m_currentProgress = 0;
            EventDispatcher.AddEventListener<float>(BattleEvent.AddProgress, RefreshProgress);
        }

   
        private void RefreshProgress(float progress)
        {
           
            m_currentProgress += progress;
           
            m_progressSlider.value = m_currentProgress;
            m_progressText.text = m_currentProgress.ToString();
            if (m_currentProgress >= TotalProgress)
            {
                EventDispatcher.TriggerEvent(BattleEvent.GameEnd,true);
            }


        }

        private void OnDestroy()
        {
            EventDispatcher.RemoveEventListener<float>(BattleEvent.AddProgress, RefreshProgress);
        }

    }

}

