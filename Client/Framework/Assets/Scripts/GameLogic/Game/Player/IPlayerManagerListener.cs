/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: IPlayerManagerListener.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/3 21:37:44
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000;

namespace GameLogic
{
    public enum IPlayerManagerListenerEventType
    {
        OnPlayerLogin = 1,
        OnPlayerLogout = 2,
        OnPlayerOffline = 3,
    }
    public interface IPlayerManagerListener
    {
        void OnPlayerLogin(Player varPlayer);
        void OnPlayerLogout(Player varPlayer);
        void OnPlayerOffline(Player varPlayer);
    }
}
