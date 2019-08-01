using System.Collections;
using System.Collections.Generic;
using Framework.Common;
using UnityEngine;
using UnityEngine.UI;
namespace WhackMole
{
    public class PlayerMgr : MonoBehaviour
    {
        private GameObject m_downImage, m_upImage, m_rightImage, m_leftImage;

        private GameObject m_lastImage;

        private Vector3 m_direction;

        private Direction m_dirEnum;
        private CharacterController m_controller;
        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR
            TestScene.SetEnable(true);
#endif
            m_controller = transform.GetComponent<CharacterController>();
            InitPlayerImage();
        }

        private void InitPlayerImage()
        {
            m_downImage = ObjectEX.GetGameObjectByName(this.gameObject, "down");
            m_upImage = ObjectEX.GetGameObjectByName(this.gameObject, "up");
            m_upImage.SetActive(false);
            m_rightImage = ObjectEX.GetGameObjectByName(this.gameObject, "right");
            m_rightImage.SetActive(false);
            m_leftImage = ObjectEX.GetGameObjectByName(this.gameObject, "left");
            m_leftImage.SetActive(false);
            m_lastImage = m_downImage;
            m_dirEnum = Direction.Down;
            m_lastImage.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
            Update(Time.deltaTime);
        }
        private float m_fpsTimer;
        private int m_fpsCount;
        public void Update(float fTimer)
        {


            // FPS temp code
#if UNITY_EDITOR
            ++m_fpsCount;
            m_fpsTimer += Time.deltaTime;
            if (m_fpsTimer >= 0.5f)
            {

                TestScene.ShowLabel("FPS      ", (m_fpsCount / m_fpsTimer).ToString("f2"));
                m_fpsCount = 0;
                m_fpsTimer = 0f;
            }
#endif


            if (Input.GetKey(KeyCode.A))
            {

                PlayerMove(Direction.Left,fTimer);
            }
            else if (Input.GetKey(KeyCode.D))
            {

                PlayerMove(Direction.Right, fTimer);
            }
            else if (Input.GetKey(KeyCode.W))
            {

                PlayerMove(Direction.Up, fTimer);
            }
            else if (Input.GetKey(KeyCode.S))
            {

                PlayerMove(Direction.Down, fTimer);
            }
           
        }

        private void ChangePlayerDirection(Direction direction)
        {
            GameObject temObj = null;
            switch (direction)
            {
                case Direction.Up:
                    temObj = m_upImage;
                    m_direction = Vector3.up;
                    break;
                case Direction.Down:
                    temObj = m_downImage;
                    m_direction = Vector3.down;
                    break;
                case Direction.Left:
                    temObj = m_leftImage;
                    m_direction = Vector3.left;
                    break;
                case Direction.Right:
                    temObj = m_rightImage;
                    m_direction = Vector3.right;
                    break;
            }

            if (temObj != m_lastImage)
            {
                m_lastImage.SetActive(false);
                m_lastImage = temObj;
                if (m_lastImage != null)
                {
                    m_lastImage.SetActive(true);
                }
                temObj = null;
            }
        
           
         

        }

        private const float Speed = 50;
        private void PlayerMove(Direction direction,float mTimer)
        {
            if (m_dirEnum != direction)
            {
                m_dirEnum = direction;
                ChangePlayerDirection(direction);
            }

            m_controller.Move(m_direction * Speed * mTimer);
            if (transform.localPosition.z != 0)
            {
                Vector3 pos = transform.localPosition;
                pos.z = 0;
                transform.localPosition = pos;
            }

            // transform.localPosition += ;
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Top1":
                    transform.SetSiblingIndex(2);
                    break;
                case "Top2":
                    transform.SetSiblingIndex(3);
                    break;
                case "Top3":
                    transform.SetSiblingIndex(4);
                    break;
                case "Top4":
                    transform.SetSiblingIndex(5);
                    break;


            }
        }
    }

}

