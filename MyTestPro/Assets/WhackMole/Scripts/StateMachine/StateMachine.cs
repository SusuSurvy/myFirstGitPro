namespace WhackMole
{
	public class StateMachine<Actor_type>
	{
		private Actor_type m_pOwner;
		private State<Actor_type> m_pCurrentState;
		private State<Actor_type> m_pPreviousState;
		private State<Actor_type> m_pGlobalState;

		public StateMachine (Actor_type owner)
		{
			m_pOwner = owner;
			m_pCurrentState = null;
			m_pPreviousState = null;
			m_pGlobalState = null;
		}
	
		public void GlobalStateEnter ()
		{
			m_pGlobalState.Enter (m_pOwner);
		}
	
		public void SetGlobalStateState (State<Actor_type> GlobalState)
		{
			m_pGlobalState = GlobalState;
			m_pGlobalState.m_actor = m_pOwner;
			m_pGlobalState.Enter (m_pOwner);
		}
	
		public void SetCurrentState (State<Actor_type> CurrentState)
		{
         	m_pCurrentState = CurrentState;
            m_pCurrentState.m_actor = m_pOwner;
			m_pCurrentState.Enter (m_pOwner);
		
		}

		public void SMUpdate (float fTime)
		{
            //global state
			if (m_pGlobalState != null)
				m_pGlobalState.Execute (m_pOwner, fTime);
		
			//normal state
			if (m_pCurrentState != null)
				m_pCurrentState.Execute (m_pOwner, fTime);
		}

        /// <summary>
        /// 改变当前状态
        /// </summary>
        /// <param name="pNewState">将要变为的状态</param>
		public void ChangeState (State<Actor_type> pNewState)
		{
			if (pNewState == null) {
			
//			Debug.LogError ("this state is not exist!");
			}
            //首先退出原来状态
			m_pCurrentState.Exit (m_pOwner);
            //将当前状态设置为新状态的前一个状态
			m_pPreviousState = m_pCurrentState;
            //设置当前状态
			m_pCurrentState = pNewState;
            //为当前状态指定状态绑定的主角
            m_pCurrentState.m_actor = m_pOwner;
            //进入新状态
			m_pCurrentState.Enter (m_pOwner);

		}

        //恢复到以前状态
		public void RevertToPreviousState ()
		{
			ChangeState (m_pPreviousState);
		}

        
		public State<Actor_type> CurrentState ()
		{
			return m_pCurrentState;
		}

		public State<Actor_type> GlobalState ()
		{
			return m_pGlobalState;
		}

		public State<Actor_type> PreviousState ()
		{
			return m_pPreviousState;
		}

        //发送消息到当前的状态
        public bool HandleMessage(IStateEvent msg)
		{
			//the message
			if (m_pCurrentState != null && m_pCurrentState.OnMessage (m_pOwner, msg)) {
				return true;
			}

			// message to the global state
			if (m_pGlobalState != null && m_pGlobalState.OnMessage (m_pOwner, msg)) {
				return true;
			}
		
			return false;
		}
	
	}
}