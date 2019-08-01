using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Lobby3D.Commom.UI
{
    [AddComponentMenu("UI/MutiRect", 16)]
    public class MutiRect : MaskableGraphic, ICanvasRaycastFilter
    {
        private List<Rect> m_RectList;

        protected MutiRect()
        {
            m_RectList = new List<Rect>();
        }

        public void Clear()
        {
            m_RectList.Clear();
        }

        public void AddRect(Rect rect)
        {
            m_RectList.Add(rect);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = GetPixelAdjustedRect();
            int offset = 0;
            foreach (var rect in m_RectList)
            {
                var color32 = color;
                float left = r.x + r.width * rect.x;
                float right = r.x + r.width * (rect.x + rect.width);
                float bottom = r.y + r.height * rect.y;
                float top = r.y + r.height * (rect.y + rect.height);

                vh.AddVert(new Vector3(left, bottom), color32, new Vector2(0, 0));
                vh.AddVert(new Vector3(left, top), color32, new Vector2(0, 1));
                vh.AddVert(new Vector3(right, top), color32, new Vector2(1, 1));
                vh.AddVert(new Vector3(right, bottom), color32, new Vector2(1, 0));

                vh.AddTriangle(offset + 0, offset + 1, offset + 2);
                vh.AddTriangle(offset + 2, offset + 3, offset + 0);

                offset += 4;
            }
        }

        public static Rect WorldSpaceRectToMaskRelateveSpace(Rect worldSpaceRect)
        {
            Rect maskSpaceRect = new Rect();
            float viewportHeight = Camera.main.orthographicSize * 2;
            float viewportWidht = viewportHeight * Screen.width / Screen.height;
            float viewportX = Camera.main.transform.position.x - viewportWidht * 0.5f;
            float viewportY = Camera.main.transform.position.y - viewportHeight * 0.5f;
            maskSpaceRect.x = (worldSpaceRect.x - viewportX) / viewportWidht;
            maskSpaceRect.y = (worldSpaceRect.y - viewportY) / viewportHeight;
            maskSpaceRect.width = worldSpaceRect.width / viewportWidht;
            maskSpaceRect.height = worldSpaceRect.height / viewportHeight;
            return maskSpaceRect;
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            Vector2 rsp = new Vector2(sp.x / Screen.width, sp.y / Screen.height);
            foreach (var rect in m_RectList)
            {
                if (rect.Contains(rsp))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
