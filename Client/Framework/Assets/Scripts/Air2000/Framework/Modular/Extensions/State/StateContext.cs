using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Air2000.Modular
{
    public class StateContext : IterativeContextWrapper
    {
        private string m_StateName;//该状态的名称

      //  [@Inject(typeof(StateMachineContextWrapper))]
        protected StateMachineContextWrapper mStateMachine { get; set; }//所属的状态机实例
        public string pStateName
        {
            get { return m_StateName; }
            set { m_StateName = value; }
        }
        public StateMachineContextWrapper pStateMachine
        {
            get { return mStateMachine; }
            set { mStateMachine = value; }
        }
        public StateContext(string stateName) : base(stateName) { m_StateName = stateName; }
        public virtual void OnCreated() { }
        public virtual void Begin() { }
        public virtual void Update() { }
        public virtual void End() { }
        public virtual void OnDestroy() { }
    }
}
