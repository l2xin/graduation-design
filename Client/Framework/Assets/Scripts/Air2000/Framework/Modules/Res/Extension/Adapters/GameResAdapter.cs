/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GameResAdapter.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/21 9:10:56
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Air2000.Res
{
    public class GameResAdapter : ResAdapter
    {
        private static GameResAdapter mInstance;
        public static GameResAdapter Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new GameResAdapter();
                }
                return mInstance;
            }
            set { mInstance = value; }
        }
    }
}
