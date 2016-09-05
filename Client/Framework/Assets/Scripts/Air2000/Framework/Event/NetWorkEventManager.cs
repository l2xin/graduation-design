/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: NetWorkEventManager.cs
			// Describle:游戏网络消息的收发
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
    public class NetWorkEventManager : EventManager
    {
        private static NetWorkEventManager mInstance;
        private List<INetworkManagerListener> mListeners;

        private NetWorkEventManager()
        {
            mListeners = new List<INetworkManagerListener>();
        }

        public static NetWorkEventManager GetSingleton()
        {
            if (mInstance == null)
            {
                mInstance = new NetWorkEventManager();
            }
            return mInstance;
        }

        /// <summary>
        /// 广播网络事件
        /// </summary>
        /// <param name="varEvent"></param>
        public void NotifyNetWorkEvent(NetWorkEvent varEvent)
        {
            if (varEvent == null)
            {
                return;
            }
            base.NotifyEvent((int)varEvent.pEventType, varEvent);
        }

        /// <summary>
        /// 注册网络事件
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varDel"></param>
        public void RegisterNetWorkEvent(NetWorkEventType varType, EventFuntion varDel)
        {
            if (varDel != null)
            {
                base.RegisterMsgHandler((int)varType, varDel);
            }
        }

        /// <summary>
        /// 解注册网络事件
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varDel"></param>
        public void UnRegisterNetWorkEvent(NetWorkEventType varType, EventFuntion varDel)
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

        public void SendMsgToServer(object varParam)
        {

        }

        public void AddListener(INetworkManagerListener varListener)
        {
            if (mListeners == null)
            {
                mListeners = new List<INetworkManagerListener>();
            }
            if (varListener == null)
            {
                return;
            }
            if (mListeners.Contains(varListener))
            {
                return;
            }
            mListeners.Add(varListener);
        }
        public void RemoveListener(INetworkManagerListener varListener)
        {
            if (varListener == null || mListeners == null)
            {
                return;
            }
            mListeners.Remove(varListener);
        }
        public void NotifyListener(INetworkManagerListenerEventType varEvtType, params object[] varArgs)
        {
            if (mListeners == null || mListeners.Count == 0)
            {
                return;
            }
            for (int i = 0; i < mListeners.Count; i++)
            {
                INetworkManagerListener listener = mListeners[i];
                if (listener == null)
                {
                    continue;
                }
                switch (varEvtType)
                {
                    case INetworkManagerListenerEventType.OnNetworkChanged:
                        listener.OnNetworkChanged();
                        break;
                    case INetworkManagerListenerEventType.OnNetworkDisable:
                        listener.OnNetworkDisable();
                        break;
                    case INetworkManagerListenerEventType.OnConnectedServer:
                        listener.OnConnectedServer();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
