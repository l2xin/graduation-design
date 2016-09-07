/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: NetWorkEventManager.cs
			// Describle: Network event processor.
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
    public class NetworkEventProcessor : EventProcessor
    {
        private static NetworkEventProcessor m_Instance;

        /// <summary>
        /// Get a single instance of type 'NetworkEventProcessor'
        /// </summary>
        /// <returns></returns>
        public static NetworkEventProcessor GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new NetworkEventProcessor();
            }
            return m_Instance;
        }
        private NetworkEventProcessor() { }
    }

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

}
