using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace WhackMole
{
    public class StateBase<Actor_type> : State<Actor_type> where Actor_type : NpcActor
    {
        protected float m_time;
    }
}
