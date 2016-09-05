/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: SceneMachine.cs
			// Describle: Manage our game scenes
			// Created By:  hsu
			// Date&Time:  2015/6/13 17:34:18
            // Modify History:
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Air2000;

namespace Air2000
{
    public class SceneMachine : StateMachine
    {
        private static SceneMachine m_Instance;
        public static SceneMachine GetSingleton()
        {
            if (m_Instance == null)
                m_Instance = new SceneMachine();
            return m_Instance;
        }
        public static GameScene GetLastScene()
        {
            return SceneMachine.GetSingleton().m_LastState as GameScene;
        }
        public static GameScene GetCurrentScene()
        {
            return SceneMachine.GetSingleton().GetCurrentState() as GameScene;
        }
    }
}