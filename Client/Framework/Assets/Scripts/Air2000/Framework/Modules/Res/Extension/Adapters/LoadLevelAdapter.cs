/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: LoadLevelAdapter.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/21 9:33:25
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Air2000.Res
{
    public class LevelData : MonoBehaviour
    {

    }
    public class LoadLevelAdapter : ResAdapter
    {
        private static LoadLevelAdapter mInstance;
        public static LoadLevelAdapter Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new LoadLevelAdapter();
                }
                return mInstance;
            }
            set { mInstance = value; }
        }
        private LoadLevelAdapter() { }

    }
}
