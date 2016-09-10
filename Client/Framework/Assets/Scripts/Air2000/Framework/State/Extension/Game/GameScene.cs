/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GameScene.cs
			// Describle: 场景基类
			// Created By:  hsu
			// Date&Time:  2016/1/13 17:59:59
            // Modify History:
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Air2000
{
    public class GameScene : State
    {
        public enum LAYER
        {
            Default = 0,
            FXQUI = 8,
            FXQDialog = 9,
            Ground = 10,
            SceneEffect = 11,
            Player = 12,
            Plane = 13,
            ThrModel = 14,
        }

        private GameObject mSceneObject;//场景的GameObject 实例
        private SceneEventProcessor mEventManager;

        /// <summary>
        /// 该场景的Event转发实例
        /// </summary>
        public SceneEventProcessor pEventManager
        {
            get { return mEventManager; }
        }

        /// <summary>
        /// 该场景的GameObject 实例
        /// </summary>
        public GameObject pSceneObject
        {
            get { return mSceneObject; }
            set { mSceneObject = value; }
        }

        public GameScene(string varSceneName)
            : base(varSceneName)
        {
            mEventManager = new SceneEventProcessor(this);
        }

        /// <summary>
        /// 注册场景事件
        /// </summary>
        /// <param name="evtType"></param>
        /// <param name="func"></param>
        public void RegisterSceneEvent(SceneEventType evtType, EventProcessorHandler func)
        {
            if (mEventManager == null)
            {
                return;
            }
            mEventManager.Register((int)evtType, func);
        }

        /// <summary>
        /// 解注册场景事件
        /// </summary>
        /// <param name="evtType"></param>
        /// <param name="func"></param>
        public void UnRegisterSceneEvent(SceneEventType evtType, EventProcessorHandler func)
        {
            if (mEventManager == null)
            {
                return;
            }
            mEventManager.Unregister((int)evtType, func);
        }

        /// <summary>
        /// 广播场景事件
        /// </summary>
        /// <param name="evt"></param>
        public void NotifySceneEvent(Event evt)
        {
            if (mEventManager == null)
            {
                return;
            }
            mEventManager.Notify(evt);
            return;
        }

        public override void Begin()
        {
            CrossContextEventProcessor.GetInstance().Notify(new EventEX<GameScene>((int)CrossContextEventType.GE_SceneBegin, this));
            return;
        }

        public override void Update()
        {
        }

        public override void End()
        {
            if (mSceneObject != null)
            {
                GameObject.DestroyObject(mSceneObject);
            }
            Resources.UnloadUnusedAssets();
            CrossContextEventProcessor.GetInstance().Notify(new EventEX<GameScene>((int)CrossContextEventType.GE_SceneEnd, this));
            return;
        }
    }
}
