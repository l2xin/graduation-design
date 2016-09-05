/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: AssetBundleLoadLevelOperation.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 14:42:40
            // Modify History:
            //
//----------------------------------------------------------------*/
using System.Collections;
using UnityEngine;

namespace GTools.Res
{
    public class AssetBundleLoadLevelOperation : LoadOperation
    {
        public string AssetBundleName;
        public string LevelName;
        public bool Additive;
        public string DownloadingError;
        public AsyncOperation Request;
        public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool additive)
        {
            AssetBundleName = assetbundleName;
            LevelName = levelName;
            Additive = additive;
        }
        public override bool Update()
        {
            if (Request != null)
                return false;
            LoadedAssetBundle bundle = ResourceManager.GetLoadedAssetBundle(AssetBundleName, out DownloadingError);
            if (bundle != null)
            {
                if (Additive)
                    Request = Application.LoadLevelAdditiveAsync(LevelName);
                else
                    Request = Application.LoadLevelAsync(LevelName);
                return false;
            }
            else
                return true;
        }
        public override bool IsDone()
        {
            if (Request == null && DownloadingError != null)
            {
                Utility.LogError(DownloadingError);
                return true;
            }
            return Request != null && Request.isDone;
        }
    }
}
