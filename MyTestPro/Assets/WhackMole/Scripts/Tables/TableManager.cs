using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Common;

namespace WhackMole
{
    public class TableManager 
    {
        public NatureTable  NatureTable {get; private set;}
        public ObsTable  ObsTable {get; private set;}
        public LevelTable  LevelTable {get; private set;}

        public IEnumerator Init(string filePath, Action onFinish = null)
        {
            var www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();

            var helper = new BufferHelper(www.downloadHandler.data);

            this.NatureTable = new NatureTable();
            NatureTable.Init(helper);

            this.ObsTable = new ObsTable();
            ObsTable.Init(helper);

            this.LevelTable = new LevelTable();
            LevelTable.Init(helper);

            if (onFinish != null) onFinish();
        }

        public void Init(byte[] buffer)
        {
            var helper = new BufferHelper(buffer);
            this.NatureTable = new NatureTable();
            NatureTable.Init(helper);

            this.ObsTable = new ObsTable();
            ObsTable.Init(helper);

            this.LevelTable = new LevelTable();
            LevelTable.Init(helper);
        }

        public bool InitByResources(string filePath)
        {
            var txtAsset = Resources.Load<TextAsset>(filePath);
            if (txtAsset == null || txtAsset.bytes == null)
                return false;
            
            var helper = new BufferHelper(txtAsset.bytes);
        
            this.NatureTable = new NatureTable();
            NatureTable.Init(helper);

            this.ObsTable = new ObsTable();
            ObsTable.Init(helper);

            this.LevelTable = new LevelTable();
            LevelTable.Init(helper);

            return true;
        }

        public bool InitByBytes(byte[] bytes)
        {
            var helper = new BufferHelper(bytes);
        
            this.NatureTable = new NatureTable();
            NatureTable.Init(helper);

            this.ObsTable = new ObsTable();
            ObsTable.Init(helper);

            this.LevelTable = new LevelTable();
            LevelTable.Init(helper);

            return true;
        }
    }
} //namespace
