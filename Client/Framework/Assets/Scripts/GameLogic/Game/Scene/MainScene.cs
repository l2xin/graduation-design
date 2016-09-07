#define JohnMotify


using UnityEngine;
using System.Collections;
using Air2000;

namespace GameLogic
{
    public class MainScene : GameScene
    {
        public MainScene()
        : base(SceneType.CityScene.ToString()) { }

#if JohnMotify

        private bool mIsMainWindow;
        private bool mIsMainMode;
        private AsyncOperation mAsyncOperation;
        private EventHandlerQueue mPlayerEventQueue;
        private EventHandlerQueue mNetMsgQueue;
#else

		LodingMainScene mLoding;

#endif
        public override void Begin()
        {
            base.Begin();

#if JohnMotify

            mIsMainWindow = false;
            mIsMainMode = false;

            RegisterPlayerEvent();

            GameContext.GetInstance().StartCoroutine(ExecuteLoad());

#else

			mLoding = new LodingMainScene();
			
			mLoding.Start();

#endif
        }

        public override void Update()
        {
            base.Update();
        }

        public override void End()
        {
#if JohnMotify

            UnRegisterEvent();

#else
			mLoding = null;
#endif
            base.End();
        }

#if JohnMotify

        void RegisterPlayerEvent()
        {
            if (PlayerManager.GetSingleton().pHero != null)
            {
                mPlayerEventQueue = new EventHandlerQueue(PlayerManager.GetSingleton().pHero.pEventManager);
                mPlayerEventQueue.Add((int)PlayerEventType.PE_NotifyMianModeEnd, MianModeEnd);
            }

            if (mNetMsgQueue == null)
            {
                mNetMsgQueue = new EventHandlerQueue(NetworkEventProcessor.GetInstance());
            }

            mNetMsgQueue.Add((int)NetWorkEventType.NE_LoginFailed, OnLoginFail);
        }

        private void UnRegisterEvent()
        {
            if (mPlayerEventQueue != null)
            {
                mPlayerEventQueue.RemoveAll();
            }
            if (mNetMsgQueue != null)
            {
                mNetMsgQueue.RemoveAll();
            }
        }

        private void OnLoginFail(Air2000.Event varEvt)
        {
            UnRegisterEvent();
            GameContext.GetInstance().StopCoroutine(ExecuteLoad());
            //WindowManager.GetSingleton().DestoryWidowByName(MainView.VIEW_KEY);
        }

        private IEnumerator ExecuteLoad()
        {
            mAsyncOperation = Application.LoadLevelAsync(SceneType.CityScene.ToString());

            yield return mAsyncOperation;
            Air2000.OpenWindowCallBack LoadFinish = delegate (GameObject view, GTools.Res.ResourceLoadParam param)
            {
                MainViewFinish();
            };
            //WindowManager.GetSingleton().OpenWindow(MainView.VIEW_KEY,LoadFinish,null);
        }

        private void MianModeEnd(Air2000.Event varEvent)
        {
            GTimer.CancelAll("LoadMainModeTimeOut");
            mIsMainMode = true;
            TryLogin2MainScene();
        }

        private void MainViewFinish()
        {
            mIsMainWindow = true;
            TryLogin2MainScene();
            if (mIsMainMode == false)
            {
                GTimer.In(2.0f, LoadMainModeTimeOut);
            }
        }

        private void LoadMainModeTimeOut()
        {
            GTimer.CancelAll("LoadMainModeTimeOut");
            mIsMainMode = true;
            TryLogin2MainScene();
        }

        private void TryLogin2MainScene()
        {
            if (mIsMainWindow == false || mIsMainMode == false)
            {
                return;
            }
            GTimer.CancelAll("LoadMainModeTimeOut");
            WindowManager.GetSingleton().DestoryWidowByName(LoginView.VIEW_KEY);
            PlayerManager.GetSingleton().pHero.NofityPlayerEvent(new Air2000.Event((int)PlayerEventType.PE_NotifyEnterMainScene));

            this.NotifySceneEvent(new Air2000.Event((int)SceneEventType.SE_CompletedLoading));
        }

#endif

    }
}
