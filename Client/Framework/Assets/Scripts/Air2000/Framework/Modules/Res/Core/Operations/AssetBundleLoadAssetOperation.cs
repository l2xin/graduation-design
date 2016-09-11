/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: AssetBundleLoadAssetOperation.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 14:50:08
            // Modify History:
            //
//----------------------------------------------------------------*/
using System.Collections;
using UnityEngine;

namespace Air2000.Res
{
    public class AssetBundleLoadAssetOperation : LoadAssetOperation
    {
        public string DownloadingError;
        public AssetBundleRequest Request = null;
        public AssetBundleLoadAssetOperation(string bundleName, string assetName, System.Type type)
        {
            AssetBundleName = bundleName;
            AssetName = assetName;
            AssetType = type;
        }
        public override T GetAsset<T>()
        {
            if (Request != null && Request.isDone)
                return Request.asset as T;
            else
                return null;
        }
        public override Object GetAsset()
        {
            if (Request != null && Request.isDone)
                return Request.asset;
            else
                return null;
        }
        public override bool Update()
        {
            if (Request != null)
                return false;
            LoadedAssetBundle bundle = ResContext.GetLoadedAssetBundle(AssetBundleName, out DownloadingError);
            if (bundle != null)
            {
                Request = bundle.AssetBundle.LoadAssetAsync(AssetName, AssetType);
            }
            return true;
        }
        public override bool IsDone()
        {
            if (Request == null && string.IsNullOrEmpty(DownloadingError) == false)
            {
                //Debug.LogError(DownloadingError);
                return true;
            }
            if (Request != null)
            {
                if (string.IsNullOrEmpty(AssetName) || AssetName.Equals("NULL") || AssetName.Equals("null"))
                {
                    return true;
                }
            }
            return Request != null && Request.isDone;
        }
    }
}
