using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.Resource;
using PictureMatching;
using RetroSnake;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct ItemInfo
{
    public string Info;
    public int Index;
    public NatureTable.Data NatureData;
    public Vector2 Pos;
}
static public class GameEvent
{
    public readonly static string GameWin = "GameWin";
    public readonly static string GameLose = "GameLose";
    public readonly static string GameRestart = "GameRestart";
    public readonly static string GamePause = "GamePause";
    public readonly static string TouchItem = "TouchItem";
}
/// <summary>
/// 中心控制器
/// </summary>
public class MainMgr : MonoBehaviour
{
    private TableManager m_tableMgr;
    private ResourceManager m_resMgr;
    public int CurrentLevel;
    public static MainMgr Instance
    {
        get { return m_instance; }
    }
    private static MainMgr m_instance;

    public TableManager TableMgr
    {
        get { return m_tableMgr; }
    }

    // Use this for initialization
    void Start()
    {
        m_instance = this;
        CurrentLevel = 1;
         //  m_resObj = Resources.Load<GameObject>("Item");

         m_resMgr = new ResourceManager("RetroSnake");
        m_tableMgr = new TableManager();
       GameObject.DontDestroyOnLoad(this.gameObject);
        m_resMgr.LoadAsync<TextAsset>("RetroSnake/Tables/tables", (bin) =>
        {
            m_tableMgr.Init(bin.bytes);
            Timer.New(2, () =>
            {
               SceneManager.LoadScene(1);
            });
            
        });
        


    }

   


    // Update is called once per frame
	void Update () {
		
	}
}
