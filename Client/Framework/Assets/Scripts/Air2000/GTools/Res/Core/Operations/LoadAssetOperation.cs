/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: LoadAssetOperation.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 14:46:40
            // Modify History:
            //
//----------------------------------------------------------------*/
using System.Collections;
using UnityEngine;
using System;

namespace GTools.Res
{
    public abstract class LoadAssetOperation : LoadOperation
    {
        public string AssetBundleName;
        public string AssetName;
        public System.Type AssetType;
    }
}
