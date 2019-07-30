using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Common;

namespace WhackMole
{
    public class NatureTable 
    {
        public enum EData
        {
            Nature1 = 0,
            Nature2,
            Nature3,
            Nature4,
            Nature5,
            Nature6,
            Nature7,
            Nature8,
            Nature9,
            Nature10,
            Nature11,
            Nature12,
            Count,
        };

        public class Data
        {
            public String  info {get; private set;}
            public List<int>  colour {get; private set;}

            public void Init(
                String info,
                List<int> colour
            )
            {
                this.info = info;
                this.colour = colour;
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
                    info : helper.ReadStr(),
                    colour : helper.ReadInt32List()
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
