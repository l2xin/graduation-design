using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000.Modular
{
    public class StateMachineContext : Context
    {
        protected Dictionary<string, StateContext> m_States;
        protected StateContext m_LastState;
        protected StateContext m_CurrentState;
        protected StateContext m_NextState;

        public StateMachineContext() : base()
        {
            m_States = new Dictionary<string, StateContext>();
            m_CurrentState = null;
            m_NextState = null;
        }

        public virtual void Update()
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.Update();
            }
            if (m_NextState != null)
            {
                if (m_CurrentState != null)
                {
                    m_CurrentState.End();
                }
                m_CurrentState = m_NextState;
                m_NextState = null;
                m_CurrentState.Begin();
            }
        }

        /// <summary>
        /// get state with statename
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public StateContext GetState(string stateName)
        {
            StateContext state = null;
            if (m_States.TryGetValue(stateName, out state))
            {
                return state;
            }
            return null;
        }

        /// <summary>
        /// register a new state to state machine
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool RegisterState(StateContext state)
        {
            if (state == null)
            {
                return false;
            }
            string stateName = state.pStateName;
            if (m_States.ContainsKey(stateName) == true)
            {
                return false;
            }
            state.pStateMachine = this;
            m_States.Add(stateName, state);
            return true;
        }

        /// <summary>
        /// unregister state with name
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public bool UnRegisterState(string stateName)
        {
            StateContext state = null;
            if (m_States.TryGetValue(stateName, out state) == false)
            {
                return false;
            }
            state.pStateMachine = null;
            m_States.Remove(stateName);
            return true;
        }

        /// <summary>
        /// change state
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        public virtual bool SetNextState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                return false;
            }
            StateContext state = GetState(stateName);
            if (state == null)
            {
                return false;
            }
            m_LastState = m_CurrentState;
            if (state == m_NextState)
            {
                return false;
            }
            m_NextState = state;
            return true;
        }

        /// <summary>
        /// get state that will be executed in next frame
        /// </summary>
        /// <returns></returns>
        public StateContext GetNextState()
        {
            return m_NextState;
        }

        /// <summary>
        /// get current executing state
        /// </summary>
        /// <returns></returns>
        public StateContext GetCurrentState()
        {
            return m_CurrentState;
        }

        /// <summary>
        /// get last executed state
        /// </summary>
        /// <returns></returns>
        public StateContext GetLastState()
        {
            return m_LastState;
        }
    }
}
