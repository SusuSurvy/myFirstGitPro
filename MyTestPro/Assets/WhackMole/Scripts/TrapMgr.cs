using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;
public class TrapMgr : MonoBehaviour
{
    private Image m_image;

    private float m_time;

    private float DeltaTime;

    private bool isCollider = false;
    private bool m_isTrigger;
    private Color m_originalColor;

    private Collider m_collider;
	// Use this for initialization
	void Start ()
	{
	    m_time = 0;
        m_image = transform.GetComponent<Image>();
	    DeltaTime = Random.Range(3f, 10f);
	    m_originalColor = m_image.color;
	    m_isTrigger = false;
	    m_collider = transform.GetComponent<Collider>();
	    m_collider.enabled = false;
	    m_image.color= new Color(m_originalColor.r, m_originalColor.g, m_originalColor.b,0);


    }
	
	// Update is called once per frame
	void Update ()
	{
	    m_time += Time.deltaTime;
        if (m_isTrigger == false)
	    {
	     
	        if (m_time > DeltaTime)
	        {
	           
	                m_isTrigger = true;
	                m_time = 0;

	            
	        }
	    }
	    else
	        {
	            if (isCollider == false)
	            {
	                m_image.color=new Color(m_originalColor.r,m_originalColor.g,m_originalColor.b,m_time*0.5f);
	                if (m_time > 2)
	                {
	                    m_isTrigger = false;
	                    m_time = 0;
	                    isCollider = true;

                        m_collider.enabled = isCollider;


	                }

	            }
	            else
	            {
	                m_image.color = new Color(m_originalColor.r, m_originalColor.g, m_originalColor.b,1- m_time * 0.5f);
	                if (m_time > 2)
	                {
	                    m_isTrigger = false;
	                    m_time = 0;
	                    isCollider = false;

	                    m_collider.enabled = isCollider;


	                }
            }
	        }
        }

	  
	
}
