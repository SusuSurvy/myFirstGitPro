using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackMole
{
    public class NpcStateInternet : StateBase<NpcActor>
    {

        public override void Enter(NpcActor owner)
        {
            base.Enter(owner);
            owner.ChangeState(NpcState.Internet);
        }

       
        public override bool OnMessage(NpcActor owner, IStateEvent t)
        {
            switch (t.EventType)
            {
                case StateEventType.BossCome:
                    owner.StateMachine.ChangeState(new NpcStateWork());
                    return true;
               
            }

            return false;
        }
    }
}


