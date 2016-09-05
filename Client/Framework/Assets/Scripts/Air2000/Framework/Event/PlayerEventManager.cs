/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: PlayerEventManager.cs
			// Describle:角色事件
			// Created By:  hsu
			// Date&Time:  2016/1/19 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLogic;

namespace Air2000
{
    public class PlayerEventManager : EventManager
    {
        private Player mPlayer;
        public Player pPlayer
        {
            get { return mPlayer; }
            set { mPlayer = value; }
        }
        public PlayerEventManager(Player varPlayer)
        {
            mPlayer = varPlayer;
        }

        public void NotifyPlayerEvent(PlayerEvent varEvent)
        {
            if (varEvent == null)
            {
                return;
            }
            base.NotifyEvent((int)varEvent.pEventType, varEvent);
        }

        public void RegisterPlayerEvent(PlayerEventType varType, EventFuntion varDel)
        {
            if (varDel != null)
            {
                base.RegisterMsgHandler((int)varType, varDel);
            }
        }
        public void UnRegisterPlayerEvent(PlayerEventType varType, EventFuntion varDel)
        {
            if (varDel != null)
            {
                base.UnRegisterMsgHandler((int)varType, varDel);
            }
        }

        public void UnRegisterAllEvents()
        {
            base.UnRegisterAllMsgHandlers();
        }
    }
}
