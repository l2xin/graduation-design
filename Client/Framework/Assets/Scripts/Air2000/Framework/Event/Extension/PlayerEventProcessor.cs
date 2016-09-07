/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: PlayerEventProcessor.cs
			// Describle:  Player event processor.
			// Created By:  hsu
			// Date&Time:  2016/1/19 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLogic;

namespace Air2000
{
    public class PlayerEventProcessor : EventProcessor
    {
        private Player m_Player;
        public Player Player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }
        public PlayerEventProcessor(Player player)
        {
            m_Player = player;
        }
    }

    public enum PlayerEventType
    {
        /// <summary>
        /// 倒计时结算.
        /// </summary>
        PE_Notify_BattleUI_CountDown,


        /// <summary>
        /// 掷骰子状态.
        /// </summary>
        PE_UI_Battle_Dice,

        /// <summary>
        /// 通知显示骰子数.
        /// </summary>
        PE_Notify_DiceNumber,

        /// <summary>
        /// 飞机选择.
        /// </summary>
        PE_UI_Battle_PlaneChoice,

        /// <summary>
        /// 飞机移动.
        /// </summary>
        PE_UI_Battle_PlaneChange,

        /// <summary>
        /// 飞机到准备位.
        /// </summary>
        PE_UI_Battle_PanelReady,

        ///// <summary>
        ///// 通知已经选择了飞机（关闭Ready按钮和人物描边）.
        ///// </summary>
        //PE_UI_Battle_NotifyChoicedPlane,

        /// <summary>
        /// 通知骰子结束
        /// </summary>
        PE_UI_NotifyDiceEnd,

        /// <summary>
        /// 通知创建飞机模型完成.
        /// </summary>
        PE_UI_NotifyCharacterEnd,

        /// <summary>
        /// 通知飞机动作结束.
        /// </summary>
        PE_UI_NotifyAirplaneTaskEnd,

        /// <summary>
        /// 通知Tag变化.
        /// </summary>
        PE_UI_NotifyTagChange,

        /// <summary>
        /// 通知积分变化.
        /// </summary>
        PE_UI_NotifyScoreChange,

        /// <summary>
        /// 通知飞机状态变化.
        /// </summary>
        PE_UI_NotifyPlaneStateChange,

        /// <summary>
        /// 通知托管变化.
        /// </summary>
        PE_UI_NotifyTrusteeshipChange,


        /// <summary>
        /// 游戏结束.
        /// </summary>
        PE_GameEnd,

        PE_SelfAirplaneMoveBegin,
        PE_SelfAirplaneMoveEnd,

        PE_UI_NotifySwitchViewButtonClick,

        PE_NotifyRankListFold,//排行版折叠
        PE_NotifyRankListUnfold,//排行版展开

        PE_UI_NotifyFriendMailInfoRsp,//好友邮件返回;
        PE_UI_NotifySystemMailInfoRsp,//系统邮件返回;
        PE_UI_NotifyFriendMailAdjunctRsp,//好友附件返回;
        PE_UI_NotifySystemMailAdjunctRsp,//好友附件返回;

        PE_NotifyMianWindowEnd,//通知主界面初始化完成;
        PE_NotifyMianModeEnd,//通知主界面模型完成;

        PE_NotifyAvatarInfoReturn,//角色详情返回;
        PE_NotifyChangeAvatar,  //角色出战;
        PE_NotifyAvatarCultivateUp, //角色属性升级;
        PE_NotifyAvatarSkillUp,		//角色技能升级;

        PE_NotifyAvatarNeedREQ,	//服务器通知角色列表改变;

        PE_NotifyShopGetAvatarID,

        PE_NotifyShopBuyAvatarSuccess, //广播商城购买角色成功;


        PE_NotifyUserLogout,			//用户注销;

        PE_NotifyEnterMainScene,    //通知进入主场景;

        PE_ColseNANotification,//公告界面关闭通知

        PE_NotifyVIPTypeChange,

        PE_NotifyInitializeMainAnimation,   //通知初始化主界面动画;

        PE_NotifyPlayMainAnimation, //通知播放主界面动画;

        PE_NotifyMissionProcess,		//任务进度变更;
        PE_NotifyMissionComplete,		//任务完成;
        PE_NotifyMissionAward,			//任务领奖;
        PE_NotifyMissionReturn,			//任务链返回;

        PE_NotifyPlayerInfoToUI,


        PE_NotifyOpenChoiceUI,
        PE_NotifyCloseChoiceUI,

        PE_NotifyChatInfo,


        PE_None,
    }

}
