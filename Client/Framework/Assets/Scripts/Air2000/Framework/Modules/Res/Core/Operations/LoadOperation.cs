/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: LoadOperation.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 14:32:58
            // Modify History:
            //
//----------------------------------------------------------------*/
using System.Collections;
using UnityEngine;

namespace Air2000.Res
{
    public abstract class LoadOperation : IEnumerator
    {
        public UnityEngine.Object LoadedAsset { get; set; }
        public virtual UnityEngine.Object GetAsset() { return null; }
        public virtual T GetAsset<T>() where T : UnityEngine.Object { return default(T); }
        public LoadAssetCallback Callback { get; set; }
        public object CallbackParam { get; set; }
        public object Current { get { return null; } set { } }
        public bool MoveNext() { return !IsDone(); }
        public void Reset() { }
        public abstract bool Update();
        public abstract bool IsDone();
        public virtual void Finish(UnityEngine.Object obj = null)
        {
            if (Callback != null)
            {
                if (obj != null)
                {
                    Callback(obj, CallbackParam as ResourceLoadParam);
                }
                else
                {
                    Callback(GetAsset(), CallbackParam as ResourceLoadParam);
                }
            }
        }
        public virtual void Execute() { }
    }
}
