/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: IViewListener.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/4 8:55:08
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public enum IViewListenerEventType
    {
        /// <summary>
        /// 该事件在BaseView 的Awake方法执行之后被广播
        /// </summary>
        OnViewInited = 1,
        OnViewStart = 2,
        OnViewEnable = 3,
        OnViewDisable = 4,
        /// <summary>
        /// 该事件在BaseView 的OnDestroy方法执行之后被广播
        /// </summary>
        OnViewDestroy = 5,
    }
    public interface IViewListener
    {
        void OnViewInited();
        void OnViewStart();
        void OnViewEnable();
        void OnViewDisable();
        void OnViewDestroy();
    }
}
