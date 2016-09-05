/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GlobalEvent.cs
			// Describle:全局事件
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
    public enum GlobalEventType
    {
        GE_HeroChange,//角色发生切换
        GE_SceneBegin, //场景开始
        GE_LevelWasLoaded, //关卡加载完成
        GE_SceneWasLoaded, //场景加载完成
        GE_SceneEnd, //场景结束

        /// <summary>
        /// 主界面蒙板打开/关闭
        /// </summary>
        GE_SetMainMask,

		GE_ClientCommond,

		GE_NetWorkState,

        GE_EditorCreateCharacter,
    }

    public class GlobalEvent : BaseEvent
    {
        public GlobalEventType mEventType;

        public GlobalEventType pEventType
        {
            get { return mEventType; }
        }

        public GlobalEvent(GlobalEventType evtType)
        {
            mEventType = evtType;
            pEventID = (int)evtType;
        }

    }

    public class GlobalEventEx<T> : GlobalEvent
    {
        public T mParam;

        public GlobalEventEx(GlobalEventType evtType, T param)
            : base(evtType)
        {
            mEventType = evtType;
            mParam = param;
            pEventID = (int)evtType;
        }

        public T GetData()
        {
            return mParam;
        }
    }

    public class GlobalEventQueue
    {
        public Dictionary<GlobalEventType, List<EventFuntion>> mEvents;

        public GlobalEventQueue()
        {
            mEvents = new Dictionary<GlobalEventType, List<EventFuntion>>();
        }

        public void AddEvent(GlobalEventType varType, EventFuntion varDel)
        {
            if (varDel == null)
            {
                return;
            }
            if (mEvents == null)
            {
                mEvents = new Dictionary<GlobalEventType, List<EventFuntion>>();
            }
            List<EventFuntion> tmpDels = null;
            if (mEvents.TryGetValue(varType, out tmpDels) == false)
            {
                tmpDels = new List<EventFuntion>();
                tmpDels.Add(varDel);
                mEvents.Add(varType, tmpDels);
                GlobalEventManager.GetSingleton().RegisterGlobalEvent(varType, varDel);
            }
            else
            {
                if (tmpDels.Contains(varDel) == false)
                {
                    tmpDels.Add(varDel);
                    GlobalEventManager.GetSingleton().RegisterGlobalEvent(varType, varDel);
                }
            }
        }

        public void RemoveEvent(GlobalEventType varType, EventFuntion varDel)
        {
            if (varDel == null)
            {
                return;
            }
            if (mEvents == null || mEvents.Count == 0)
            {
                return;
            }
            List<EventFuntion> tmpDels = null;
            if (mEvents.TryGetValue(varType, out tmpDels))
            {
                tmpDels.Clear();
                mEvents.Remove(varType);
                GlobalEventManager.GetSingleton().UnRegisterGlobalEvent(varType, varDel);
            }
        }

        public void RemoveAllEvent()
        {
            if (mEvents == null || mEvents.Count == 0)
            {
                return;
            }
            Dictionary<GlobalEventType, List<EventFuntion>>.Enumerator em = mEvents.GetEnumerator();
            for (int i = 0; i < mEvents.Count; i++)
            {
                em.MoveNext();
                KeyValuePair<GlobalEventType, List<EventFuntion>> kvp = em.Current;
                if (kvp.Value == null || kvp.Value.Count == 0)
                {
                    continue;
                }
                for (int j = 0; j < kvp.Value.Count; j++)
                {
                    GlobalEventManager.GetSingleton().UnRegisterGlobalEvent(kvp.Key, kvp.Value[j]);
                }
            }
            mEvents.Clear();
        }
    }
}
