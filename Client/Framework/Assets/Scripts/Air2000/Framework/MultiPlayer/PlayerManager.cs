/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: PlayerManager.cs
			// Describle: Manage all players.
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:04:49
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public class PlayerManager
    {
        private static PlayerManager m_Instance;
        private PlayerManager() { }
        public PlayerManager GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new PlayerManager();
            }
            return m_Instance;
        }
    }
}
