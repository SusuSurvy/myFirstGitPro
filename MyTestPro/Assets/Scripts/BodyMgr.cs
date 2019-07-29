using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 蛇身控制
/// </summary>
public class BodyMgr : MonoBehaviour
{
    public Transform TargeTransform;
    public List<Vector3> PosList;

    public void SetTargetPos(Vector3 pos)
    {
        PosList.Add(pos);
    }

    public Vector3 GetDir()
    {
        return m_dir;
    }

    void Awake()
    {
        PosList = new List<Vector3>(); 
    }

    public void InitPosList(List<Vector3> nextList)
    {
       
        for (int i = 0; i < nextList.Count; i++)
        {
            PosList.Add(nextList[i]);
        }
    }

    private Vector3 m_dir;

  

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (PosList.Count > 0)
	    {
	        m_dir = (PosList[0] - transform.localPosition).normalized;
	        transform.localPosition += m_dir * Time.deltaTime * PlayerMgr.Speed;
            if (Vector2.Distance(PosList[0], transform.localPosition) <2)
            {
                transform.localPosition = PosList[0];

                PosList.RemoveAt(0);
	          

	        }

	    }
	    else
	    {
	        m_dir = (TargeTransform.localPosition - transform.localPosition).normalized;
	        transform.localPosition += m_dir * Time.deltaTime * PlayerMgr.Speed;
        }
	  


    }
}
