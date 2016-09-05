/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: SceneEventManager.cs
			// Describle: 场景事件
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
    public class SceneEventManager : EventManager
    {
        private GameScene mScene;
        public GameScene pScene
        {
            get { return mScene; }
            set { mScene = value; }
        }
        public SceneEventManager(GameScene varScene)
        {
            mScene = varScene;
        }

        public void NotifySceneEvent(SceneEvent varEvent)
        {
            if (varEvent == null)
            {
                return;
            }
            base.NotifyEvent((int)varEvent.pEventType, varEvent);
        }

        public void RegisterSceneEvent(SceneEventType varType, EventFuntion varDel)
        {
            if (varDel != null)
            {
                base.RegisterMsgHandler((int)varType, varDel);
            }
        }
        public void UnRegisterSceneEvent(SceneEventType varType, EventFuntion varDel)
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
