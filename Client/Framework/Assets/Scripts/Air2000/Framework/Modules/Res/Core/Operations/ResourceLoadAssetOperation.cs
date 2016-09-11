/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ResourceLoadAssetOperation.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/20 9:27:34
            // Modify History:
            //
//----------------------------------------------------------------*/
using System.Collections;
using UnityEngine;

namespace Air2000.Res
{
    public class ResourceLoadAssetOperation : LoadAssetOperation
    {
        public ResourceRequest mRequest;
        public bool BoolError = false;
        public override T GetAsset<T>()
        {
            if (mRequest != null && mRequest.isDone)
                return mRequest.asset as T;
            else
                return LoadedAsset as T;
        }
        public override Object GetAsset()
        {
            if (mRequest != null && mRequest.isDone)
                return mRequest.asset;
            else
                return LoadedAsset;
        }
        public override void Execute()
        {
            if (string.IsNullOrEmpty(AssetBundleName))
            {
                BoolError = true;
            }
            mRequest = Resources.LoadAsync(AssetBundleName, AssetType);
        }
        public override bool IsDone()
        {
            if (BoolError || mRequest == null) return true;
            return mRequest.isDone;
        }
        public override bool Update()
        {
            if (IsDone())
            {
                return false;
            }
            return true;
        }
    }
}
