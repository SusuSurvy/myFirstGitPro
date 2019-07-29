using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 蝙蝠运动逻辑
/// </summary>
public class BatMgr : MonoBehaviour
{
    private Vector2 direction;

    private float m_time;

    private float DeltaTime;
    public  int Speed = 150;
    private bool m_isTrigger = false;
    
    private Rect Bord=new Rect(-700,-300,1400,600);
    // Use this for initialization
    void Start ()
    {
        Debug.Log(Bord.xMin+"__"+Bord.xMax+"__"+Bord.yMin+"__"+Bord.yMax);
        direction=new Vector2();
        m_isTrigger = false;
        CreateRandomDir();
    }

    private void CreateRandomDir()
    {
        Vector2 Dir=new Vector2();
        do
        {
            int seed = Random.Range(0, 4);
            switch (seed)
            {
                case 0:
                    Dir = Vector2.up;
                    break;
                case 1:
                    Dir = Vector2.down;
                    break;
                case 2:
                    Dir = Vector2.left;
                    break;
                case 3:
                    Dir = Vector2.left;
                    break;

            }
        } while (Dir == direction);

        direction = Dir;
        DeltaTime = Random.Range(3.0f, 6.0f);
        Speed = Random.Range(150, 300);
    }

    // Update is called once per frame
	void Update ()
	{
	    m_time += Time.deltaTime;
	    if (m_isTrigger)
	    {
	        if (m_time > 2)
	        {
	            m_time = 0;
	            CreateRandomDir();
	            m_isTrigger = false;

	        }
	    }
	    else
	    {
	        transform.localPosition += new Vector3(direction.x, direction.y, 0) * Time.deltaTime * Speed;
            if(!Bord.Contains(new Vector2(transform.localPosition.x,transform.localPosition.y)))
            {
                Debug.Log(transform.localPosition);
                ReSetDir();

            }	    
            if (m_time > DeltaTime)
	        {
	            m_isTrigger = true;
	            m_time = 0;

	        }
	    }
	}

    private void ReSetDir()
    {
        if (transform.localPosition.x >= 700)
        {
            direction=Vector2.left;
            
        }
        else if (transform.localPosition.x <= -700)
        {
            direction = Vector2.right;
        }
        else if (transform.localPosition.y >= 300)
        {
            direction=Vector2.down;
            ;
        }
        else if (transform.localPosition.y <= -300)
        {
            direction = Vector2.up;
            ;
        }
        transform.localPosition += new Vector3(direction.x, direction.y, 0) * Time.deltaTime * Speed*2;
    }
}
