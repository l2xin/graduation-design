/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ResAdapter.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/21 9:00:41
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTools.Res
{
    public abstract class ResAdapter
    {
        public LoadAssetOperation IsInProgress(string assetPath, string assetbundleName, string assetName)
        {
            if (ResourceManager.IsResourceMode)
            {
                return ResourceManager.IsInProgress(assetPath, assetName);
            }
            else
            {
                return ResourceManager.IsInProgress(assetbundleName, assetName);
            }
        }

        public LoadAssetOperation LoadAssetAsync(string assetPath, string assetbundleName, string assetName, Type assetType, LoadAssetCallback callback = null, ResourceLoadParam param = null)
        {
            if (ResourceManager.IsResourceMode)
            {
                return ResourceManager.LoadAssetAsync(assetPath, assetName, assetType, callback, param);
            }
            else
            {
                return ResourceManager.LoadAssetAsync(assetbundleName, assetName, assetType, callback, param);
            }
        }
    }
}
