/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTListenerControl.cs
			// Describle:  模块控制
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:24:39
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public abstract class GTModuleListenerControl : INetworkManagerListener, IModelListener, IViewListener
    {
        protected GTView mView;
        protected GTModel mModel;
        protected GlobalEventManager mGlobalEventManager;
        protected NetWorkEventManager mNetMsgEventManager;
        protected GlobalEventQueue mGlobalMsgQueue;
        protected NetWorkEventQueue mNetMsgQueue;

        public GlobalEventManager pGlobalEventManager
        {
            get { return mGlobalEventManager; }
        }
        public NetWorkEventManager pNetMsgEventManager
        {
            get { return mNetMsgEventManager; }
        }
        public GlobalEventQueue pGlobalMsgQueue
        {
            get { return mGlobalMsgQueue; }
            set { mGlobalMsgQueue = value; }
        }
        public NetWorkEventQueue pNetMsgQueue
        {
            get { return mNetMsgQueue; }
            set { mNetMsgQueue = value; }
        }
        //public GTModuleListenerControl()
        //{
        //    mGlobalEventManager = GlobalEventManager.GetSingleton();
        //    mNetMsgEventManager = NetWorkEventManager.GetSingleton();
        //}
        public GTModuleListenerControl(GTModel varModel)
        {
            mModel = varModel;
            if (varModel != null)
            {
                varModel.AddListener(this);
            }
            mGlobalMsgQueue = new GlobalEventQueue();
            mNetMsgQueue = new NetWorkEventQueue();
            mGlobalEventManager = GlobalEventManager.GetSingleton();
            mNetMsgEventManager = NetWorkEventManager.GetSingleton();
        }

        #region Implement for IModelListener
        public virtual void OnModelInited() { }
        public virtual void OnViewAssigned(GTView varView)
        {
            if (varView == null)
            {
                return;
            }
            if (mView != null)
            {
                varView.RemoveListener(this);
            }
            mView = varView;
        }
        public virtual void OnModelDestroyed()
        {
            if (mNetMsgEventManager != null)
            {
                mNetMsgEventManager.RemoveListener(this);
            }
            if (mModel != null)
            {
                mModel.RemoveListener(this);
            }
            if (mView != null)
            {
                mView.RemoveListener(this);
            }
            if (mNetMsgQueue != null)
            {
                mNetMsgQueue.RemoveAllEvent();
            }
            if (mGlobalMsgQueue != null)
            {
                mGlobalMsgQueue.RemoveAllEvent();
            }
        }
        public virtual void OnModelStateChanged(ModelState varModelState) { }
        #endregion

        #region Implement for IViewListener
        public virtual void OnViewInited() { }
        public virtual void OnViewStart() { }
        public virtual void OnViewEnable() { }
        public virtual void OnViewDisable() { }
        public virtual void OnViewDestroy() { }
        #endregion

        #region Implement for INetworkManagerListener
        public virtual void OnNetworkChanged() { }
        public virtual void OnNetworkDisable() { }
        public virtual void OnConnectedServer() { }
        #endregion
    }
}
