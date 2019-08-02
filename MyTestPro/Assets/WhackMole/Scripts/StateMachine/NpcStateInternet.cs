using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackMole
{
    public class NpcStateInternet : NpcStatePlay
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
               owner.RefreshWorkSate(WorkState.HitToWork);

                    return true;
               
            }

            return false;
        }
    }
}


