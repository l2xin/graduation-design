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
    public abstract class GTModuleListenerControl : IModelListener, IViewListener
    {
        protected GTView mView;
        protected GTModel mModel;
        protected ApplicationEventProcessor mGlobalEventManager;
        protected NetworkEventProcessor mNetMsgEventManager;
        protected EventHandlerQueue mGlobalMsgQueue;
        protected EventHandlerQueue mNetMsgQueue;

        public ApplicationEventProcessor pGlobalEventManager
        {
            get { return mGlobalEventManager; }
        }
        public NetworkEventProcessor pNetMsgEventManager
        {
            get { return mNetMsgEventManager; }
        }
        public EventHandlerQueue pGlobalMsgQueue
        {
            get { return mGlobalMsgQueue; }
            set { mGlobalMsgQueue = value; }
        }
        public EventHandlerQueue pNetMsgQueue
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
            mGlobalMsgQueue = new EventHandlerQueue(ApplicationEventProcessor.GetInstance());
            mNetMsgQueue = new EventHandlerQueue(NetworkEventProcessor.GetInstance());
            mGlobalEventManager = ApplicationEventProcessor.GetInstance();
            mNetMsgEventManager = NetworkEventProcessor.GetInstance();
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
                mNetMsgQueue.RemoveAll();
            }
            if (mGlobalMsgQueue != null)
            {
                mGlobalMsgQueue.RemoveAll();
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
    }
}
