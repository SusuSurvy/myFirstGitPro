using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WhackMole
{
    public class NpcStateScare : StateBase<NpcActor>
    {
        
        public override void Enter(NpcActor owner)
        {
            base.Enter(owner);
            owner.ChangeState(NpcState.Scare);
        }

        public override void Execute(NpcActor owner, float fTimer)
        {
            base.Execute(owner, fTimer);
            m_time += fTimer;
            if (m_time > 6)
            {
                owner.StateMachine.ChangeState(new NpcStateWork());
            }
        }
    }
}

