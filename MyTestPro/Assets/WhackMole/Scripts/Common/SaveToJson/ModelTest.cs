using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby3D
{

    [System.Serializable]
    public class ModelTest
    {
        public string name;
        public int age;
        public string[] interests;
        public bool b = false;
        public List<int> list = new List<int>() { 1, 2 };

        
        public List<ModelData> listData = new List<ModelData>() { new ModelData(), new ModelData() };

        //[SerializeField]
        //public Dictionary<int, string> dict = new Dictionary<int, string>() { { 1, "dict" } };

    }

    [System.Serializable]
    public class ModelListTest
    {
        public List<int> listData = new List<int>() { 3, 4 };
    }


    [System.Serializable]
    public class ModelData
    {
        public int key = 0;
        public string value = "value";
        public ModelListTest modelListTest = new ModelListTest();
    }    

}
