/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: SceneEvent.cs
			// Describle: 场景事件
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
    public enum SceneEventType
    {
        SE_None,
        SE_SceneLoadingEnd,
        SE_SceneLoadingBegin,
        /// <summary>
        /// 战斗界面初始化完成.
        /// </summary>
        SE_BattleUIStartEnd,
        /// <summary>
        /// 游戏开始.
        /// </summary>
        SE_StartBattleGame,
        /// <summary>
        /// 游戏相机更改
        /// </summary>
        SE_GameCameraChange,

        /// <summary>
        /// 通知飞机动画结算.
        /// </summary>
        SE_NotifyPlaneAnimaionEnd,

        /// <summary>
        /// 显示点数.
        /// </summary>
        SE_ShowDice,

        /// <summary>
        /// 通知谁掷骰子.
        /// </summary>
        SE_Notify_BattleUI_WhoDice,

        /// <summary>
        /// 飞机抵达位置.
        /// </summary>
        SE_PlaneArrive,

        /// <summary>
        /// 单个玩家飞机模型创建完成.
        /// </summary>
        SE_CharacterEnd,

        /// <summary>
        /// 游戏重连.
        /// </summary>
        SE_GameReconnection,

        /// <summary>
        /// 再玩一次.
        /// </summary>
        SE_PlayAgain,

        /// <summary>
        /// 托管状态.
        /// </summary>
        SE_TrusteeshipState,

        SE_InitedChessboard,

		///通知TSDK登录成功;
		SE_NotifyTSDKLoginSucceed,
		SE_NotifyTSDKLoginFail,
		SE_NotifyStartTSDKLogin,
		SE_NotifyVersionUpdate,
		SE_NotifyConfigCheck,

        SE_GameOver,

        /// <summary>
        /// 加载完成.
        /// </summary>
        SE_CompletedLoading,
    }

    public class SceneEvent : BaseEvent
    {
        public SceneEventType mEventType;

        public SceneEventType pEventType
        {
            get { return mEventType; }
        }

        public SceneEvent(SceneEventType evtType)
        {
            mEventType = evtType;
            pEventID = (int)evtType;
        }

    }

    public class SceneEventEx<T> : SceneEvent
    {
        public T mParam;
        public SceneEventEx(SceneEventType evtType, T param)
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

    public class SceneEventQueue
    {
        public Dictionary<SceneEventType, List<EventFuntion>> mEvents;
        private SceneEventManager mEventManager;
        private GameScene mScene;

        public SceneEventManager pEventManager
        {
            get { return mEventManager; }
            set { mEventManager = value; }
        }

        public GameScene pScene
        {
            get { return mScene; }
            set { mScene = value; }
        }

        public SceneEventQueue(GameScene varScene)
        {
            mScene = varScene;
            mEvents = new Dictionary<SceneEventType, List<EventFuntion>>();
            mEventManager = mScene.pEventManager;
        }

        public void AddEvent(SceneEventType varType, EventFuntion varDel)
        {
            if (mScene == null)
            {
                return;
            }
            if (varDel == null)
            {
                return;
            }
            if (mEvents == null)
            {
                mEvents = new Dictionary<SceneEventType, List<EventFuntion>>();
            }
            List<EventFuntion> tmpDels = null;
            if (mEvents.TryGetValue(varType, out tmpDels) == false)
            {
                tmpDels = new List<EventFuntion>();
                tmpDels.Add(varDel);
                mEvents.Add(varType, tmpDels);
                mEventManager.RegisterSceneEvent(varType, varDel);
            }
            else
            {
                if (tmpDels.Contains(varDel) == false)
                {
                    tmpDels.Add(varDel);
                    mEventManager.RegisterSceneEvent(varType, varDel);
                }
            }
        }

        public void RemoveEvent(SceneEventType varType, EventFuntion varDel)
        {
            if (varDel == null || mEventManager == null)
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
                mEventManager.UnRegisterSceneEvent(varType, varDel);
            }
        }

        public void RemoveAllEvent()
        {
            if (mEvents == null || mEvents.Count == 0 || mEventManager == null)
            {
                return;
            }
            Dictionary<SceneEventType, List<EventFuntion>>.Enumerator em = mEvents.GetEnumerator();
            for (int i = 0; i < mEvents.Count; i++)
            {
                em.MoveNext();
                KeyValuePair<SceneEventType, List<EventFuntion>> kvp = em.Current;
                if (kvp.Value == null || kvp.Value.Count == 0)
                {
                    continue;
                }
                for (int j = 0; j < kvp.Value.Count; j++)
                {
                    mEventManager.UnRegisterSceneEvent(kvp.Key, kvp.Value[j]);
                }
            }
            mEvents.Clear();
        }
    }
}
