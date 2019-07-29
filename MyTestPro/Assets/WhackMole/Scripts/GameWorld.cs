using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Framework.Common;
using PictureMatching;
using RetroSnake;
using UnityEngine;
using RobotAvatar.Commom.Event;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
/// <summary>
/// 游戏逻辑控制
/// </summary>
public class GameWorld : MonoBehaviour {
    private GameObject m_resObj;
    private GameObject m_obsObj;
    private GameObject m_trapObj;
    private GameObject m_batObj;
    private List<Item> m_itemList;
    private List<Transform> m_obsList;
    private List<Transform> m_bats;
    private int m_currentIndex;

    private int m_maxIndex;
    private GameObject m_player;
    private GameObject GamePauseWindow;
    private Button RestartBtn, NextBtn, MenuBtn, ContinueBtn;
    private Text m_GameResult;
    // Use this for initialization
    /// <summary>
    /// 实例化按钮的一些操作
    /// </summary>
    void Start ()
    {
       
        GameObject obj = GameObject.Find("Canvas/Main");

        m_resObj = ObjectEX.GetGameObjectByName(obj, "Item");
        m_obsObj = ObjectEX.GetGameObjectByName(obj, "BigObs");
        m_player = ObjectEX.GetGameObjectByName(obj, "player");
        m_trapObj = ObjectEX.GetGameObjectByName(obj, "TrapObs");
        m_batObj = ObjectEX.GetGameObjectByName(obj,"Bat");
        m_GameResult = ObjectEX.GetGameObjectByName(obj, "GameResult").GetComponent<Text>();
        m_resObj.SetActive(false);
            GamePauseWindow = ObjectEX.GetGameObjectByName(obj, "GamePauseWindow").gameObject;

        MenuBtn = ObjectEX.GetGameObjectByName(obj, "Menu").GetComponent<Button>();
        MenuBtn.onClick.AddListener(() =>
            {
                if (GamePauseWindow.activeSelf)
                {
                    ShowPauseWindow(false);
                }
                else
                {
                    GamePause();
                    ShowPauseWindow(true);
                }


            });
        ContinueBtn = ObjectEX.GetGameObjectByName(obj, "ContinBtn").GetComponent<Button>();
        ContinueBtn.onClick.AddListener(() =>
        {
            ShowPauseWindow(false);
        });
        Button quitButton = ObjectEX.GetGameObjectByName(obj, "QuitBtn").GetComponent<Button>();
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();

        });
       RestartBtn = ObjectEX.GetGameObjectByName(obj, "RestartBtn").GetComponent<Button>();
            RestartBtn.onClick.AddListener(() =>
            {
                Time.timeScale = 1;
                string level = "Level1";
               // string level = "Level" + MainMgr.Instance.CurrentLevel;
                SceneHelper.LoadSceneAsync(level, (bool val) => { });
                //     SceneManager.LoadSceneAsync(Level);
                GamePauseWindow.SetActive(false);
            });
            NextBtn = ObjectEX.GetGameObjectByName(obj, "NextBtn").GetComponent<Button>();
        NextBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            MainMgr.Instance.CurrentLevel++;
            if (MainMgr.Instance.CurrentLevel > 10)
            {
                MainMgr.Instance.CurrentLevel = 1;
            }
            string level = "Level1";
            // string level = "Level" + MainMgr.Instance.CurrentLevel;
            SceneHelper.LoadSceneAsync(level, (bool val) => { });
            //     SceneManager.LoadSceneAsync(Level);
            GamePauseWindow.SetActive(false);
        });
            GamePauseWindow.SetActive(false);
            EventDispatcher.AddEventListener<bool>(GameEvent.GameLose, GameLose);
            EventDispatcher.AddEventListener(GameEvent.GameWin, GameWin);
            EventDispatcher.AddEventListener(GameEvent.GamePause, GamePause);
            EventDispatcher.AddEventListener<int>(GameEvent.TouchItem, TouchItem);

            StartLevel(MainMgr.Instance.CurrentLevel);
        
       

        
    }

   
    /// <summary>
    /// 显示暂停界面
    /// </summary>
    /// <param name="val"></param>
    /// <param name="result"></param>
    private void ShowPauseWindow(bool val,string result="")
    {
        GamePauseWindow.SetActive(val);
        Time.timeScale = val?0:1;
        m_GameResult.text = result;
    }
    /// <summary>
    /// 游戏胜利
    /// </summary>
    private void GameWin()
    {
        ContinueBtn.gameObject.SetActive(false);
        MenuBtn.gameObject.SetActive(false);
        RestartBtn.gameObject.SetActive(false);
        NextBtn.gameObject.SetActive(true);
        NextBtn.transform.localPosition=new Vector3(0,0,0);
        Debug.Log("Game Win");
        ShowPauseWindow(true,"胜利");
    }
    /// <summary>
    /// 游戏失败
    /// </summary>
    /// <param name="isTouchItem"></param>
    private void GameLose(bool isTouchItem)
    {
        ContinueBtn.gameObject.SetActive(false);
        MenuBtn.gameObject.SetActive(false);
        if (isTouchItem)
        ShowPauseWindow(true, "字词顺序错误");
        else
        {
            ShowPauseWindow(true, "失败");
        }
        RestartBtn.gameObject.SetActive(true);
        NextBtn.gameObject.SetActive(false);
        RestartBtn.transform.localPosition = new Vector3(0, 0, 0);
        for (int i = 0; i < m_bats.Count; i++)
        {
            m_bats[i].gameObject.SetActive(false);
        }
        Debug.Log("Game Lose");
  
    }
    /// <summary>
    /// 游戏暂停
    /// </summary>
    private void GamePause()
    {
        ContinueBtn.gameObject.SetActive(true);
        Debug.Log("Game Pause");
        RestartBtn.gameObject.SetActive(true);
        NextBtn.gameObject.SetActive(false);
        RestartBtn.transform.localPosition = new Vector3(0, 0, 0);
    }
    /// <summary>
    /// 触碰到字词或障碍物时的操作
    /// </summary>
    /// <param name="_item"></param>
    private void TouchItem(int _item)
    {
        if (_item != m_currentIndex)//字词顺序不对
        {
            GameLose(true);
        }
        else
        {
            if (m_currentIndex == m_maxIndex)//游戏胜利
            {
                GameWin();
            }
            else
            {
                Debug.Log(m_currentIndex);
                m_currentIndex++;
               
            }
         
        }
    }

    void OnDestroy()
    {
        EventDispatcher.RemoveEventListener<bool>(GameEvent.GameLose, GameLose);
        EventDispatcher.RemoveEventListener(GameEvent.GameWin, GameWin);
        EventDispatcher.RemoveEventListener(GameEvent.GamePause, GamePause);
        EventDispatcher.Cleanup();
    }
    
/// <summary>
/// 创建障碍物
/// </summary>
/// <param name="level"></param>
    private void CreateObs(int level)
    {

        ObsTable.Data obsData = MainMgr.Instance.TableMgr.ObsTable.getData(level - 1);
        int obsCount = obsData.BigObs;
        int trapsCount = obsData.Traps;
        int batCount = obsData.Bat;
        for (int i = 0; i < batCount; i++)
        {

            GameObject obsObj = Instantiate(m_batObj);
            obsObj.SetActive(true);
            BatMgr item = obsObj.GetComponent<BatMgr>();
            if (item == null)
            {
                item = obsObj.AddComponent<BatMgr>();
            }
            obsObj.transform.SetParent(m_batObj.transform.parent);
            obsObj.transform.localScale = m_batObj.transform.localScale;
            Vector2 pos = GetBatEnablePos();
            obsObj.transform.localPosition = pos;
            m_bats.Add(obsObj.transform);
        }
        for (int i = 0; i < obsCount; i++)
        {

            GameObject obsObj = Instantiate(m_obsObj);
            obsObj.SetActive(true);

            obsObj.transform.SetParent(m_obsObj.transform.parent);
            obsObj.transform.localScale = m_obsObj.transform.localScale;
            Vector2 pos = GetObsEnablePos();
            obsObj.transform.localPosition = pos;
            m_obsList.Add(obsObj.transform);
        }
     
        for (int i = 0; i < trapsCount; i++)
        {

            GameObject obsObj = Instantiate(m_trapObj);
            obsObj.SetActive(true);
            TrapMgr item = obsObj.GetComponent<TrapMgr>();
            if (item == null)
            {
                item = obsObj.AddComponent<TrapMgr>();
            }
            obsObj.transform.SetParent(m_trapObj.transform.parent);
            obsObj.transform.localScale = m_trapObj.transform.localScale;
            Vector2 pos = GetObsEnablePos();
            obsObj.transform.localPosition = pos;
            m_obsList.Add(obsObj.transform);
        }
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="level"></param>
    public void StartLevel(int level)
    {
        m_currentIndex = 1;
        m_itemList = new List<Item>();
        m_obsList=new List<Transform>();
        m_bats=new List<Transform>();
        CreateObs(level);
        //通过表单获取到所有本官字词信息
        List<LevelTable.Data> m_itemDatas =MainMgr.Instance.TableMgr.LevelTable.DataList.Where(d => d.Level == level).ToList();

       
        m_maxIndex = m_itemDatas.Count;
        foreach (LevelTable.Data mItemData in m_itemDatas)
        {
            GameObject itemObj = Instantiate(m_resObj);
            itemObj.SetActive(true);

            itemObj.transform.SetParent(m_resObj.transform.parent);
            itemObj.transform.localScale = m_resObj.transform.localScale;
            Item item = itemObj.GetComponent<Item>();
            if (item == null)
            {
                item = itemObj.AddComponent<Item>();
            }

            Vector2 pos = GetItemEnablePos();
            itemObj.transform.localPosition = pos;
            ItemInfo itemInfo = new ItemInfo();
            itemInfo.Info = mItemData.Info;
            itemInfo.Index = mItemData.Index;
            itemInfo.Pos = pos;
            itemInfo.NatureData = MainMgr.Instance.TableMgr.NatureTable.getData(mItemData.Nature);
            item.Init(itemInfo);
            m_itemList.Add(item);
        }
    }
    /// <summary>
    /// 创建字词时达到不重叠随机效果
    /// </summary>
    /// <returns></returns>
    private Vector2 GetItemEnablePos()
    {
        Vector2 pos = new Vector2();
        bool isSuit = false;
        while (!isSuit)
        {
            bool _suit = true;
            pos = new Vector2(Random.Range(-800, 800), Random.Range(-400, 400));
            for (int i = 0; i < m_obsList.Count; i++)
            {
                if (Vector2.Distance(pos, m_obsList[i].transform.localPosition) < 150)
                {
                    _suit = false;
                    break;
                }
            }
            for (int i = 0; i < m_itemList.Count; i++)
            {
                if (Vector2.Distance(pos, m_itemList[i].Pos) < 150)
                {
                    _suit = false;
                    break;
                }
            }

            if (Vector2.Distance(pos,m_player.transform.localPosition) < 300)
            {
                _suit = false;
             
            }
            if (_suit)
            {
                isSuit = true;
            }
        }


        return pos;
    }
    /// <summary>
    /// 创建蝙蝠时达到不重叠随机效果
    /// </summary>
    /// <returns></returns>
    private Vector2 GetBatEnablePos()
    {

        Vector2 pos = new Vector2();
        bool isSuit = false;
        while (!isSuit)
        {
            bool _suit = true;
            pos = new Vector2(Random.Range(-700, 700), Random.Range(-300, 300));
          
            if (Vector2.Distance(pos, m_player.transform.localPosition) < 400)
            {
                _suit = false;

            }
            if (_suit)
            {
                isSuit = true;
            }
        }


        return pos;
    }
    /// <summary>
    /// 创建障碍时达到不重叠随机效果
    /// </summary>
    /// <returns></returns>
    private Vector2 GetObsEnablePos()
    {

        Vector2 pos = new Vector2();
        bool isSuit = false;
        while (!isSuit)
        {
            bool _suit = true;
            pos = new Vector2(Random.Range(-700, 700), Random.Range(-350, 350));
            for (int i = 0; i < m_obsList.Count; i++)
            {
                if (Vector2.Distance(pos, m_obsList[i].transform.localPosition) < 150)
                {
                    _suit = false;
                    break;
                }
            }
            if (Vector2.Distance(pos, m_player.transform.localPosition) < 300)
            {
                _suit = false;

            }
            if (_suit)
            {
                isSuit = true;
            }
        }


        return pos;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
