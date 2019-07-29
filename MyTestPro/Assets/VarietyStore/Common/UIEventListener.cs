using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Framework.Common
{
    public enum UIMSGType
    {
        OnPointDown,
        OnPointUp,
        OnPointDrag,
        OnClick,
    }

    public delegate void VoidDelegate(GameObject go);

    public class UIEventListener : UnityEngine.EventSystems.EventTrigger
    {
        private bool m_isDown = false;

        public VoidDelegate m_onPointDown = null;
        public VoidDelegate m_onPointUp = null;
        public VoidDelegate m_onPointDrag = null;

        void Update()
        {
            if (m_isDown)
            {
                if (m_onPointDrag != null)
                {
                    m_onPointDrag(gameObject);
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            m_isDown = true;
            if (m_onPointDown != null)
            {
                m_onPointDown(gameObject);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            m_isDown = false;
            if (m_onPointUp != null)
            {
                m_onPointUp(gameObject);
            }

        }
    }
}