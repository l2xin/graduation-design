/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GlobalEventManager.cs
			// Describle:程序中的全局事件转发
			// Created By:  hsu
			// Date&Time:  2016/1/19 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public class GlobalEventManager : EventManager
    {
        public static GlobalEventManager mInstance;

        public static GlobalEventManager GetSingleton()
        {
            if (mInstance == null)
            {
                mInstance = new GlobalEventManager();
            }
            return mInstance;
        }

        /// <summary>
        /// 广播程序发生的全局事件
        /// </summary>
        /// <param name="varEvent"></param>
        public void NotifyGlobalEvent(GlobalEvent varEvent)
        {
            if (varEvent == null)
            {
                return;
            }
            base.NotifyEvent((int)varEvent.pEventType, varEvent);
        }

        /// <summary>
        /// 注册程序的全局事件
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varDel"></param>
        public void RegisterGlobalEvent(GlobalEventType varType, EventFuntion varDel)
        {
            if (varDel != null)
            {
                base.RegisterMsgHandler((int)varType, varDel);
            }
        }

        /// <summary>
        /// 解注册程序的全局事件
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varDel"></param>
        public void UnRegisterGlobalEvent(GlobalEventType varType, EventFuntion varDel)
        {
            if (varDel != null)
            {
                base.UnRegisterMsgHandler((int)varType, varDel);
            }
        }

        public void UnRegisterAllEvent()
        {
            base.UnRegisterAllMsgHandlers();
        }
    }
}
