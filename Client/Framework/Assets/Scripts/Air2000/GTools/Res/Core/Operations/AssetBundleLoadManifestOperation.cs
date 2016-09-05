/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: AssetBundleLoadManifestOperation.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 14:51:57
            // Modify History:
            //
//----------------------------------------------------------------*/
using System.Collections;
using UnityEngine;

namespace GTools.Res
{
    public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperation
    {
        public AssetBundleLoadManifestOperation(string bundleName, string assetName, System.Type type)
            : base(bundleName, assetName, type) { }
        public override bool Update()
        {
            base.Update();
            if (Request != null && Request.isDone)
            {
                ResourceManager.pAssetBundleManifest = GetAsset<AssetBundleManifest>();
                return false;
            }
            else
                return true;
        }
    }
}
