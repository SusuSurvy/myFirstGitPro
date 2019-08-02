using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Framework.Common;
using UnityEngine;
using UnityEngine.UI;

namespace WhackMole
{
    public class Actor : MonoBehaviour
    {

    }
    public enum NpcState
    {
        Work = 0,
        Sleep,
        PlayPhone,
        Internet,
        ScareStart,
        Scaring,
        Count

    }

    public enum WorkState
    {
        HitToWork,
        NormalWork,
        OwnerReturnWork,
      
    }

    
    public class NpcActor : Actor
    {

      

        public StateMachine<NpcActor> StateMachine { get; set; }

        private GameObject m_workImage, m_sleepImage, m_playPhoneImage, m_internetImage, m_scareStartImage,m_scaringImage, m_boomImage;

        private GameObject m_hitToWorkImage, m_NormalWorkImage, m_ownerWorkImage;

        private Button m_AbuseBtn, m_HitBtn;

        private GameObject m_lastImage;

        private GameObject m_lastWorkImage;
        public bool IsBossShow;
        // Use this for initialization
        void Start()
        {
            InitUi();
            IsBossShow = false;
            StateMachine =new StateMachine<NpcActor>(this);
            StateMachine.SetCurrentState(new NpcStateWork());
            StateMachine.SetGlobalStateState(new NpcGlobalState());
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
            m_scareStartImage = ObjectEX.GetGameObjectByName(gameObject, "ScareStart");
            m_scareStartImage.SetActive(false);
            m_scaringImage = ObjectEX.GetGameObjectByName(gameObject, "Scaring");
            m_scaringImage.SetActive(false);
            m_boomImage = ObjectEX.GetGameObjectByName(gameObject, "BoomBtn");
            m_boomImage.SetActive(false);
            m_AbuseBtn = ObjectEX.GetGameObjectByName(gameObject, "AbuseBtn").GetComponent<Button>();
            m_AbuseBtn.onClick.AddListener(BossAbuse);
             m_HitBtn = ObjectEX.GetGameObjectByName(gameObject, "HitBtn").GetComponent<Button>();
            m_HitBtn.onClick.AddListener(BossHit);
            m_lastImage = m_workImage;

            m_hitToWorkImage= ObjectEX.GetGameObjectByName(gameObject, "HitToWork");
            m_hitToWorkImage.SetActive(false);
            m_NormalWorkImage = ObjectEX.GetGameObjectByName(gameObject, "NormalWork");
            m_NormalWorkImage.SetActive(false);
            m_ownerWorkImage = ObjectEX.GetGameObjectByName(gameObject, "ReturnWork");
            m_ownerWorkImage.SetActive(false);
            m_lastWorkImage = m_NormalWorkImage;

            m_lastImage.SetActive(transform);
         

        }

        private void BossAbuse()
        {
            StateMachine.HandleMessage(new StateEvent(StateEventType.BossAbuse));
            m_boomImage.SetActive(false);
        }

        private void BossHit()
        {
            StateMachine.HandleMessage(new StateEvent(StateEventType.BossHit));
            m_boomImage.SetActive(false);
        }

        public void RefreshWorkSate(WorkState state)
        {

            GameObject temObj = null;

            float addProgress = 0;
            switch (state)
            {
                case WorkState.HitToWork:
                    addProgress = 0.5f;
                    temObj = m_hitToWorkImage;
                    break;
                case WorkState.NormalWork:
                    addProgress = 1f;
                    temObj = m_NormalWorkImage;
                    break;
                case WorkState.OwnerReturnWork:
                    addProgress = 1.5f;
                    temObj = m_ownerWorkImage;
                    break;
               
            }
            if (m_lastWorkImage != null)
            {
                m_lastWorkImage.SetActive(false);
                if (temObj != null)
                {
                    m_lastWorkImage = temObj;
                }
                m_lastWorkImage.SetActive(true);
                Timer.New(1f, () => { m_lastWorkImage.SetActive(false); });

            }
            EventDispatcher.TriggerEvent(BattleEvent.AddProgress, addProgress);

          

        }

        public void ChangeState(NpcState state)
        {
       
            GameObject temObj = null;
            switch (state)
            {
                case NpcState.Work:
                    temObj = m_workImage;
                    break;
                case NpcState.Sleep:
                    temObj = m_sleepImage;
                    break;
                case NpcState.PlayPhone:
                    temObj = m_playPhoneImage;
                    break;
                case NpcState.Internet:
                    temObj = m_internetImage;
                    break;
                case NpcState.ScareStart:
                    temObj = m_scareStartImage;
                    break;
                case NpcState.Scaring:
                    temObj = m_scaringImage;
                    break;

            }

            if (temObj != null)
            {
                m_lastImage.SetActive(false);
                m_lastImage = temObj;
                m_lastImage.SetActive(true);
            }

        
        }

        public void ShowBoom(bool show)
        {
            m_boomImage.SetActive(show);
        }

        // Update is called once per frame
        void Update()
        {
            StateMachine.SMUpdate(Time.deltaTime);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                IsBossShow = true;
                StateMachine.HandleMessage(new StateEvent(StateEventType.BossCome));
            }

      
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                IsBossShow = false;
                StateMachine.HandleMessage(new StateEvent(StateEventType.BossLeave));
            }
        }
    }

}

