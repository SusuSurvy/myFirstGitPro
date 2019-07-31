using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhackMole
{
    class StateEvent : IStateEvent
    {

        //如果需要发送延迟消息，则可以建立一个消息管理类。见《人工智能》 2.8  P.56
        //本项目目前无需延迟 直接由m_receiver 处理消息


        private object m_sender;
        private object m_receiver;
        private StateEventType m_eventType;
        private object m_extraEventInfo;
      


        public StateEvent(object sender, object receiver, StateEventType eventType, object extraEventInfo)
        {
            this.m_sender = sender;
            this.m_receiver = receiver;
            this.m_eventType = eventType;
            this.m_extraEventInfo = extraEventInfo;
        }
        public StateEvent( StateEventType eventType)
        {
            
            this.m_eventType = eventType;
        
        }

        public StateEventType EventType
        {
            get
            {
                return this.m_eventType;
            }
            set
            {
                m_eventType = value;
            }
        }

        public object ExtraEventInfo
        {
            get
            {
                return this.m_extraEventInfo;
            }
            set
            {
                m_extraEventInfo = value;
            }
        }

        public object Receiver
        {
            get
            {
                return this.m_receiver;
            }
            set
            {
                m_receiver = value;
            }
        }

        public object Sender
        {
            get
            {
                return this.m_sender;
            }
            set
            {
                m_sender = value;
            }
        }

    }
}
