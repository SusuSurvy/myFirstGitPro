using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackMole
{
    public class NpcStatePlay : StateBase<NpcActor>
    {
        private const float Playtime = 10f;
        public override void Enter(NpcActor owner)
        {
            base.Enter(owner);
        }

        public override void Execute(NpcActor owner, float fTimer)
        {
            base.Execute(owner, fTimer);
            m_time += fTimer;
            if (m_time > Playtime)
            {
                owner.RefreshWorkSate(WorkState.OwnerReturnWork);
                owner.StateMachine.ChangeState(new NpcStateWork());
            }
        }
    }

}

