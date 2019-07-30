using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Common;

namespace WhackMole
{
    public class LevelTable 
    {
        public enum EData
        {
            Parm1 = 0,
            Parm2,
            Parm3,
            Parm4,
            Parm5,
            Parm6,
            Parm7,
            Parm8,
            Parm9,
            Parm10,
            Parm11,
            Parm12,
            Parm13,
            Parm14,
            Parm15,
            Parm16,
            Parm17,
            Parm18,
            Parm19,
            Parm20,
            Parm21,
            Parm22,
            Parm23,
            Parm24,
            Parm25,
            Parm26,
            Parm27,
            Parm28,
            Parm29,
            Parm30,
            Parm31,
            Parm32,
            Parm33,
            Parm34,
            Parm35,
            Parm36,
            Parm37,
            Parm38,
            Parm39,
            Parm40,
            Parm41,
            Parm42,
            Parm43,
            Parm44,
            Parm45,
            Parm46,
            Parm47,
            Parm48,
            Parm49,
            Parm50,
            Parm51,
            Parm52,
            Parm53,
            Parm54,
            Parm55,
            Parm56,
            Parm57,
            Parm58,
            Parm59,
            Parm60,
            Parm61,
            Parm62,
            Parm63,
            Parm64,
            Parm65,
            Parm66,
            Parm67,
            Parm68,
            Parm69,
            Parm70,
            Parm71,
            Parm72,
            Parm73,
            Parm74,
            Parm75,
            Parm76,
            Parm77,
            Parm78,
            Parm79,
            Parm80,
            Parm81,
            Parm82,
            Parm83,
            Parm84,
            Parm85,
            Parm86,
            Count,
        };

        public class Data
        {
            public int  Level {get; private set;}
            public String  Info {get; private set;}
            public NatureTable.EData  Nature {get; private set;}
            public int  Index {get; private set;}

            public void Init(
                int Level,
                String Info,
                NatureTable.EData Nature,
                int Index
            )
            {
                this.Level = Level;
                this.Info = Info;
                this.Nature = Nature;
                this.Index = Index;
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
                    Level : helper.ReadInt32(),
                    Info : helper.ReadStr(),
                    Nature : (NatureTable.EData)helper.ReadInt32(),
                    Index : helper.ReadInt32()
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
