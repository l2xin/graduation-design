/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ViewResAdapter.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/21 9:07:29
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Air2000.Res
{
    public class ViewResAdapter : ResAdapter
    {
        private static ViewResAdapter m_Instance;
        public static ViewResAdapter Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new ViewResAdapter();
                }
                return m_Instance;
            }
            set { m_Instance = value; }
        }
        private ViewResAdapter() { }
        public LoadAssetOperation LoadWindowWithAssetBundle(string bundleName, string assetName, LoadAssetCallback callback = null, ResourceLoadParam param = null)
        {
            if (string.IsNullOrEmpty(assetName)) return null;
            if (ResContext.IsInProgress(bundleName, assetName) != null) return null;
            return ResContext.LoadAssetAsync(bundleName, assetName, typeof(GameObject), true, callback, param);
        }
        public LoadAssetOperation LoadWindowWithResource(string resPath, LoadAssetCallback callback = null, ResourceLoadParam param = null)
        {
            if (string.IsNullOrEmpty(resPath)) return null;
            if (ResContext.IsInProgress(resPath, string.Empty) != null) return null;
            return ResContext.LoadAssetAsync(resPath, "", typeof(GameObject), true, callback, param);
        }
    }
}
