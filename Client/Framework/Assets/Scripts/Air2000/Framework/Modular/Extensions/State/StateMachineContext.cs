using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000.Modular
{
    public class StateMachineContextWrapper : IterativeContextWrapper
    {
        protected StateContext m_LastState;
        protected StateContext m_CurrentState;
        protected StateContext m_NextState;

        public StateMachineContextWrapper(string identifyName) : base(identifyName)
        {
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


        ///// <summary>
        ///// change state
        ///// </summary>
        ///// <param name="stateName"></param>
        ///// <returns></returns>
        //public virtual bool SetNextState(string stateName)
        //{
        //    if (string.IsNullOrEmpty(stateName))
        //    {
        //        return false;
        //    }
        //    StateContext state = GetState(stateName);
        //    if (state == null)
        //    {
        //        return false;
        //    }
        //    m_LastState = m_CurrentState;
        //    if (state == m_NextState)
        //    {
        //        return false;
        //    }
        //    m_NextState = state;
        //    return true;
        //}

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
