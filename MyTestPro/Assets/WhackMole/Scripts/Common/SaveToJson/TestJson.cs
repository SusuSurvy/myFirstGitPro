using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using LitJson;

namespace Lobby3D
{

    public class TestJson : MonoBehaviour
    {

        
        // Use this for initialization
        void Start()
        {

            TestDictionary();
            TestObject();
        }

        void TestList()
        {
            //List 放在序列化对象中才行,需要嵌套可以通过对象嵌套 参考ModelListTest
        }

        void TestDictionary()
        {
            SerializableDictionary<int, string> serializableDict = new SerializableDictionary<int, string>();
            serializableDict.Add(0, "0");
            serializableDict.Add(1, "1");
            string str1 = JsonUtility.ToJson(serializableDict);
            Debug.Log(str1);

            SerializableDictionary<int, string> sDict = JsonUtility.FromJson<SerializableDictionary<int, string>>(str1);
            foreach (var v in sDict)
            {
                Debug.Log("key = " + v.Key);
                Debug.Log("value = " + v.Value);
            }
        }

        void TestObject()
        {
            ModelTest model = new ModelTest();
            model.name = "aaa";
            model.age = 11;
            model.interests = new string[] { "xxx", "yyy" };

            string str = JsonUtility.ToJson(model);
            Debug.Log(str);

            ModelTest obj = JsonUtility.FromJson<ModelTest>(str);
            Debug.Log(obj.name);
            Debug.Log(obj.age);
            Debug.Log(obj.b);
            foreach (var inter in obj.interests)
            {
                Debug.Log(inter);
            }
        }
    }
}