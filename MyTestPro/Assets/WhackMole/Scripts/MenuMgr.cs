using System.Collections;
using System.Collections.Generic;
using Framework.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WhackMole
{

    public class MenuMgr : MonoBehaviour
    {
        private Button m_RestartBtn;

        private Button m_QuitBtn;

        private Text m_resultText;

        private GameObject m_menu;
        // Use this for initialization
        void Start()
        {
            m_menu = ObjectEX.GetGameObjectByName(gameObject, "Menu");
            m_menu.SetActive(false);
            m_RestartBtn = ObjectEX.GetGameObjectByName(m_menu, "Restart").GetComponent<Button>();
            m_RestartBtn.onClick.AddListener(() =>
            {
                Time.timeScale = 1;
                SceneManager.LoadSceneAsync("Main");
            });
            m_QuitBtn = ObjectEX.GetGameObjectByName(m_menu, "QuitBtn").GetComponent<Button>();
            m_QuitBtn.onClick.AddListener(() =>
            {

                Application.Quit();
            });
            m_resultText = ObjectEX.GetGameObjectByName(m_menu, "Result").GetComponent<Text>();
            EventDispatcher.AddEventListener<bool>(BattleEvent.GameEnd,ShowMenu);
        }

        private void ShowMenu(bool isSuccess)
        {
            Time.timeScale = 0;
            m_menu.SetActive(true);
            if (isSuccess)
            {
                m_resultText.text = "胜利";
            }
            else
            {
                m_resultText.text = "失败";
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            EventDispatcher.RemoveEventListener<bool>(BattleEvent.GameEnd, ShowMenu);
        }
    }
}

