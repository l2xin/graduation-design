/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: 登录模块.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/2/1 15:43:22
            // Modify History:John 2016年6月1日 15:51:59
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Air2000;
using System.Runtime.InteropServices;
using System;
using Air2000.UI;

namespace GameLogic
{
    public enum FristCommondClientType
    {
        ///重新登录;
        ReLogin = 100,
        ///登出;
        LogOut = 101,
        ///显示提示;
        ShowTip = 102,
        ///系统信息(系统时间等);
        SysInfo = 200,
    }

    public enum CommondFristDetailType
    {
        ///签名校验失败;
        VerifyFalled = 10000,
        ///多设备登录-执行动作:登出;
        LogOut_MultiDevice = 10100,
        ///多设备登录-执行动作:显示提示;
        ShowTip_MultiDevice = 10200,
    }

    public enum CommondSecDetailType
    {
        ///签名ST中的accounttype错误;
        VerifyFalled_Account = 1,
        ///签名ST中Uin错误;
        VerifyFalled_Uin,
        ///签名ST过期;
        VerifyFalled_Expired,
    }

    ///客户端被拉起的状态;
    public enum LoginExt
    {
        ///玩家正常拉起;
        Normal = 0,
        ///微信游戏中心拉起;
        WXGameCenter_Share = 10000,
        ///QQ游戏中心拉起;
        QQGameCenter_Share = 10001,
        ///QQ客户端拉起;
        QQGameCenter_ClientShare,
        ///微信客户端拉起;
        WXGameCenter_ClientShare,
    }

    public class LoginModel : BaseModel
    {

        private enum ErrMsgFromt
        {
            Formt_UTF8 = 1,
            Formt_GBK,
        }

        #region 登录相关参数
        public static string openId = string.Empty;
        public static string openKey = string.Empty;
        public static string pf = string.Empty;
        public static string pfKey = string.Empty;
        public static string accessToken = string.Empty;
        public static string qqPayToken = string.Empty;
        /// 帐号
        public static ulong uin = 0;
        /// 帐号类型
        public static sbyte loginType = -1;
        /// 游戏ID
        public static int gameid = 399;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /// 客户端类型
        public static readonly int clienttype = 32;
        public static readonly int SupportVersion = 11;
#elif  UNITY_ANDROID
        /// 客户端类型
        public static readonly int clienttype = 64;
		public static readonly int SupportVersion = 11;
#elif UNITY_IOS
		/// 客户端类型
		public static readonly int clienttype = 13;
		public static readonly int SupportVersion = 27;
#endif
        #endregion

        public LoginModel()
            : base(ModelType.MT_Login, typeof(LoginListenerControl))
        {
        }

        private NetWorkEventQueue mNetMsgQueue;
        private PlayerEventQueue mPlayerEventQueue;
        private GameScene mScene;

        private bool mIsHandleWakeUpInfo = false;
        //        private VPTimer.Handle mLoginTimeoutHandler;
        public override void OnModuleInit()
        {
            base.OnModuleInit();
            InitNetMsg();
        }

        public override void Destroy()
        {
            base.Destroy();
            if (mNetMsgQueue != null)
            {
                mNetMsgQueue.RemoveAllEvent();
            }
            if (mPlayerEventQueue != null)
            {
                mPlayerEventQueue.RemoveAllEvent();
            }
            mScene = null;
        }


        private void InitNetMsg()
        {
            mNetMsgQueue = new NetWorkEventQueue();
            mNetMsgQueue.AddEvent(NetWorkEventType.NE_LoginSuccess, RspTSDKLoginSuccess);
            mNetMsgQueue.AddEvent(NetWorkEventType.NE_LoginFailed, OnLoginFail);
        }

        public void RegisterEvent()
        {
            mPlayerEventQueue = new PlayerEventQueue(PlayerManager.GetSingleton().pHero);
        }

        public void RegisterSceneEvent()
        {
            mScene = SceneMachine.GetCurrentScene() as GameScene;
        }

        public void LoginViewDestory(){ }

        private void RspTSDKLoginSuccess(BaseEvent varEvt) { }

        private void OnLoginFail(BaseEvent varEvt)
        {
            if (mScene != null)
            {
                mScene.NotifySceneEvent(new SceneEvent(SceneEventType.SE_NotifyTSDKLoginFail));
            }
        }

        #region ClientEXECommond
        private bool mIsNotAllow = false;
     
        #endregion
    }

}
