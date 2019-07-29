using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PictureMatching;
using UnityEngine;
using RobotAvatar.Commom.Event;
/// <summary>
/// 蛇头控制
/// </summary>
public class PlayerMgr : MonoBehaviour
{
    public const int Speed = 200;
    public const int DeltaLength = 70;
    Vector2 direction = Vector2.up;
    List<BodyMgr> bodyList = new List<BodyMgr>();
    void Start()
    {
   
        m_time = 0;
        transform.localEulerAngles = new Vector3(0, 0, 90);
    }

    private float m_time;
    void Update()
    {
        m_time += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.W)&& direction!= Vector2.up&& direction != Vector2.down)
        {
            direction = Vector2.up;
            transform.localEulerAngles=new Vector3(0,0,90);
            AddTargetPoint();
        }
        if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up && direction != Vector2.down)
        {
            direction = Vector2.down;
            transform.localEulerAngles = new Vector3(0, 0, -90);
            AddTargetPoint();
        }
        if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.left && direction != Vector2.right)
        {
            direction = Vector2.left;
            transform.localEulerAngles = new Vector3(0, 180, 0);
            AddTargetPoint();
        }
        if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left && direction != Vector2.right)
        {
            direction = Vector2.right;
            transform.localEulerAngles = new Vector3(0, 0, 0);
            AddTargetPoint();
        }
        transform.localPosition += new Vector3(direction.x, direction.y, 0) * Time.deltaTime * Speed;
       
    }

    private void AddTargetPoint()
    {
        if (bodyList.Count > 0)
        {
            for (int i = bodyList.Count - 1; i >= 0; i--)
            {
                bodyList[i].SetTargetPos(transform.localPosition);
             //   bodyList[i].SetTargetPos(bodyList[i - 1].transform.localPosition);
            }
          //  bodyList[0].SetTargetPos(transform.localPosition);
        }

      
    }

    private void SetOriginalPos(BodyMgr body)
    {
        float deltaLength = DeltaLength;
        Transform m_targetTrans;
        Vector3 dir;
        if (bodyList.Count > 0)
        {
            m_targetTrans = bodyList.Last().transform;
            dir = bodyList.Last().GetDir();
            body.InitPosList(bodyList.Last().PosList);
        }
        else
        {
            m_targetTrans = transform;
            dir = direction;
        }
        Debug.Log(dir);
     
        
        Vector3 pos = new Vector3();
        pos = m_targetTrans.localPosition - dir* deltaLength;


        body.transform.localPosition = pos;
        bodyList.Add(body);


    }



    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer ==8)
        {
            EventDispatcher.TriggerEvent(GameEvent.GameLose,false);
            return;
        }
       

        Item itemInfo = other.GetComponent<Item>();

        EventDispatcher.TriggerEvent(GameEvent.TouchItem,itemInfo.Index);
        other.transform.GetComponent<Collider>().enabled = false;
    
        BodyMgr bodyMgr = other.GetComponent<BodyMgr>();
            if (bodyMgr == null)
            {
                bodyMgr = other.gameObject.AddComponent<BodyMgr>();
          
            }
            
            if (bodyList.Count > 0)
            {
                bodyMgr.TargeTransform = bodyList.Last().transform;
             
            }
            else
            {
                bodyMgr.TargeTransform = transform;
         
        }

        SetOriginalPos(bodyMgr);
      
      
      
    }

  
}
