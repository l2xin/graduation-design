/*----------------------------------------------------------------
            // Copyright (C) 2015 南昌光速科技有限公司
            // 版权所有。 
            //
            // 文件名： Player
            // 文件功能描述：玩家
            //
            // 
            // 创建标识：hsu-20160113
            // 修改描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Air2000;

namespace GameLogic
{
    public enum PlayerType
    {
        /// <summary>
        /// 当前玩家
        /// </summary>
        PT_Self = 1,
        /// <summary>
        /// 其他玩家
        /// </summary>
        PT_OtherPlayer = 2,
        /// <summary>
        /// 机器人
        /// </summary>
        PT_Robot = 3,
    }

    public enum PlayerState
    {
        /// <summary>
        /// 掷骰子
        /// </summary>
        Throw,

        /// <summary>
        /// 选棋子.
        /// </summary>
        Select,

        /// <summary>
        /// 其他.
        /// </summary>
        Other,
    }


    public class Player : GTPlayer
    {
        private List<NewGameCharacter> mCharacters;//当前玩家的所有飞机，控制类
        private GameObject mCharacterHold;
        private PlayerEventProcessor mEventManager;

        private Dictionary<int, List<int>> mSuperposition;




        private PlayerState mPlayerState;
        public string mPlayerUiName;

        public PlayerEventProcessor pEventManager
        {
            get { return mEventManager; }
            set { mEventManager = value; }
        }
        public List<NewGameCharacter> pCharacters
        {
            get { return mCharacters; }
            set { mCharacters = value; }
        }

        public Dictionary<int, List<int>> pSuperposition
        {
            get { return mSuperposition; }
            set { mSuperposition = value; }
        }

        public PlayerState pPlayerState
        {
            get { return mPlayerState; }
            set { mPlayerState = value; }
        }

        public Player()
        {
            mEventManager = new PlayerEventProcessor(this);
            mSuperposition = new Dictionary<int, List<int>>();
            mPlayerData = new PlayerData();
            mCharacters = new List<NewGameCharacter>();
            mIsTrusteeship = false;
        }

        /// <summary>
        /// 玩家数据.
        /// </summary>
        private PlayerData mPlayerData;
        public PlayerData pPlayerData
        {
            get { return mPlayerData; }
            set { mPlayerData = value; }
        }



        ///////////////////////////////////////////////战斗内

        /// <summary>
        /// 玩家类型.
        /// </summary>
        private PlayerType mMyType;
        public PlayerType pMyType
        {
            get { return mMyType; }
            set { mMyType = value; }
        }

        /// <summary>
        /// 是否托管.
        /// </summary>
        private bool mIsTrusteeship;
        public bool pIsTrusteeship
        {
            get { return mIsTrusteeship; }
            set { mIsTrusteeship = value; }
        }




        private void NotifyCharacterEnd()
        {
            GameScene mScene = SceneMachine.GetCurrentScene() as GameScene;
            if (mScene != null)
            {
                mScene.NotifySceneEvent(new EventEX<Player>((int)SceneEventType.SE_CharacterEnd, this));
            }
        }


        /// <summary>
        /// 玩家断线
        /// </summary>
        public void OnPlayerOffLine()
        {
            mSuperposition.Clear();

            if (mCharacters != null && mCharacters.Count > 0)
            {
                for (int i = 0; i < mCharacters.Count; i++)
                {
                    NewGameCharacter ch = mCharacters[i];
                    if (ch == null)
                    {
                        continue;
                    }
                    CharacterPoolController.Pool(ch);
                }
                mCharacters.Clear();
                GameObject.DestroyObject(mCharacterHold.gameObject);
            }


            mSuperposition.Clear();
        }



        /// <summary>
        /// 注册角色事件.
        /// </summary>
        /// <param name="varPET"></param>
        /// <param name="varFun"></param>
        public void RegisterPlayerEvent(PlayerEventType varType, Air2000.EventProcessorHandler varFunc)
        {
            if (mEventManager == null)
            {
                mEventManager = new PlayerEventProcessor(this);
                return;
            }
            mEventManager.Register((int)varType, varFunc);
        }

        /// <summary>
        /// 解单个注册角色事件.
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varFun"></param>
        public void UnRegisterPlayerEvent(PlayerEventType varType, Air2000.EventProcessorHandler varFunc)
        {
            if (mEventManager == null)
            {
                return;
            }
            mEventManager.Unregister((int)varType, varFunc);
        }

        /// <summary>
        /// 通知角色事件.
        /// </summary>
        /// <param name="varEvent"></param>
        public void NofityPlayerEvent(Air2000.Event eventObj)
        {
            if (eventObj == null)
            {
                return;
            }
            if (mEventManager == null)
            {
                Helper.LogError("NotifyPlayerEvent fail caused by null PlayerEventManager");
                return;
            }
            mEventManager.Notify(eventObj);
        }
    }
}

