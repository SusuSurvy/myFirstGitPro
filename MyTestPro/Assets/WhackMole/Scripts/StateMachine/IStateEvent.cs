using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhackMole
{
    public enum StateEventType : byte
    {
       BossCome,
        BossLeave,
        BossAbuse,
        BossHit,
   
    }

    public interface IStateEvent
    {
        StateEventType EventType { get; set; }
        object ExtraEventInfo { get; set; }
        object Receiver { get; set; }
        object Sender { get; set; }
    }
}
