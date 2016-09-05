/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: INetworkManagerListener.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/3 21:43:55
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public enum INetworkManagerListenerEventType
    {
        OnNetworkChanged,
        OnNetworkDisable,
        OnConnectedServer,
    }
    public interface INetworkManagerListener
    {
        void OnNetworkChanged();
        void OnNetworkDisable();
        void OnConnectedServer();
    }
}
