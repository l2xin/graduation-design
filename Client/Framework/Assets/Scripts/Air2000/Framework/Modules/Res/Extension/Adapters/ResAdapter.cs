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

namespace Air2000.Res
{
    public abstract class ResAdapter
    {
        public LoadAssetOperation IsInProgress(string assetPath, string assetbundleName, string assetName)
        {
            if (ResContext.IsResourceMode)
            {
                return ResContext.IsInProgress(assetPath, assetName);
            }
            else
            {
                return ResContext.IsInProgress(assetbundleName, assetName);
            }
        }

        public LoadAssetOperation LoadAssetAsync(string assetPath, string assetbundleName, string assetName, Type assetType, LoadAssetCallback callback = null, ResourceLoadParam param = null)
        {
            if (ResContext.IsResourceMode)
            {
                return ResContext.LoadAssetAsync(assetPath, assetName, assetType, callback, param);
            }
            else
            {
                return ResContext.LoadAssetAsync(assetbundleName, assetName, assetType, callback, param);
            }
        }
    }
}
