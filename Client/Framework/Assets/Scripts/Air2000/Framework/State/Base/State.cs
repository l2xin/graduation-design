/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: State.cs
			// Describle: 状态基类
			// Created By:  hsu
			// Date&Time:  2016/1/25 17:35:33
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public abstract class State
    {
        private string mStateName;//该状态的名称
        protected StateMachine mStateMachine;//所属的状态机实例
        public string pStateName
        {
            get { return mStateName; }
            set { mStateName = value; }
        }
        public StateMachine pStateMachine
        {
            get { return mStateMachine; }
            set { mStateMachine = value; }
        }
        public State(string stateName) { mStateName = stateName; }
        public virtual void OnCreated() { }
        public virtual void Begin() { }
        public virtual void Update() { }
        public virtual void End() { }
        public virtual void OnDestroy() { }
    }
}
