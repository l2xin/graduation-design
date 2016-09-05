/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTView.cs
			// Describle: 视图
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:20:03
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
    public abstract class GTView : MonoBehaviour
    {
        protected GameObject mViewRoot;//当前视图的根节点
        protected GTModel mBaseModel;//当前模块的上下文信息
        protected GTModuleListenerControl mListenerControl;
        protected List<IViewListener> mListeners;
        public GameObject pViewRoot
        {
            get
            {
                if (mViewRoot == null)
                {
                    mViewRoot = gameObject;
                }
                if (mBaseModel != null)
                {
                    mBaseModel.pViewRoot = mViewRoot;
                }
                return mViewRoot;
            }
            set { mViewRoot = value; }
        }
        public GTModel pBaseModel
        {
            get { return mBaseModel; }
            set { mBaseModel = value; }
        }
        public GTView()
        {
            mListeners = new List<IViewListener>();
        }

        public virtual void OnInitView() { }

        public void AddListener(IViewListener varListener)
        {
            if (mListeners == null)
            {
                mListeners = new List<IViewListener>();
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
        public void RemoveListener(IViewListener varListener)
        {
            if (varListener == null || mListeners == null)
            {
                return;
            }
            mListeners.Remove(varListener);
        }

        public void NotifyListener(IViewListenerEventType varEvtType, params object[] varArgs)
        {
            if (mListeners == null || mListeners.Count == 0)
            {
                return;
            }
            for (int i = 0; i < mListeners.Count; i++)
            {
                IViewListener listener = mListeners[i];
                if (listener == null)
                {
                    continue;
                }
                switch (varEvtType)
                {
                    case IViewListenerEventType.OnViewInited:
                        listener.OnViewInited();
                        break;
                    case IViewListenerEventType.OnViewStart:
                        listener.OnViewStart();
                        break;
                    case IViewListenerEventType.OnViewEnable:
                        listener.OnViewEnable();
                        break;
                    case IViewListenerEventType.OnViewDisable:
                        listener.OnViewDisable();
                        break;
                    case IViewListenerEventType.OnViewDestroy:
                        listener.OnViewDestroy();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
