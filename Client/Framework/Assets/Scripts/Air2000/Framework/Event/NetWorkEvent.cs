	/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: NetWorkEvent.cs
			// Describle:网络消息
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
    public enum NetWorkEventType
    {
        NE_LoginSuccess,
        NE_LoginFailed,
        NE_NotifyLoginInfo,
        NE_NotifySelfInfo,
        NE_NotifyBeanAndDiamond,
        NE_EnterRoomSuccess,
        NE_EnterRoomFailed,
        NE_EnterRoomLocked,
        NE_LeaveRoom,
        NE_SitDownInfo,
        NE_UserEnter,
        NE_UserExit,
        NE_HandsUp,
        NE_GameMessage,
        NE_GameRanking,
        NE_BuyBean,
        NE_SendBean,
        NE_NotifyOffLine, 
        NE_NoticeMessage,
        NE_NoticeAwardInfo,
        NE_PublicAnnoument,
        NE_NotifyComplete,
        NE_NotifyProgress,
        NE_NotifyNetworkChange,
		NE_NotifyMailNum,
		NE_NotifyMailInfo,
		NE_NotifyMailOperate,
        NE_NotifyIsGivenDonner,
		NE_NotifyMailChange,
		NE_NotifyMailFriendInfo,
		/// 通知网络连接状态.
        NE_NotifyNetConnect,
		NE_NotifyMailFriendAdjunct,
		NE_NotifyMailAllFriendAdjunct,
        NE_NotigyDimongdBugProp,
        NE_NotifyPlayerInfo,
		NE_NotifyMsgChange,
		NE_NotifySystemAdjunct,
        NE_NotifyOnlineNumber,
		NE_NotifyAvatarInfo,
		NE_NotifyChangeAvatar,
		NE_NotifyClientEXECommond,
        NE_NotifyRedChangePoint,
        
		NE_NotifyAvatarCultivate,
		NE_NotifyAvatarSkill,

        NE_NotifyUpdateVersion,
        NE_NotifyBeacon,

		NE_NotifySignatureBook,
		NE_NotifySignatureMission,


		NE_NotifyAllMissionInfo,
		NE_NotifyMissionAward,
        NE_NotifyNetworkStateInfo,
        NE_NotiftConfigChange,

        NE_NotifyChatInfo,
        NE_NotifyPhysicalNumber,
    }

    public class NetWorkEvent : BaseEvent
    {
        public NetWorkEventType mEventType;

        public NetWorkEventType pEventType
        {
            get { return mEventType; }
        }

        public NetWorkEvent(NetWorkEventType evtType)
        {
            mEventType = evtType;
            pEventID = (int)evtType;
        }

    }

    public class NetWorkEventEx<T> : NetWorkEvent
    {
        public T mParam;
        public NetWorkEventEx(NetWorkEventType evtType, T param)
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

    public class NetWorkEventQueue
    {
        public Dictionary<NetWorkEventType, List<EventFuntion>> mEvents;

        public NetWorkEventQueue()
        {
            mEvents = new Dictionary<NetWorkEventType, List<EventFuntion>>();
        }

        public void AddEvent(NetWorkEventType varType, EventFuntion varDel)
        {
            if (varDel == null)
            {
                return;
            }
            if (mEvents == null)
            {
                mEvents = new Dictionary<NetWorkEventType, List<EventFuntion>>();
            }
            List<EventFuntion> tmpDels = null;
            if (mEvents.TryGetValue(varType, out tmpDels) == false)
            {
                tmpDels = new List<EventFuntion>();
                tmpDels.Add(varDel);
                mEvents.Add(varType, tmpDels);
                NetWorkEventManager.GetSingleton().RegisterNetWorkEvent(varType, varDel);
            }
            else
            {
                if (tmpDels.Contains(varDel) == false)
                {
                    tmpDels.Add(varDel);
                    NetWorkEventManager.GetSingleton().RegisterNetWorkEvent(varType, varDel);
                }
            }
        }

        public void RemoveEvent(NetWorkEventType varType, EventFuntion varDel)
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
                NetWorkEventManager.GetSingleton().UnRegisterNetWorkEvent(varType, varDel);
            }
        }

        public void RemoveAllEvent()
        {
            if (mEvents == null || mEvents.Count == 0)
            {
                return;
            }
            Dictionary<NetWorkEventType, List<EventFuntion>>.Enumerator em = mEvents.GetEnumerator();
            for (int i = 0; i < mEvents.Count; i++)
            {
                em.MoveNext();
                KeyValuePair<NetWorkEventType, List<EventFuntion>> kvp = em.Current;
                if (kvp.Value == null || kvp.Value.Count == 0)
                {
                    continue;
                }
                for (int j = 0; j < kvp.Value.Count; j++)
                {
                    NetWorkEventManager.GetSingleton().UnRegisterNetWorkEvent(kvp.Key, kvp.Value[j]);
                }
            }
            mEvents.Clear();
        }
    }
}
