/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Profession.cs
			// Describle: 角色的职业
			// Created By:  hsu
			// Date&Time:  2016/4/15 16:27:11
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    /// <summary>
    /// 定义角色的职业
    /// </summary>
    public enum Profession
    {
        None = -1,
        /// <summary>
        /// 帅帅
        /// </summary>
        ShuaiShuai = 1,
        /// <summary>
        /// 乔乔
        /// </summary>
        Joe = 2,
        /// <summary>
        /// 小师妹
        /// </summary>
        LittleSister = 3,
        /// <summary>
        /// 巫女
        /// </summary>
        Mage = 4,
    }
}
