/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ResourceManagerOptions.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 16:37:42
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Air2000.Res
{
    [System.Serializable]
    public class ResourceManagerOptions : ScriptableObject
    {
        public ResourceMode ResourceLoadMode;
        public List<string> BundleScenes = new List<string>();
        public List<string> Platforms = new List<string>();
        public string AssetBundleServerURL;
    }
}
