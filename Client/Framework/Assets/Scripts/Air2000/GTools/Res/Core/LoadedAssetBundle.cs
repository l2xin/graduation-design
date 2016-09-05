/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: LoadedAssetBundle.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/7/4 14:11:37
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GTools.Res
{
    public class LoadedAssetBundle
    {
        public AssetBundle AssetBundle;
        public int RefCount;
        public LoadedAssetBundle(AssetBundle assetBundle)
        {
            AssetBundle = assetBundle;
        }
    }
}
