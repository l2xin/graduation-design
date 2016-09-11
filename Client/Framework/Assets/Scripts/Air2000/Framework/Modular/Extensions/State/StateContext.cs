using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Air2000.Modular
{
    public class StateContext : Context
    {
        private string m_StateName;//该状态的名称
        protected StateMachineContext mStateMachine;//所属的状态机实例
        public string pStateName
        {
            get { return m_StateName; }
            set { m_StateName = value; }
        }
        public StateMachineContext pStateMachine
        {
            get { return mStateMachine; }
            set { mStateMachine = value; }
        }
        public StateContext(string stateName) { m_StateName = stateName; }
        public virtual void OnCreated() { }
        public virtual void Begin() { }
        public virtual void Update() { }
        public virtual void End() { }
        public virtual void OnDestroy() { }
    }
}
