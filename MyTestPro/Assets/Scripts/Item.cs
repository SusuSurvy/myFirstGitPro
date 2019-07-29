using System.Collections;
using System.Collections.Generic;
using RetroSnake;
using UnityEngine;
using  UnityEngine.UI;
/// <summary>
/// 字词相关属性
/// </summary>
public class Item : MonoBehaviour
{
    private Text m_Info;

    private Text m_Nature;

    private Image m_Image;
    public int Index;
    public Vector2 Pos;
    public void Init(ItemInfo data)
    {
        m_Image = transform.GetComponent<Image>();
        m_Nature = transform.transform.Find("Nature").GetComponent<Text>();
        m_Nature.text = data.NatureData.info;
        m_Info = transform.transform.Find("Info").GetComponent<Text>();
        m_Info.text = data.Info;
        m_Image.color=new Color(data.NatureData.colour[0]/255.0f, data.NatureData.colour[1] / 255.0f, data.NatureData.colour[2] / 255.0f);
        Pos = data.Pos;
        Index = data.Index;
    }


    public Item()
    {
       
       
      
    }
}
