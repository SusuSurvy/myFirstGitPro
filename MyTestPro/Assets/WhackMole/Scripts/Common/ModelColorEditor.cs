using UnityEngine;
using System.Collections;

namespace Lobby3D
{
    public class ModelColorEditor : MonoBehaviour
    {
        private SkinnedMeshRenderer render;
        MaterialPropertyBlock block;
        private Texture2D texture;

        void Awake()
        {
            block = new MaterialPropertyBlock();
            render = GetComponent<SkinnedMeshRenderer>();
            render.GetPropertyBlock(block);
            texture = block.GetTexture("_MainTex") as Texture2D;
        }

        public void SetColor(Color color)
        {
            Debug.Log(color);
            block.SetColor("_Color", color);
            //block.SetTexture("_MainTex", texture);
            render.SetPropertyBlock(block);
        }
    }
}