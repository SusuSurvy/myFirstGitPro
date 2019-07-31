using System.Collections;
using System.Collections.Generic;
using Framework.Common;
using UnityEngine;

namespace WhackMole
{
    public class NpcStateWork : StateBase<NpcActor>
    {
        private const float MiniPlayTime = 3.0f;

        private const float MaxPlayTime = 10.0f;

        private float m_seed;

        
        public override void Enter(NpcActor owner)
        {
            base.Enter(owner);
            owner.ChangeState(NpcState.Work);
            m_seed = RandomHelper.Range(MiniPlayTime, MaxPlayTime);
        }

        public override void Execute(NpcActor owner, float fTimer)
        {
            base.Execute(owner, fTimer);
            m_time += fTimer;
            if (m_time > m_seed)
            {
                TryToPlay(owner);
            }


        }

        private void TryToPlay(NpcActor owner)
        {
            int seed= RandomHelper.Range((int) NpcState.Sleep, (int) NpcState.Count);
            NpcState state = (NpcState) seed;
            switch (state)
            {
                case NpcState.Sleep:
                    owner.StateMachine.ChangeState(new NpcStateSleep());
                    break;
                case NpcState.PlayPhone:
                    owner.StateMachine.ChangeState(new NpcStatePlayPhone());
                    break;
                case NpcState.Internet://如果转变为上网状态时老板恰好在，就恢复到工作状态，重新计算
                    if (owner.IsBossShow)
                    {
                        m_time = 0;
                    }
                    else
                    {
                        owner.StateMachine.ChangeState(new NpcStateInternet());
                    }

                  
                    break;

            }

        }
    }

}

