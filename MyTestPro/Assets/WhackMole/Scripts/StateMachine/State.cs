
namespace WhackMole
{
	public class State<Actor_type>
	{
        /// <summary>
        /// 角色的类型 主角 敌人
        /// </summary>
		public Actor_type m_actor ;

        /// <summary>
        /// 进入该状态之前执行
        /// </summary>
        /// <param name="actorType"></param>
		public virtual void Enter (Actor_type owner)
		{
		}
	    
        /// <summary>
        /// 进入该状态之后执行，
        /// </summary>
        /// <param name="actorType"></param>
		public virtual void Execute (Actor_type owner, float fTimer)
		{
		
		}

        /// <summary>
        /// 进入下一个状态之前执行
        /// </summary>
        /// <param name="actorType"></param>
		public virtual void Exit (Actor_type owner)
		{
		
		}
	    
        /// <summary>
        /// 该状态接收到某个消息，子类具体实现
        /// </summary>
        /// <param name="actorType"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool OnMessage(Actor_type owner, IStateEvent t)
		{
			return false;
		}
	}
}
