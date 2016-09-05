/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: PlayerManager.cs
            // Describle:  
            // Created By:   
            // Date&Time:  2015/04/07 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Air2000;
using System;

namespace GameLogic
{
    public class PlayerManager
    {
        #region declarations

        #region static
        private static PlayerManager mInstance;
        public static PlayerManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new PlayerManager();
                }
                return mInstance;
            }
        }
        #endregion
        /// <summary>
        /// 主角Uim.
        /// </summary>
        private uint mHeroID;

        public uint pHeroID
        {
            get { return mHeroID; }
        }

        /// <summary>
        /// 主角.
        /// </summary>
        private Player mHero;

        /// <summary>
        /// 主角.
        /// </summary>
        public Player pHero
        {
            get { return mHero; }
        }
        private List<IPlayerManagerListener> mListeners;

        /// <summary>
        /// 游戏内玩家.
        /// </summary>
        private Dictionary<int, Player> mPlayers;
        public Dictionary<int, Player> pPlayers
        {
            get { return mPlayers; }
        }
        #endregion

        #region function groups

        #region constructors
        public static PlayerManager GetSingleton()
        {
            if (mInstance == null)
            {
                mInstance = new PlayerManager();
            }
            return mInstance;
        }
        private PlayerManager()
        {
            mPlayers = new Dictionary<int, Player>();
            mHero = new Player();
            mListeners = new List<IPlayerManagerListener>();
            ModelManager.GetSingleton().pSelfPlayer = mHero;
        }
        #endregion

        #region operations
        /// <summary>
        /// 登录设置ID.
        /// </summary>
        /// <param name="varID"></param>
        public void SetHeroID(uint varID)
        {
            mHeroID = varID;
        }
   
        /// <summary>
        /// 设置玩家欢乐豆数量通过玩家ID.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="beanNum"></param>
        public void SetPlayerHappyBeanNumByPlayerId(int playerId, long beanNum)
        {
            List<int> tempList = new List<int>(mPlayers.Keys);

            for (int i = 0; i < tempList.Count; i++)
            {
                if (mPlayers[tempList[i]].pPlayerData.playerId == playerId)
                {
                    mPlayers[tempList[i]].pPlayerData.happyBeanNum = beanNum;
                }
            }
        }
        /// <summary>
        /// 通过SeatID找到对应的Player
        /// </summary>
        /// <param name="varID"></param>
        /// <returns></returns>
        public Player GetPlayerBySeatID(int varSeatID)
        {
            Player tempPlayer = null;

            if (mPlayers == null || mPlayers.Count == 0)
            {
                Helper.LogError("PlayerManager: GetPlayerByID() fail caused by null Player Collection");
                return tempPlayer;
            }

            mPlayers.TryGetValue(varSeatID, out tempPlayer);

            return tempPlayer;
        }

        /// <summary>
        /// 通过PlayerID获取对的的Player.
        /// </summary>
        /// <param name="varPlayerID"></param>
        /// <returns></returns>
        public Player GetPlayerByPlayerID(int varPlayerID)
        {
            Player tempPlayer = null;

            Dictionary<int, Player>.Enumerator em = mPlayers.GetEnumerator();
            for (int i = 0; i < mPlayers.Count; i++)
            {
                em.MoveNext();
                KeyValuePair<int, Player> kvp = em.Current;
                if (kvp.Value != null)
                {
                    if (varPlayerID == kvp.Value.pPlayerData.playerId)
                    {
                        return kvp.Value;
                    }
                }
            }

            return tempPlayer;
        }
        #endregion

        #region listen
        public void AddListener(IPlayerManagerListener varListener)
        {
            if (mListeners == null)
            {
                mListeners = new List<IPlayerManagerListener>();
            }
            if (varListener == null)
            {
                return;
            }
            if (mListeners.Contains(varListener))
            {
                return;
            }
            mListeners.Add(varListener);
        }
        public void RemoveListener(IPlayerManagerListener varListener)
        {
            if (varListener == null || mListeners == null)
            {
                return;
            }
            mListeners.Remove(varListener);
        }
        public void NotifyListener(IPlayerManagerListenerEventType varEvtType)
        {
            NotifyListener(varEvtType, PlayerManager.GetSingleton().pHero);
        }
        public void NotifyListener(IPlayerManagerListenerEventType varEvtType, params object[] varArgs)
        {
            if (mListeners == null || mListeners.Count == 0)
            {
                return;
            }
            for (int i = 0; i < mListeners.Count; i++)
            {
                IPlayerManagerListener listener = mListeners[i];
                if (listener == null)
                {
                    continue;
                }
                object arg = null;
                switch (varEvtType)
                {
                    case IPlayerManagerListenerEventType.OnPlayerLogin:
                        if (varArgs != null || varArgs.Length > 0)
                        {
                            arg = varArgs[0];
                        }
                        listener.OnPlayerLogin(arg as Player);
                        break;
                    case IPlayerManagerListenerEventType.OnPlayerLogout:
                        if (varArgs != null || varArgs.Length > 0)
                        {
                            arg = varArgs[0];
                        }
                        listener.OnPlayerLogout(arg as Player);
                        break;
                    case IPlayerManagerListenerEventType.OnPlayerOffline:
                        if (varArgs != null || varArgs.Length > 0)
                        {
                            arg = varArgs[0];
                        }
                        listener.OnPlayerOffline(arg as Player);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #endregion
    }
}