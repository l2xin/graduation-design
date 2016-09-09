/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTModel.cs
			// Describle: 模型
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:11:09
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Air2000
{
    /// <summary>
    /// 根据不同的状态监听不同的广播，移除不必要的监听
    /// </summary>
    public enum ModelState
    {
        WatingInited,
        Background,
        Active,
        InActive,
        WatingDestroy,
    }
    public abstract class GTModel
    {
        protected int mModelID;
        protected ModelState mLastModelState;
        protected ModelState mModelState;
        protected GTPlayer mSelfPlayer;
        protected GameObject mViewRoot;
        protected GTView mView;
        protected GTModuleListenerControl mListenerControl;
        protected List<IModelListener> mListeners;
        public GameObject pViewRoot
        {
            get
            {
                if (mViewRoot == null && mView != null)
                {
                    mViewRoot = mView.gameObject;
                }
                return mViewRoot;
            }
            set { mViewRoot = value; }
        }
        public ModelState pLastModelState
        {
            get { return mLastModelState; }
            set { mLastModelState = value; }
        }
        public ModelState pModelState
        {
            get { return mModelState; }
            set { mModelState = value; }
        }

        public GTModel()
        {
            mListeners = new List<IModelListener>();
        }
        public virtual void OnModuleInit() { }
        public virtual void Destroy() { NotifyListener(IModelListenerEventType.OnModelDestroyed); }
        public void ChangeModelState(ModelState varState) { mModelState = varState; NotifyListener(IModelListenerEventType.OnModelStateChanged); }

        public void AddListener(IModelListener varListener)
        {
            if (mListeners == null)
            {
                mListeners = new List<IModelListener>();
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
        public void RemoveListener(IModelListener varListener)
        {
            if (varListener == null || mListeners == null)
            {
                return;
            }
            mListeners.Remove(varListener);
        }
        public void NotifyListener(IModelListenerEventType varEvtType, params object[] varArgs)
        {
            if (mListeners == null || mListeners.Count == 0)
            {
                return;
            }
            for (int i = 0; i < mListeners.Count; i++)
            {
                IModelListener listener = mListeners[i];
                if (listener == null)
                {
                    continue;
                }
                switch (varEvtType)
                {
                    case IModelListenerEventType.OnModelInited:
                        listener.OnModelInited();
                        break;
                    case IModelListenerEventType.OnViewAssigned:
                        if (varArgs.Length > 0)
                            listener.OnViewAssigned(varArgs[0] as GTView);
                        else
                            listener.OnViewAssigned(null);
                        break;
                    case IModelListenerEventType.OnModelDestroyed:
                        listener.OnModelDestroyed();
                        break;
                    case IModelListenerEventType.OnModelStateChanged:
                        listener.OnModelStateChanged(mModelState);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
