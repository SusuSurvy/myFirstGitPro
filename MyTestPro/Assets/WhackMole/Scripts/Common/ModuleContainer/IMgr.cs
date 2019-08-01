using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby3D
{
    public interface ITouch
    {

        void OnToucheBegin(GameObject obj, Vector3 touchPos);//返回是否可以拖动（目前条件为：未解锁，不可拖动）

        void OnMove(Vector3 touchPos);

        void OnMoveEnd(Vector3 touchPos);//拖动结束

        void OnTap(Vector3 touchPos);//没有发生拖动 
    }

}

