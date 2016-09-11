/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ResourceLoadParam.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/7/4 14:18:15
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000.Res
{
    public class ResourceLoadParam
    {
        public string AssetName;
        public string AssetPath;
        public string AssetBundleName;
    }
    public class ResourceLoadParamEx<T> : ResourceLoadParam
    {
        private T mData;
        public T Data
        {
            get { return mData; }
            set { mData = value; }
        }
        public ResourceLoadParamEx(T obj)
        {
            mData = obj;
        }
    }
}
