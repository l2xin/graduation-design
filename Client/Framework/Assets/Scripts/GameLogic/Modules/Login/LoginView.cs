/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: 登录模块视图.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/2/1 15:43:22
            // Modify History:
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Air2000;

namespace GameLogic
{
    public class LoginView : BaseView
    {
        #region standard view templeate（标准视图模板）
        public static string ASSETBUNDLE_NAME = "ui/login";
        public static string VIEW_NAME = "Login";
        public static string VIEW_KEY
        {
            get
            {
#if ASSETBUNDLE_MODE
                return ASSETBUNDLE_NAME + "/" + VIEW_NAME;
#else
                return "UI/Scene_Login/Login";
#endif
            }
        }
        public LoginListenerControl pLoginListenerControl
        {
            get { return mListenerControl as LoginListenerControl; }
        }

        private EventHandlerQueue mNetMsgQueue;
        private GameScene mScene;
        private EventHandlerQueue mGlobalEventQueue;

        private bool mIsLoginSuccess = false;
        private bool mIsLoginInfo = false;
        private bool mIsPlayerInfo = false;

        void Awake()
        {
            pModel = ModelManager.GetSingleton().GetModelByType(ModelType.MT_Login);
            if (pModel != null)
            {
                pModel.pView = this;
                pListenerControl = pModel.pListenerControl;
            }
            OnInitView();
            NotifyListener(IViewListenerEventType.OnViewInited);
        }
        void Start()
        {
            NotifyListener(IViewListenerEventType.OnViewStart);
        }
        void OnEnable()
        {
            NotifyListener(IViewListenerEventType.OnViewEnable);
        }
        void OnDisable()
        {
            NotifyListener(IViewListenerEventType.OnViewDisable);
        }
        void OnDestroy()
        {
            UnRegister();
            pLoginListenerControl.pLoginModel.LoginViewDestory();
            NotifyListener(IViewListenerEventType.OnViewDestroy);
        }
        public override void OnInitView()
        {
            RegisterNetEvent();
            RegisterSceneEvent();
            InitButtonEvent();

        }
        #endregion


        private void RegisterNetEvent()
        {
            mNetMsgQueue = new EventHandlerQueue(NetworkEventProcessor.GetInstance());
            mNetMsgQueue.Add((int)NetWorkEventType.NE_LoginFailed, OnLoginFail);
            mNetMsgQueue.Add((int)NetWorkEventType.NE_NotifyLoginInfo, OnLoginInfo);
            mNetMsgQueue.Add((int)NetWorkEventType.NE_NotifyPlayerInfo, OnPlayerInfo);
        }

        private void RegisterSceneEvent()
        {
            mScene = SceneMachine.GetCurrentScene() as GameScene;

            if (mScene != null)
            {
                mScene.RegisterSceneEvent(SceneEventType.SE_NotifyTSDKLoginSucceed, TSDKLoginSuccees);
                mScene.RegisterSceneEvent(SceneEventType.SE_NotifyTSDKLoginFail, OnLoginFail);
                mScene.RegisterSceneEvent(SceneEventType.SE_NotifyStartTSDKLogin, OnStartLogin);
                mScene.RegisterSceneEvent(SceneEventType.SE_NotifyVersionUpdate, OnStatrVersionCheck);
                mScene.RegisterSceneEvent(SceneEventType.SE_NotifyConfigCheck, StartConfigCheck);
            }

            mGlobalEventQueue = new EventHandlerQueue(CrossContextEventProcessor.GetInstance());
            mGlobalEventQueue.Add((int)CrossContextEventType.GE_NetWorkState, OnNetWorkState);
        }

        private void OnNetWorkState(Air2000.Event varEvt)
        {
            EventEX<NetworkReachability> evt = varEvt as EventEX<NetworkReachability>;
            switch (evt.Param)
            {
                case NetworkReachability.NotReachable:
                    {
                        WindowManager.GetSingleton().OpenConfirmDialog(Localization.Get("NetWorkError"), null, false);
                    }
                    break;
                default: { } break;
            }
        }

        private void UnRegister()
        {
            if (mNetMsgQueue != null)
            {
                mNetMsgQueue.RemoveAll();
            }
            if (mScene != null)
            {
                mScene.UnRegisterSceneEvent(SceneEventType.SE_NotifyTSDKLoginSucceed, TSDKLoginSuccees);
                mScene.UnRegisterSceneEvent(SceneEventType.SE_NotifyTSDKLoginFail, OnLoginFail);
                mScene.UnRegisterSceneEvent(SceneEventType.SE_NotifyStartTSDKLogin, OnStartLogin);
                mScene.UnRegisterSceneEvent(SceneEventType.SE_NotifyVersionUpdate, OnStatrVersionCheck);
                mScene.UnRegisterSceneEvent(SceneEventType.SE_NotifyConfigCheck, StartConfigCheck);
            }
            if (mGlobalEventQueue != null)
            {
                mGlobalEventQueue.RemoveAll();
            }
        }


        private void TSDKLoginSuccees(Air2000.Event varData)
        {
            mIsLoginSuccess = true;

            LoginSucceed();

            InvokeRepeating("RequestSeifInfo", 0f, 3f);

            Invoke("Timeout", 10f);
        }

        private void OnLoginFail(Air2000.Event varEvent)
        {
            Helper.LogWithToast("Login Fail!");

            CancelInvoke("Timeout");

            if (Application.platform == RuntimePlatform.Android)
            {
                UIHelper.SetActiveState(this.transform, "Button_Andriod", true);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                UIHelper.SetActiveState(this.transform, "Button", true);
            }
            else
            {
                UIHelper.SetActiveState(this.transform, "Login_PC", true);
            }
            UIHelper.SetActiveState(transform, "Loding/Loding", false);
        }

        private void OnStartLogin(Air2000.Event varEvent)
        {
            StatrLoading(Localization.Get("LoginIng"));
        }

        private void OnStatrVersionCheck(Air2000.Event varEvent)
        {
            StatrLoading(Localization.Get("Login StartVersionCheck"));
        }

        private void StartConfigCheck(Air2000.Event varEvent)
        {
            StatrLoading(Localization.Get("Login StartConfigCheck"));
        }

        private void StatrLoading(string varStr)
        {
            UIHelper.SetActiveState(this.transform, "Button", false);
            UIHelper.SetActiveState(this.transform, "Button_Andriod", false);
            UIHelper.SetActiveState(this.transform, "Login_PC", false);
            UIHelper.SetActiveState(this.transform, "Loding/Loding", true);
        }

        private void OnLoginInfo(Air2000.Event varEvt)
        {
            mIsLoginInfo = true;

            LoginSucceed();
        }
        private void OnPlayerInfo(Air2000.Event varEvt)
        {
            mIsPlayerInfo = true;

            LoginSucceed();
        }

        private void LoginSucceed()
        {
            if (mIsLoginSuccess && mIsLoginInfo && mIsPlayerInfo)
            {
                GotoMainScene();
            }
        }


        private void InitButtonEvent()
        {
            //UIHelper.SetButtonEvent(transform, "Button/Login_1", BtnBeenClicked);
            //UIHelper.SetButtonEvent(transform, "Button/Login_2", BtnBeenClicked);
            //UIHelper.SetButtonEvent(transform, "Button/Login_0", BtnBeenClicked);

            //UIHelper.SetButtonEvent(transform, "Button_Andriod/Login_0", BtnBeenClicked);
            //UIHelper.SetButtonEvent(transform, "Button_Andriod/Login_1", BtnBeenClicked);

        }
        private void Timeout()
        {
            if (mIsLoginSuccess && mIsLoginInfo)
            {
                GotoMainScene();
            }
        }

        private bool mIsNotify = false;
        private void GotoMainScene()
        {
            if (mIsNotify == true)
            {
                return;
            }
            CancelInvoke("Timeout");

            PlayerManager.GetSingleton().NotifyListener(IPlayerManagerListenerEventType.OnPlayerLogin);

            GameContext.GetInstance().GotoScene(SceneType.CityScene);

            mIsNotify = true;
        }


        private void AdjustBtnScale(Transform varRoot, string varPath)
        {
            Transform tempTra = varRoot.Find(varPath);
            if (tempTra == null)
            {
                return;
            }
            Vector2 tempResolution = WindowManager.GetSingleton().pScreenResolution;
            float tempScale = GetScale(tempResolution.y, WindowManager.GetSingleton().pRootMinHeight);

            if (tempScale < 1)
            {
                tempTra.localScale = new Vector3(tempScale, tempScale, tempScale);
            }
        }

        private float GetScale(float varStandRatio, float varCurRatio)
        {
            if (Helper.FlotEqualZero(varStandRatio - varCurRatio) == true)
            {
                return 1;
            }
            return varCurRatio / varStandRatio;
        }

    }
}

