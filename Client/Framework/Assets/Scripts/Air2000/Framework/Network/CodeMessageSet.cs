/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: CodeMessageSet.cs
			// Describle:  code message
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:11:09
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;

namespace Air2000
{
    public class CodeMessageSet
    {
        private static UInt32[] crc_table = new UInt32[256];
        static bool bInitTable = false;
        public static void TabelInit()
        {
            if (!bInitTable)
            {
                UInt32 c;
                UInt32 i, j;

                for (i = 0; i < 256; i++)
                {
                    c = i;
                    for (j = 0; j < 8; j++)
                    {
                        if ((c & 1) != 0)
                            c = (UInt32)(0x13578642L ^ (c >> 1));
                        else
                            c = c >> 1;
                    }
                    crc_table[i] = c;
                }
                bInitTable = true;
            }
        }

        /// <summary>
        /// The crc key.
        /// 匹配密钥;
        /// </summary>
        public static UInt32 crcKey;

        protected static List<int> codeMsgIDList = new List<int>();

        /// <summary>
        /// Adds the one need code message.
        /// 添加一个需要加密的消息id;
        /// </summary>
        /// <param name="msgID">Message I.</param>
        public static void AddOneNeedCodeMsg(int msgID)
        {
            codeMsgIDList.Add(msgID);
        }

        public static void AddAllNeedCodeMsg(List<int> varList)
        {
            codeMsgIDList.Clear();
            if (varList != null && varList.Count > 0)
            {
                codeMsgIDList.AddRange(varList);
            }
        }

        /// <summary>
        /// Bs the need code.
        /// 此消息是否需要加密;
        /// </summary>
        /// <returns><c>true</c>, if need code was bed, <c>false</c> otherwise.</returns>
        /// <param name="msgID">Message I.</param>
        public static bool bNeedCode(int msgID)
        {
            if (codeMsgIDList.Contains(msgID))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks the and get code number.
        /// 计算此结构校验码;
        /// </summary>
        /// <returns>The and get code number.</returns>
        /// <param name="byteArray">Byte array.</param>
        public static UInt32 CheckAndGetCodeNum(Byte[] buffer)
        {
            UInt32 i;
            UInt32 crc = crcKey;
            for (i = 0; i < buffer.Length; i++)
            {
                crc = crc_table[(crc ^ buffer[i]) & 0xff] ^ (crc << 8);
            }
            return crc;
        }
    }

}
