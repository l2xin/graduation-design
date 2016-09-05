/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: AssetBundleLoadAssetOperationSimulation.cs
			// Describle: 在开发环境下模拟AssetBundle的方式进行资源加载
			// Created By:  hsu
			// Date&Time:  2016/4/18 14:48:09
            // Modify History:
            //
//----------------------------------------------------------------*/
using System.Collections;
using UnityEngine;

namespace GTools.Res
{
    public class AssetBundleLoadAssetOperationSimulation : LoadAssetOperation
    {
        public AssetBundleLoadAssetOperationSimulation(Object obj) { LoadedAsset = obj; }
        public override T GetAsset<T>() { return LoadedAsset as T; }
        public override Object GetAsset() { return LoadedAsset; }
        public override bool Update() { return false; }
        public override bool IsDone() { return true; }
    }
}
