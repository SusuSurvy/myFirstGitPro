using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Common;

namespace RetroSnake
{
    public class ObsTable 
    {
        public enum EData
        {
            E_1 = 0,
            E_2,
            E_3,
            E_4,
            E_5,
            E_6,
            E_7,
            E_8,
            E_9,
            E_10,
            Count,
        };

        public class Data
        {
            public int  BigObs {get; private set;}
            public int  Traps {get; private set;}
            public int  Bat {get; private set;}

            public void Init(
                int BigObs,
                int Traps,
                int Bat
            )
            {
                this.BigObs = BigObs;
                this.Traps = Traps;
                this.Bat = Bat;
            }
        }

        private List<Data> _dataList = null;

        public List<Data> DataList { get { return _dataList; } }

        public void Init(BufferHelper helper)
        {
            var num = helper.ReadInt32();
            if(num < (int)EData.Count)
            {
                throw new Exception(" invalid ");
            }

            _dataList = new List<Data>(num);
            for(var i = 0; i < num; ++i)
            {
                var data = new Data();
                data.Init(
                    BigObs : helper.ReadInt32(),
                    Traps : helper.ReadInt32(),
                    Bat : helper.ReadInt32()
                );

                _dataList.Add(data);
            }
        }

        public Data getData(EData e)
        {
            return _dataList[Convert.ToInt32(e)];
        }

        public Data getData(int id)
        {
            return _dataList[id];
        }
    }
} //namespace
