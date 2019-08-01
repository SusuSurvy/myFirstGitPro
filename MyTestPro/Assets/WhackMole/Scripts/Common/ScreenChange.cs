using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby3D
{
    public class ScreenChange : MonoBehaviour
    {

        private float ScreenX = 1920f;
        private float ScreenY = 1080f;
        private float ScreenMultiple;

        // Use this for initialization
        void Start()
        {
            ScreenMultiple = (ScreenY / ScreenX) * ((float)Screen.width / Screen.height);
            this.transform.localPosition = new Vector3
                (this.transform.localPosition.x * ScreenMultiple,
                this.transform.localPosition.y,
                this.transform.localPosition.z);
        }

    }
}
