using UnityEngine;
using System.Collections;
namespace GameLogic
{
    public class PlayerData
    {
        /// <summary>
        /// PlayerID.
        /// </summary>
        public int playerId;
        /// <summary>
        /// Uin号.
        /// </summary>
        public uint playerUin;
        /// <summary>
        /// PlayerID.
        /// </summary>
        public int playerSvrStatus; //保存svr下发的玩家状态
        /// <summary>
        /// 桌子ID.
        /// </summary>
        public int tableId;
        /// <summary>
        /// 座位ID.
        /// </summary>
        public int seatId;
        /// <summary>
        /// 性别（70为男）.
        /// </summary>
        public byte gender;
        /// <summary>
        /// 钻石数量.
        /// </summary>
        public long diamondNum;
        /// <summary>
        /// 欢乐豆数量.
        /// </summary>
        public long happyBeanNum;
        /// <summary>
        /// 角色ID.
        /// </summary>
        public int actorID;
        /// <summary>
        /// 昵称.
        /// </summary>
        public string nickName;
        /// <summary>
        /// 头衔Url.
        /// </summary>
        public string headPicUrl;
        
        //下面是AI才有的数据;

        /// <summary>
        /// 玩家等级.
        /// </summary>
        public int nLevel;

        /// <summary>
        /// 角色等级.
        /// </summary>
        public int iAvatarLevel;

        /// <summary>
        /// 称号.
        /// </summary>
        public byte[] szTitle;

        /// <summary>
        /// 普通玩法的单局最高积分.
        /// </summary>
        public int iNormalMaxScore;

        /// <summary>
        /// 冒险玩法的单局最高积分.
        /// </summary>
        public int iAVGMaxScore;

        public bool IsHero;

        public PlayerData()
        {
            Initialize();
        }

        public void Initialize()
        {
            playerId = -1;
            playerUin = 0;
            playerSvrStatus = -1;
            tableId = -1;
            seatId = -1;
            gender = 0;
            diamondNum = 0;
            happyBeanNum = 0;
            nickName = string.Empty;
            headPicUrl = string.Empty;
            IsHero = false;
        }
    }
}