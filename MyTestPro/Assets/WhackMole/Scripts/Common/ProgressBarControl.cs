using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Lobby3D
{
    public class ProgressBarControl
    {
        private GameObject m_progress;

        private RectTransform m_progressRect;
        private float m_progressLen;

        private List<GameObject> m_boPoints;
        private GameObject m_boPoint;


        public ProgressBarControl(GameObject progress, GameObject boPoint)
        {
            m_boPoints = new List<GameObject>();
            m_boPoint = boPoint;
            m_progress = progress;
            m_progressRect = m_progress.GetComponent<RectTransform>();
            m_progressLen = m_progressRect.sizeDelta.x;
        }

        public void SetBoPoints(List<float> ContinueTimers, float TotalTimer)
        {
            m_boPoint.SetActive(true);
            if (m_boPoints.Count + 1 > ContinueTimers.Count)
            {
                foreach (var i in m_boPoints)
                {
                    GameObject.Destroy(i);
                }
                m_boPoints.Clear();
            }

            while (m_boPoints.Count + 1 < ContinueTimers.Count - 1)
            {
                GameObject tmp = GameObject.Instantiate(m_boPoint);
                tmp.transform.SetParent(m_boPoint.transform.parent, false);
                tmp.transform.localScale = Vector3.one;
                m_boPoints.Add(tmp);
            }

            Vector2 pos = m_boPoint.GetComponent<RectTransform>().anchoredPosition;
            pos.x = ContinueTimers[0] / TotalTimer * m_progressLen;
            m_boPoint.GetComponent<RectTransform>().anchoredPosition = pos;

            float currNumber = ContinueTimers[0];
            for (int i = 1; i < ContinueTimers.Count - 1; ++ i)
            {
                pos = m_boPoint.GetComponent<RectTransform>().anchoredPosition;
                currNumber += ContinueTimers[i];
                pos.x = currNumber / TotalTimer * m_progressLen;
                m_boPoints[i - 1].GetComponent<RectTransform>().anchoredPosition = pos;
            }
        }

		public void SetProgressSign(float progress)
		{
			Vector2 pos = m_boPoint.GetComponent<RectTransform>().anchoredPosition;
			pos.x = progress * m_progressLen;
			m_boPoint.GetComponent<RectTransform>().anchoredPosition = pos;
		}

        public void SetProgress(float progress)
        {
            Vector2 size = m_progressRect.sizeDelta;
            size.x = m_progressLen * progress;
            m_progressRect.sizeDelta = size;
        }

        public Vector3 GetEneryBallPoint(int bo)
        {
            if (bo == 1)
            {
                return m_boPoint.transform.position;
            }

            if (bo - 1 > m_boPoints.Count)
            {
                return Vector3.zero;
            }

            return m_boPoints[bo - 2].transform.position;
        }
    }
}
