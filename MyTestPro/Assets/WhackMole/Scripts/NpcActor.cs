using System.Collections;
using System.Collections.Generic;
using Framework.Common;
using UnityEngine;

namespace WhackMole
{
    public class NpcActor : MonoBehaviour
    {

        private const float MiniPlayTime = 2.0f;

        private const float MaxPlayTime = 10.0f;
        private enum State
        {
            Work=0,
            Sleep,
            PlayPhone,
            Internet,
            Scare,
            Count

        }

        private State m_currentState;

        private GameObject m_workImage, m_sleepImage, m_playPhoneImage, m_internetImage, m_scareImage;

        private GameObject m_lastImage;

        private Timer m_trigger;
        // Use this for initialization
        void Start()
        {
            InitUi();
            WaitToPlay();
        }

        private void InitUi()
        {
            m_workImage = ObjectEX.GetGameObjectByName(gameObject, "Work");
            m_sleepImage = ObjectEX.GetGameObjectByName(gameObject, "Sleep");
            m_sleepImage.SetActive(false);
            m_playPhoneImage = ObjectEX.GetGameObjectByName(gameObject, "PlayPhone");
            m_playPhoneImage.SetActive(false);
            m_internetImage = ObjectEX.GetGameObjectByName(gameObject, "Internet");
            m_internetImage.SetActive(false);
            m_scareImage = ObjectEX.GetGameObjectByName(gameObject, "Scare");
            m_scareImage.SetActive(false);
            m_lastImage = m_workImage;
            m_lastImage.SetActive(transform);
            m_currentState = State.Work;

        }

        private void WaitToPlay()
        {
          
            float seed = RandomHelper.Range(MiniPlayTime, MaxPlayTime);
            m_trigger= Timer.New(seed, () => { ChangeToPlay(); });
        }

        private void ChangeToPlay()
        {
            int seed = RandomHelper.Range(1, (int)State.Count);
            State state = (State)seed;
            GameObject temObj = null;
            switch (state)
            {
                case State.Sleep:
                    temObj = m_sleepImage;
                    break;
                case State.PlayPhone:
                    temObj = m_playPhoneImage;
                    break;
                case State.Internet:
                    temObj = m_internetImage;
                    break;
                case State.Scare:
                    temObj = m_scareImage;
                    break;
            }

            if (temObj != null)
            {
                m_lastImage.SetActive(false);
                m_lastImage = temObj;
                m_lastImage.SetActive(true);
            }

            WaitToPlay();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

