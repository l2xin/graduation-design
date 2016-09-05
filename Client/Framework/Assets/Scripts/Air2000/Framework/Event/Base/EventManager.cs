/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: EventManager.cs
			// Describle:事件处理
			// Created By:  hsu
			// Date&Time:  2016/1/19 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace Air2000
{
    public delegate void EventFuntion(BaseEvent varEvent);

    public abstract class EventManager
    {
        List<EventFuntion> mDeleteMsgHandlers;

        /// <summary>
        /// 所有注册的消息列表，
        /// </summary>
        private Dictionary<int, List<EventFuntion>> mMsgHandlers;

        public EventManager()
        {
            mMsgHandlers = new Dictionary<int, List<EventFuntion>>();
            mDeleteMsgHandlers = new List<EventFuntion>();
        }

        /// <summary>
        /// 获取所有以注册事件的个数
        /// </summary>
        /// <returns></returns>
        public int GetRegisterEventCount()
        {
            return mMsgHandlers.Count();
        }

        /// <summary>
        /// 获取某一事件的监听者个数
        /// </summary>
        /// <param name="varId"></param>
        /// <returns></returns>
        public int GetRegisterEventCountById(int varMsgID)
        {
            if (mMsgHandlers == null || mMsgHandlers.Count == 0)
            {
                return -1;
            }
            List<EventFuntion> tmpFuns = null;
            if (mMsgHandlers.TryGetValue(varMsgID, out tmpFuns))
            {
                return tmpFuns.Count;
            }
            return 0;
        }

        /// <summary>
        /// 事件广播
        /// </summary>
        /// <param name="varMsgID"></param>
        /// <param name="varEvent"></param>
        public void NotifyEvent(int varMsgID, BaseEvent varEvent)
        {
            List<EventFuntion> tmpFuncs = null;
            if (mMsgHandlers.TryGetValue(varMsgID, out tmpFuncs))
            {
                if (mMsgHandlers.Count != 0 && tmpFuncs != null)
                {
                    for (int i = 0; i < mDeleteMsgHandlers.Count; i++)
                    {
                        EventFuntion temRem = mDeleteMsgHandlers[i];
                        if (tmpFuncs.Contains(temRem))
                        {
                            tmpFuncs.Remove(temRem);
                            mDeleteMsgHandlers.RemoveAt(i);
                            i--;
                        }
                    }
                    //mDeleteMsgHandlers.Clear();
                }
                if (tmpFuncs != null && tmpFuncs.Count > 0)
                {
                    for (int i = 0; i < tmpFuncs.Count; i++)
                    {
                        EventFuntion tmpFunc = tmpFuncs[i];
                        if (tmpFunc == null || tmpFunc.Target == null)
                        {
                            mDeleteMsgHandlers.Add(tmpFunc);
                        }
                        else
                        {
                            tmpFunc(varEvent);
                        }
                    }
                    //mDeleteMsgHandlers.Clear();
                    //for (int i = 0; i < mDeleteMsgHandlers.Count; i++)
                    //{
                    //    EventFuntion temRem = mDeleteMsgHandlers[i];
                    //    if (tmpFuncs.Contains(temRem))
                    //    {
                    //        tmpFuncs.Remove(temRem);
                    //        mDeleteMsgHandlers.RemoveAt(i);
                    //        i--;
                    //    }
                    //}
                    //mDeleteMsgHandlers.Clear();
                }
            }
        }


        /// <summary>
        /// 注册消息句柄
        /// </summary>
        /// <param name="varMsgID"></param>
        /// <param name="varFunc"></param>
        public void RegisterMsgHandler(int varMsgID, EventFuntion varFunc)
        {
            if (varFunc == null)
            {
                return;
            }
            if (mMsgHandlers == null)
            {
                mMsgHandlers = new Dictionary<int, List<EventFuntion>>();
            }
            List<EventFuntion> tmpFuns = null;
            if (mMsgHandlers.TryGetValue((int)varMsgID, out tmpFuns) == false)
            {
                tmpFuns = new List<EventFuntion>();
                mMsgHandlers.Add((int)varMsgID, tmpFuns);
            }
			else
			{
				if (mMsgHandlers.Count != 0 && tmpFuns != null)
				{
					for (int i = 0; i < mDeleteMsgHandlers.Count; i++)
					{
						EventFuntion temRem = mDeleteMsgHandlers[i];
						if (tmpFuns.Contains(temRem))
						{
							tmpFuns.Remove(temRem);
							mDeleteMsgHandlers.RemoveAt(i);
							i--;
						}
					}
				}
			}
            for (int i = 0; i < tmpFuns.Count; i++)
            {
                EventFuntion func = tmpFuns[i];
                if (func == varFunc)
                {
                    return;
                }
            }
            tmpFuns.Add(varFunc);
        }

        /// <summary>
        /// 移除已经注册的消息句柄
        /// </summary>
        /// <param name="varMsgID"></param>
        /// <param name="varFunc"></param>
        public void UnRegisterMsgHandler(int varMsgID, EventFuntion varFunc)
        {
            List<EventFuntion> tmpFuns = null;
            if (mMsgHandlers.TryGetValue(varMsgID, out tmpFuns))
            {
                if (tmpFuns != null && tmpFuns.Count > 0)
                {
                    for (int i = tmpFuns.Count - 1; i >= 0; --i)
                    {
                        EventFuntion tmpFun = tmpFuns[i];
                        if (tmpFun == null || tmpFun.Target == null)
                        {
                            mDeleteMsgHandlers.Add(tmpFun);
                            //tmpFuns.Remove(tmpFun);
                            continue;
                        }
                        if (varFunc != null && tmpFun.Target == varFunc.Target)
                        {
                            mDeleteMsgHandlers.Add(tmpFun);
                            //tmpFuns.Remove(tmpFun);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除所有已注册的消息
        /// </summary>
        public void UnRegisterAllMsgHandlers()
        {
            if (mMsgHandlers != null)
            {
                mMsgHandlers.Clear();
            }
        }
    }
}
