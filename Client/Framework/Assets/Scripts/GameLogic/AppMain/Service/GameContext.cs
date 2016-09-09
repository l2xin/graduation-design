/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GameContext.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/1/13 9:00:41
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Air2000;
using System;
using GTools.Res;
using Air2000.Module;

namespace GameLogic
{
    public enum SceneType
    {
        LoginScene,
        CityScene,
    }

    public class GameContext : MonoBehaviour
    {
        #region [Fields]
        public const string GameName = "Hello World";
        public bool Standalone;
        private static GameContext m_Instance;
        public SceneMachine SceneMachine;
        private SceneType m_CurrentSceneType;
        #endregion

        #region [Functions]

        #region monobehaviour
        void Awake()
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start() { InitGame(); }

        void Update()
        {
            if (SceneMachine != null)
            {
                SceneMachine.Update();
            }
            ConnectionManager.GetInstance().Update();
            DetectionNetWork();
        }

        void LateUpdate() { }

        void OnLevelWasLoaded()
        {
            AppEventProcessor.GetInstance().Notify(new Air2000.Event((int)AppEventType.GE_LevelWasLoaded));
        }
        void OnDestroy()
        {
            ConnectionManager.GetInstance().Destroy();
        }
        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 300, 40), "Send a test msg"))
            {
                Connection toLoginServer = ConnectionManager.GetInstance().GetConnection(ConnectionManager.ServerType.Logic);
                if (toLoginServer != null)
                {
                    byte[] bytes = null;

                    bytes = BitConverter.GetBytes(8888);
                    toLoginServer.SendMessage(9009, bytes, 10000);
                }
            }

            if (GUI.Button(new Rect(10, 60, 300, 40), "Send a test msg (Struct)"))
            {
                Connection toLoginServer = ConnectionManager.GetInstance().GetConnection(ConnectionManager.ServerType.Logic);
                if (toLoginServer != null)
                {
                    byte[] bytes = null;

                    bytes = BitConverter.GetBytes(8888);
                    toLoginServer.SendMessage(9009, bytes, 10000);
                }
            }
        }
        #endregion

        #region internal functions
        /// <summary>
        /// Initilize our game here.
        /// </summary>
        void InitGame()
        {
            Application.targetFrameRate = 30; // disable vsync and set target frame rate to 30fps

            Screen.sleepTimeout = SleepTimeout.NeverSleep; // keep the screen always display

            // initiaize SceneMachine and register scenes
            SceneMachine = SceneMachine.GetSingleton();
            SceneMachine.RegisterState(new LoginScene());
            SceneMachine.RegisterState(new MainScene());

            //  ModuleManager.GetInstance().CreateContext<LoginContext>();

            //LoginContext<LoginModuleController, LoginViewController> context = ModuleManager.GetInstance().CreateViewContext<LoginModuleController, LoginViewController>() as LoginContext<LoginModuleController, LoginViewController>;

            LoginContext<LoginController, LoginViewController> loginContext = ContextManager.GetInstance().Add<LoginContext<LoginController, LoginViewController>>();
            ContextManager.GetInstance().Add<ResContext<ResController>>();

            GTools.Res.ResourceManager.ListenInitializeFinish += OnResourceManagerInitialized;
            GTools.Res.ResourceManager.Initialize();
        }

        void OnResourceManagerInitialized()
        {
            ConnectionManager.GetInstance().CreateConnection(ConnectionManager.ServerType.Logic, "127.0.0.1", 4999, null);

            ModelManager.GetSingleton().InitModel();
            AudioAdapter.Instance.Initialize();
#if ASSETBUNDLE_MODE
            ResourceManager.LoadAssetAsync("ui/common/atlas", "NULL", typeof(TextAsset), OnLoadedUICommonAtlas);
            ResourceManager.LoadAssetAsync("ui/common/dialog", "NULL", typeof(TextAsset), OnLoadedUICommonDialogs);
            ResourceManager.LoadAssetAsync("ui/common/font", "NULL", typeof(TextAsset), OnLoadedUIFont);
            ResourceManager.LoadAssetAsync("ui/common/localization", "Chinese", typeof(TextAsset), OnLoadedUILocalization);
#else
            ResourceManager.LoadAssetAsync("Localization/Chinese", string.Empty, typeof(TextAsset), OnLoadedUILocalization);
#endif
        }
        void OnLoadedUICommonAtlas(object obj, object param)
        {
            Helper.Log("GameContext::OnLoadedUICommonAtlas: bundle name: ui/common/altas");
        }
        void OnLoadedUICommonDialogs(object obj, object param)
        {
            Helper.Log("GameContext::OnLoadedUICommonDialogs: bundle name: ui/common/dialog");
        }
        void OnLoadedUIFont(object obj, object param)
        {
            Helper.Log("GameContext::OnLoadedUIFont: bundle name: ui/common/font");
        }
        void OnLoadedUILocalization(object obj, object param)
        {
            TextAsset asset = obj as TextAsset;
            if (asset == null)
            {
                Debug.LogError("LoadLocalization Error!!");
            }
            else
            {
                Localization.Load(asset);
                Debug.Log("LoadLocalization Success!!");
            }

            if (Standalone)
            {
                //AsyncOperation mAsyncOperation = Application.LoadLevelAsync(SceneType.SimpleScene.ToString());
                return;
            }
            else
            {
                GotoScene(SceneType.LoginScene);
            }
        }
        #endregion

        #region external functions

        public static GameContext GetInstance()
        {
            if (m_Instance == null)
            {
                GameObject obj = GameObject.Find("NewGame");
                if (obj != null)
                {
                    m_Instance = obj.GetComponent<GameContext>();
                }
            }
            return m_Instance;
        }

        public void GotoScene(SceneType type)
        {
            m_CurrentSceneType = type;
            string tmpSceneName = type.ToString();
            if (SceneMachine == null)
            {
                SceneMachine = SceneMachine.GetSingleton();
            }

            GameScene tmpScene = SceneMachine.GetState(tmpSceneName) as GameScene;
            if (tmpScene == null)
            {
                return;
            }
            GameScene tmpCurrentScene = SceneMachine.GetCurrentScene() as GameScene;

            if (tmpCurrentScene == tmpScene)
            {
                //两次跳转的场景相同，则返回;
                return;
            }
            SceneMachine.SetNextState(tmpScene.pStateName);
        }
        private void DetectionNetWork() { }

        #endregion

        #endregion
    }
}
