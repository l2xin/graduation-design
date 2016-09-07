/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GameObjPool.cs
            // Describle: 游戏对象缓存
            // Created By:  hsu
            // Date&Time:  2015/04/07 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GTools.Res;

namespace Air2000
{
    public delegate void GameObjPoolDelegate(CacheObjInfo varInfo, object varParam);
    public delegate void GameObjPoolDelegateEx(List<CacheObjInfo> varInfo, object varParam);

    /// <summary>
    /// 源对象
    /// </summary>
    public class OriginalObjInfo
    {
        public UnityEngine.Object mObj;
        public int mRefCount;
        public string mFilePath;
        private string mFileName;
        private List<CacheObjInfo> mCacheObjs;
        public Transform mHolder;
        private GameObjPool.ObjFlag mObjFlag;

        public List<CacheObjInfo> pCacheObjs
        {
            get { return mCacheObjs; }
        }
        public string pFileName
        {
            get
            {
                if (string.IsNullOrEmpty(mFileName))
                {
                    mFileName = Helper.GetFileNameFromFullPath(mFilePath);
                }
                return mFileName;
            }
        }
        public GameObjPool.ObjFlag pObjFlag
        {
            get { return mObjFlag; }
            set { mObjFlag = value; }
        }
        public OriginalObjInfo(UnityEngine.Object varObj, string varFilePath)
        {
            mObj = varObj;
            mFilePath = varFilePath;
            mCacheObjs = new List<CacheObjInfo>();
            mRefCount = 1;
            if (varObj == null)
            {
                return;
            }
            mHolder = new GameObject("[RefCount: " + mRefCount + " ]" + mFilePath).transform;
            if (varObj.GetType() == typeof(GameObject))
            {
                GameObject tmpObj = varObj as GameObject;
                if (tmpObj == null)
                {
                    return;
                }
                tmpObj.name = "[Original] HashCode: " + tmpObj.GetHashCode() + " (" + Helper.GetFileNameFromFullPath(mFilePath) + ")";
                tmpObj.transform.SetParent(mHolder);
            }
            else
            {
                GameObject tmpObj = new GameObject("[Original] HashCode: " + mObj.GetHashCode() + " (" + Helper.GetFileNameFromFullPath(mFilePath) + ")");
                tmpObj.transform.SetParent(mHolder);
            }
            GameObjPool.GetSingleton().AddToHolder(this);
        }
        public void RefreshHolder()
        {
            if (mHolder != null)
            {
                mHolder.name = "[RefCount: " + mRefCount + " ][ObjFlag: " + pObjFlag + "] " + mFilePath;
            }
        }
        public void AddToHolder(CacheObjInfo varCacheInfo)
        {
            if (varCacheInfo == null || varCacheInfo.mHolder == null)
            {
                return;
            }
            varCacheInfo.mHolder.SetParent(mHolder);
        }

        /// <summary>
        /// 获取一个缓存对象，该方法会返回源对象的拷贝
        /// </summary>
        /// <returns></returns>
        public CacheObjInfo GetCacheObj()
        {
            return GetCacheObj(GameObjPool.ObjFlag.CloneWhenUse);
        }

        /// <summary>
        /// 获取一个缓存对象，varIsUseOriObj：是否使用源对象
        /// </summary>
        /// <param name="varIsUseOriObj"></param>
        /// <returns></returns>
        public CacheObjInfo GetCacheObj(GameObjPool.ObjFlag varResult)
        {
            if (mCacheObjs == null)
            {
                mCacheObjs = new List<CacheObjInfo>();
            }
            CacheObjInfo info = null;
            for (int i = 0; i < mCacheObjs.Count; i++)
            {
                CacheObjInfo tmpInfo = mCacheObjs[i];
                if (tmpInfo == null)
                {
                    mCacheObjs.Remove(tmpInfo);
                    i--;
                    continue;
                }
                if (tmpInfo.IsLock() == false)
                {
                    info = tmpInfo;
                }
            }
            if (info == null)
            {
                switch (varResult)
                {
                    case GameObjPool.ObjFlag.Clone:
                        info = new CacheObjInfo(UnityEngine.Object.Instantiate(mObj), this);
                        break;
                    case GameObjPool.ObjFlag.UseOriObj:
                        info = new CacheObjInfo(mObj, this);
                        break;
                    case GameObjPool.ObjFlag.CloneWhenUse:
                        info = new CacheObjInfo(null, this);
                        break;
                }
                mCacheObjs.Add(info);
            }
            info.Lock();
            mRefCount++;
            RefreshHolder();
            return info;
        }

        /// <summary>
        /// 回收使用完的对象
        /// </summary>
        /// <param name="varCacheInfo"></param>
        public void RecycleObj(CacheObjInfo varCacheInfo)
        {
            mRefCount--;
            if (varCacheInfo == null)
            {
                return;
            }
            //if (varCacheInfo.mObj == null)
            //{
            //    mCacheObjs.Remove(varCacheInfo);
            //}
            varCacheInfo.UnLock();
            RefreshHolder();
        }

        /// <summary>
        /// 移除当前的缓存物体
        /// </summary>
        /// <param name="varCacheInfo"></param>
        public void RemoveCacheObj(CacheObjInfo varCacheInfo)
        {
            if (varCacheInfo == null)
            {
                return;
            }
            if (varCacheInfo.IsLock() == true)
            {
                mRefCount--;
            }
            varCacheInfo.UnLock();
            mCacheObjs.Remove(varCacheInfo);
            RefreshHolder();
        }

        /// <summary>
        /// 该方法将销毁源对象，以及所有依赖该对象的缓存对象
        /// </summary>
        public void Destory()
        {
            mRefCount = 0;
            if (mCacheObjs != null && mCacheObjs.Count > 0)
            {
                for (int i = 0; i < mCacheObjs.Count; i++)
                {
                    if (mCacheObjs[i] != null)
                    {
                        mCacheObjs[i].OnRemove();
                    }
                }
                mCacheObjs.Clear();
                mCacheObjs = null;
            }
            if (mObj != null)
            {
                if (mObj.GetType() == typeof(GameObject))
                {
                    GameObject.DestroyObject(mObj);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(mObj);
                }
            }
            if (mHolder != null)
            {
                GameObject.DestroyObject(mHolder.gameObject);
            }
            GameObjPool.GetSingleton().RemoveOriginalObjInfo(this);
        }
    }

    /// <summary>
    /// 拷贝的缓存对象
    /// </summary>
    public class CacheObjInfo
    {
        public OriginalObjInfo mOriginalObjInfo;
        private bool bLock;
        private UnityEngine.Object mObj;
        public Transform mHolder;
        public CacheObjInfo(UnityEngine.Object varObj, OriginalObjInfo varOriginalObjInfo)
        {
            bLock = false;
            mObj = varObj;
            mOriginalObjInfo = varOriginalObjInfo;
            if (varObj != null && mOriginalObjInfo != null)
            {
                if (varObj.GetType() == typeof(GameObject))
                {
                    GameObject tmpObj = varObj as GameObject;
                    if (tmpObj == null)
                    {
                        return;
                    }
                    mHolder = tmpObj.transform;
                    mHolder.name = "[Lock: " + bLock + "] HashCode: " + mObj.GetHashCode() + " (" + Helper.GetFileNameFromFullPath(mOriginalObjInfo.mFilePath) + ")";
                }
                else
                {
                    mHolder = new GameObject("[Lock: " + bLock + "] HashCode: " + mObj.GetHashCode() + " (" + Helper.GetFileNameFromFullPath(mOriginalObjInfo.mFilePath) + ")").transform;
                }
            }
            varOriginalObjInfo.AddToHolder(this);
        }

        private void RefreshHolder()
        {
            if (mHolder != null && mOriginalObjInfo != null && mObj != null)
            {
                mHolder.name = "[Lock: " + bLock + "] HashCode: " + mObj.GetHashCode() + " (" + Helper.GetFileNameFromFullPath(mOriginalObjInfo.mFilePath) + ")";
            }
        }


        /// <summary>
        /// 重新指定引用
        /// </summary>
        /// <returns></returns>
        public CacheObjInfo ReAssign()
        {
            if (mOriginalObjInfo != null)
            {
                CacheObjInfo info = mOriginalObjInfo.GetCacheObj(mOriginalObjInfo.pObjFlag);
                return info;
            }
            return null;
        }

        /// <summary>
        /// 锁定当前对象
        /// </summary>
        public void Lock()
        {
            bLock = true;
            RefreshHolder();
        }

        /// <summary>
        /// 解锁当前对象
        /// </summary>
        public void UnLock()
        {
            bLock = false;
            RefreshHolder();
        }

        /// <summary>
        /// 是否被占用
        /// </summary>
        /// <returns></returns>
        public bool IsLock()
        {
            return bLock;
        }

        /// <summary>
        /// 回收对象，当对象处于空闲状态时，务必调用该方法回收
        /// </summary>
        public void Recycle()
        {
            if (mOriginalObjInfo != null)
            {
                mOriginalObjInfo.RecycleObj(this);
            }
            if (mObj != null && mObj.GetType() == typeof(GameObject))
            {
                GameObject gObj = mObj as GameObject;
                if (gObj == null)
                {
                    return;
                }
                gObj.transform.SetParent(mHolder);
                //Helper.SetLayer(gObj,GameScene.LAYER.)
            }
        }

        /// <summary>
        /// 该方法将销毁当前缓存的对象,如果对象的引用为源对象，则会同时删除源对象
        /// </summary>
        public void Destory()
        {
            if (mObj != null)
            {
                if (mObj.GetType() == typeof(GameObject))
                {
                    GameObject.DestroyObject(mObj);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(mObj);
                }
            }
            if (mHolder != null)
            {
                GameObject.DestroyObject(mHolder.gameObject);
            }
            if (mOriginalObjInfo != null)
            {
                mOriginalObjInfo.RemoveCacheObj(this);
            }
            if (mObj == mOriginalObjInfo.mObj)
            {
                mOriginalObjInfo.Destory();
            }
        }

        public void OnRemove()
        {
            if (mObj != null)
            {
                if (mObj.GetType() == typeof(GameObject))
                {
                    GameObject.DestroyObject(mObj);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(mObj);
                }
            }
            if (mHolder != null)
            {
                GameObject.DestroyObject(mHolder.gameObject);
            }
        }
        public UnityEngine.Object GetObj()
        {

            if (mObj == null && mOriginalObjInfo != null && mOriginalObjInfo.mObj != null)
            {
                mObj = UnityEngine.Object.Instantiate(mOriginalObjInfo.mObj);
                if (mObj != null)
                {
                    if (mObj.GetType() == typeof(GameObject))
                    {
                        GameObject tmpObj = mObj as GameObject;
                        if (tmpObj == null)
                        {
                            return null;
                        }
                        mHolder = tmpObj.transform;
                        mHolder.name = "[Lock: " + bLock + "] HashCode: " + mObj.GetHashCode() + " (" + Helper.GetFileNameFromFullPath(mOriginalObjInfo.mFilePath) + ")";
                    }
                    else
                    {
                        mHolder = new GameObject("[Lock: " + bLock + "] HashCode: " + mObj.GetHashCode() + " (" + Helper.GetFileNameFromFullPath(mOriginalObjInfo.mFilePath) + ")").transform;
                        mOriginalObjInfo.AddToHolder(this);
                    }
                }
            }
            return mObj;
        }
    }

    public class GameObjPool
    {
        #region Inner Class

        public class Handle
        {
            public object[] Params { get; set; }
            public GameObjPoolDelegate Callback { get; set; }
            public int TaskCount { get; set; }
            public Handle(GameObjPoolDelegate varFunc)
            {
                Callback = varFunc;
            }
            public Handle(GameObjPoolDelegate varFunc, params object[] varObjs)
            {
                Callback = varFunc;
                Params = varObjs;
            }
            public void OnTaskEnd(LoadTask varTask, CacheObjInfo varObjInfo)
            {
                if (TaskCount < 0)
                {
                    return;
                }
                TaskCount--;
                if (TaskCount == 0)
                {
                    if (Callback != null)
                    {
                        Callback(null, Params);
                    }
                }
            }
            public void Destroy()
            {
                if (Params != null)
                {
                    Params = null;
                }
            }
        }
        public enum ObjFlag
        {
            /// <summary>
            /// 当使用时拷贝
            /// </summary>
            CloneWhenUse,
            /// <summary>
            /// 拷贝一个新的对象
            /// </summary>
            Clone,
            /// <summary>
            /// 使用源对象
            /// </summary>
            UseOriObj,
        }
        public class LoadTask
        {
            public string mFilePath;
            public string mBundleName;
            public string mAssetName;
            public object mParam;
            public GameObjPoolDelegate mFunc;
            public Type mType;
            public ObjFlag mObjFlag;
            public Handle pHandle { get; set; }
            public LoadTask(string varFilePath, Type varType, GameObjPoolDelegate varFunc)
            {
                mFilePath = varFilePath;
                mType = varType;
                mFunc = varFunc;
                mObjFlag = ObjFlag.CloneWhenUse;
                mAssetName = Helper.GetFileNameFromFullPath(mFilePath);
            }
            public LoadTask(string varFilePath, Type varType)
            {
                mFilePath = varFilePath;
                mType = varType;
                mObjFlag = ObjFlag.CloneWhenUse;
                mAssetName = Helper.GetFileNameFromFullPath(mFilePath);
            }
            public LoadTask(string varFilePath, Type varType, ObjFlag varObjFlag)
            {
                mFilePath = varFilePath;
                mType = varType;
                mObjFlag = varObjFlag;
                mAssetName = Helper.GetFileNameFromFullPath(mFilePath);
            }
            public LoadTask(string varFilePath, Type varType, GameObjPoolDelegate varFunc, ObjFlag varObjFlag)
            {
                mFilePath = varFilePath;
                mType = varType;
                mFunc = varFunc;
                mObjFlag = varObjFlag;
                mAssetName = Helper.GetFileNameFromFullPath(mFilePath);
            }
            public LoadTask(string varFilePath, Type varType, GameObjPoolDelegate varFunc, object varParam)
            {
                mFilePath = varFilePath;
                mType = varType;
                mFunc = varFunc;
                mParam = varParam;
                mObjFlag = ObjFlag.CloneWhenUse;
                mAssetName = Helper.GetFileNameFromFullPath(mFilePath);
            }

            public LoadTask(string varFilePath, Type varType, GameObjPoolDelegate varFunc, object varParam, ObjFlag varObjFlag)
            {
                mFilePath = varFilePath;
                mType = varType;
                mFunc = varFunc;
                mParam = varParam;
                mObjFlag = varObjFlag;
                mAssetName = Helper.GetFileNameFromFullPath(mFilePath);
            }
            public void Callback(CacheObjInfo varInfo)
            {
                if (mFunc != null)
                {
                    mFunc(varInfo, mParam);
                    if (pHandle != null)
                    {
                        pHandle.OnTaskEnd(this, varInfo);
                    }
                }
            }

            ~LoadTask()
            {
            }
        }
        public class TaskQueue
        {
            private string mFilePath;
            private string mFileName;
            private Type mType;
            private List<LoadTask> mLoadTasks;
            private ObjFlag mObjFlag;
            public string pFilePath
            {
                get { return mFilePath; }
                set { mFilePath = value; }
            }
            public string pFileName
            {
                get { return mFileName; }
                set { mFileName = value; }
            }
            public Type pType
            {
                get { return mType; }
                set { mType = value; }
            }
            public ObjFlag pObjFlag
            {
                get { return mObjFlag; }
                set { mObjFlag = value; }
            }

            public TaskQueue(string varFilePath)
            {
                mFilePath = varFilePath;
                mLoadTasks = new List<LoadTask>();
                mFileName = Helper.GetFileNameFromFullPath(varFilePath);
            }

            public TaskQueue(string varFilePath, LoadTask varTask)
            {
                mLoadTasks = new List<LoadTask>();
                if (varTask == null)
                {
                    return;
                }
                mFilePath = varFilePath;
                mType = varTask.mType;
                mObjFlag = varTask.mObjFlag;
                mLoadTasks.Add(varTask);
                mFileName = Helper.GetFileNameFromFullPath(varFilePath);
            }

            /// <summary>
            /// 向队列中添加一个加载任务
            /// </summary>
            /// <param name="varTask"></param>
            public void AddLoadTask(LoadTask varTask)
            {
                if (mLoadTasks == null)
                {
                    mLoadTasks = new List<LoadTask>();
                }
                if (mLoadTasks.Contains(varTask))
                {
                    return;
                }
                mType = varTask.mType;
                mLoadTasks.Add(varTask);
            }
            /// <summary>
            /// 执行加载任务
            /// </summary>
            public void Excute()
            {
                if (mLoadTasks == null || mLoadTasks.Count == 0)
                {
                    Helper.LogError("TaskQueue Excute: Error caused by null mLoadTasks || mLoadTasks.Count =0");
                    return;
                }
                LoadTask firstTask = mLoadTasks[0];
                if (firstTask == null)
                { return; }
                //   ResourceManager.GetSingleton().LoadAssetBundle(mFilePath, mFileName, mType, OnLoadCallbackPreviosVersion, null);
                GTools.Res.GameResAdapter.Instance.LoadAssetAsync(firstTask.mFilePath, firstTask.mBundleName, firstTask.mAssetName, firstTask.mType, OnLoadCallback, null);
            }

            private void OnLoadCallback(UnityEngine.Object varObj, GTools.Res.ResourceLoadParam varParam)
            {
                GameObjPool.GetSingleton().RemoveTaskQueue(this);
                if (varObj == null)
                {
                    if (varParam != null)
                    {
                        Helper.LogError("LoadTask OnLoadCallback:Error caused by null UnityEngine.Object instance,type: " + mType.ToString() + ",path: " + mFilePath);
                    }
                    else
                    {
                        Helper.LogError("LoadTask OnLoadCallback:Error caused by null UnityEngine.Object instance,type: " + mType.ToString() + ",path: " + mFilePath);
                    }
                    if (mLoadTasks != null && mLoadTasks.Count > 0)
                    {
                        for (int i = 0; i < mLoadTasks.Count; i++)
                        {
                            LoadTask task = mLoadTasks[i];
                            if (task != null)
                            {
                                task.Callback(null);
                            }
                        }
                    }
                    mLoadTasks.Clear();
                    return;
                }
                OriginalObjInfo info = null;
                if (mType == typeof(Texture) || mType == typeof(Texture2D) || mType == typeof(Texture3D))
                {
                    info = new OriginalObjInfo(varObj, mFilePath);
                }
                else
                {
                    info = new OriginalObjInfo(UnityEngine.Object.Instantiate(varObj), mFilePath);

                }
                info.pObjFlag = pObjFlag;
                GameObjPool.GetSingleton().AddOriginalObjInfo(info);
                if (mLoadTasks != null && mLoadTasks.Count > 0)
                {
                    for (int i = 0; i < mLoadTasks.Count; i++)
                    {
                        LoadTask task = mLoadTasks[i];
                        if (task != null)
                        {
                            task.Callback(info.GetCacheObj(task.mObjFlag));
                        }
                    }
                }
            }
            private void OnLoadCallbackPreviosVersion(UnityEngine.Object varObj, ResourceLoadParam varParam)
            {
                GameObjPool.GetSingleton().RemoveTaskQueue(this);
                if (varObj == null)
                {
                    if (varParam != null)
                    {
                        Helper.LogError("LoadTask OnLoadCallback:Error caused by null UnityEngine.Object instance,type: " + mType.ToString() + ",path: " + mFilePath);
                    }
                    else
                    {
                        Helper.LogError("LoadTask OnLoadCallback:Error caused by null UnityEngine.Object instance,type: " + mType.ToString() + ",path: " + mFilePath);
                    }
                    if (mLoadTasks != null && mLoadTasks.Count > 0)
                    {
                        for (int i = 0; i < mLoadTasks.Count; i++)
                        {
                            LoadTask task = mLoadTasks[i];
                            if (task != null)
                            {
                                task.Callback(null);
                            }
                        }
                    }
                    mLoadTasks.Clear();
                    return;
                }
                OriginalObjInfo info = null;
                if (mType == typeof(Texture) || mType == typeof(Texture2D) || mType == typeof(Texture3D))
                {
                    info = new OriginalObjInfo(varObj, mFilePath);
                }
                else
                {
                    info = new OriginalObjInfo(UnityEngine.Object.Instantiate(varObj), mFilePath);

                }
                info.pObjFlag = pObjFlag;
                GameObjPool.GetSingleton().AddOriginalObjInfo(info);
                if (mLoadTasks != null && mLoadTasks.Count > 0)
                {
                    for (int i = 0; i < mLoadTasks.Count; i++)
                    {
                        LoadTask task = mLoadTasks[i];
                        if (task != null)
                        {
                            task.Callback(info.GetCacheObj(task.mObjFlag));
                        }
                    }
                }
            }

            ~TaskQueue()
            {

            }
        }
        #endregion

        #region member
        private static GameObjPool mInstance;
        private EventHandlerQueue mGlobalEventQueue;//全局事件队列
        private Transform mObjHolder;
        private Transform mGameObjectHolder;
        private Transform mTextAssetHolder;
        private Transform mAudioClipHolder;
        private Transform mTextureHolder;
        private Transform mMaterialHolder;
        private const string GAMEOBJPOOL_CONFIG_FILEPATH = "Config/Other/GameObjPoolConfig.xml";
        private const string GAMEOBJPOOL_VERSION = "1.0";
        private List<TaskQueue> mLoadingQueues;
        private Dictionary<string, OriginalObjInfo> mPoolObjs;
        #endregion

        #region private method
        private GameObjPool()
        {
            mObjHolder = new GameObject("Obj Holder").transform;
            mObjHolder.position = new Vector3(0, -999, 0);
            mObjHolder.gameObject.SetActive(false);

            mGameObjectHolder = new GameObject("GameObject Holder").transform;
            mGameObjectHolder.SetParent(mObjHolder);


            mTextAssetHolder = new GameObject("TextAsset Holder(For Debug)").transform;
            mTextAssetHolder.SetParent(mObjHolder);

            mAudioClipHolder = new GameObject("AudioClip Holder(For Debug)").transform;
            mAudioClipHolder.SetParent(mObjHolder);

            mTextureHolder = new GameObject("Texture Holder(For Debug)").transform;
            mTextureHolder.SetParent(mObjHolder);

            mMaterialHolder = new GameObject("Material Holder(For Debug)").transform;
            mMaterialHolder.SetParent(mObjHolder);

            GameObject.DontDestroyOnLoad(mObjHolder);

            mPoolObjs = new Dictionary<string, OriginalObjInfo>();
            mLoadingQueues = new List<TaskQueue>();
        }

        private void OnLoadConfigCallback(UnityEngine.Object varObj, ResourceLoadParam varParam)
        {
            if (varParam == null)
            {
                Helper.LogError("GameObjPool OnLoadConfigCallback: Init fail caused by null ResourceLoadParam instance");
                return;
            }
            TextAsset text = varObj as TextAsset;
            if (text == null)
            {
                Helper.LogError("GameObjPool OnLoadConfigCallback: Init fail caused by null config file,path: " + varParam.AssetPath);
                return;
            }
        }

        /// <summary>
        /// 判断某一资源是否正在加载
        /// </summary>
        /// <param name="varFilePath"></param>
        /// <returns></returns>
        private bool IsInLoading(string varFilePath)
        {
            if (string.IsNullOrEmpty(varFilePath))
            {
                return false;
            }
            if (mLoadingQueues == null || mLoadingQueues.Count == 0)
            {
                return false;
            }
            for (int i = 0; i < mLoadingQueues.Count; i++)
            {
                TaskQueue queue = mLoadingQueues[i];
                if (queue == null)
                {
                    continue;
                }
                if (queue.pFilePath == varFilePath)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加一个加载队列
        /// </summary>
        /// <param name="varQueue"></param>
        private void AddTaskQueue(TaskQueue varQueue)
        {
            if (mLoadingQueues == null)
            {
                mLoadingQueues = new List<TaskQueue>();
            }
            if (mLoadingQueues.Contains(varQueue))
            {
                return;
            }
            if (IsInLoading(varQueue.pFilePath))
            {
                return;
            }
            mLoadingQueues.Add(varQueue);
        }

        /// <summary>
        /// 通过资源路径查找相应的队列
        /// </summary>
        /// <param name="varFilePath"></param>
        /// <returns></returns>
        private TaskQueue GetQueueByName(string varFilePath)
        {
            if (string.IsNullOrEmpty(varFilePath))
            {
                return null;
            }
            if (mLoadingQueues == null || mLoadingQueues.Count == 0)
            {
                return null;
            }
            for (int i = 0; i < mLoadingQueues.Count; i++)
            {
                TaskQueue queue = mLoadingQueues[i];
                if (queue == null)
                {
                    continue;
                }
                if (queue.pFilePath == varFilePath)
                {
                    return queue;
                }
            }
            return null;
        }

        /// <summary>
        /// 为指定的加载队列增加一个任务
        /// </summary>
        /// <param name="varTask"></param>
        private void AddLoadTaskToQueue(LoadTask varTask)
        {
            if (varTask == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(varTask.mFilePath))
            {
                return;
            }
            TaskQueue queue = GetQueueByName(varTask.mFilePath);
            if (queue != null)
            {
                queue.AddLoadTask(varTask);
            }
        }
        private void RefreshTasks()
        {

        }
        #endregion


        #region public method
        /// <summary>
        /// 更新holder下的对象显示
        /// </summary>
        /// <param name="varOriObjInfo"></param>
        public void AddToHolder(OriginalObjInfo varOriObjInfo)
        {
            if (varOriObjInfo == null || varOriObjInfo.mObj == null || varOriObjInfo.mHolder == null)
            {
                return;
            }
            Type type = varOriObjInfo.mObj.GetType();
            if (type == typeof(TextAsset))
            {
                varOriObjInfo.mHolder.SetParent(mTextAssetHolder);
            }
            else if (type == typeof(GameObject))
            {
                varOriObjInfo.mHolder.SetParent(mGameObjectHolder);
            }
            else if (type == typeof(Texture) || type == typeof(Texture2D) || type == typeof(Texture3D))
            {
                varOriObjInfo.mHolder.SetParent(mTextureHolder);
            }
            else if (type == typeof(AudioClip))
            {
                varOriObjInfo.mHolder.SetParent(mAudioClipHolder);
            }
            else if (type == typeof(AnimationClip))
            {

            }
            else if (type == typeof(Material))
            {
                varOriObjInfo.mHolder.SetParent(mMaterialHolder);
            }
        }
        /// <summary>
        /// 当一个加载任务完成后，从队列中移除
        /// </summary>
        /// <param name="varTask"></param>
        public void RemoveTaskQueue(TaskQueue varTaskQueue)
        {
            if (mLoadingQueues != null && mLoadingQueues.Count > 0 && varTaskQueue != null)
            {
                mLoadingQueues.Remove(varTaskQueue);
            }
        }
        /// <summary>
        /// 对象池中是否存在某个对象的缓存
        /// </summary>
        /// <param name="varFilePath"></param>
        /// <returns></returns>
        public bool IsExistObjInfo(string varFilePath)
        {
            if (string.IsNullOrEmpty(varFilePath))
            {
                return false;
            }
            if (mPoolObjs == null || mPoolObjs.Count == 0)
            {
                return false;
            }
            OriginalObjInfo info = null;
            if (mPoolObjs.TryGetValue(varFilePath, out info))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将加载好的对象加入对象池中
        /// </summary>
        /// <param name="varInfo"></param>
        /// <returns></returns>
        public bool AddOriginalObjInfo(OriginalObjInfo varInfo)
        {
            if (varInfo == null || string.IsNullOrEmpty(varInfo.mFilePath))
            {
                return false;
            }
            OriginalObjInfo info = null;
            if (mPoolObjs.TryGetValue(varInfo.mFilePath, out info))
            {
                return false;
            }
            mPoolObjs.Add(varInfo.mFilePath, varInfo);
            return true;
        }

        /// <summary>
        /// 获取对象池中的某一源对象
        /// </summary>
        /// <param name="varFilePath"></param>
        /// <returns></returns>
        public OriginalObjInfo GetOriginalObjInfo(string varFilePath)
        {
            if (string.IsNullOrEmpty(varFilePath))
            {
                Helper.LogError("GameObjPool GetOriginalObjInfo: Error caused by null varFilePath");
                return null;
            }
            if (mPoolObjs == null || mPoolObjs.Count == 0)
            {
                Helper.LogError("GameObjPool GetOriginalObjInfo: Error caused by null mPoolObjs || mPoolObjs.Count = 0");
                return null;
            }
            OriginalObjInfo info = null;
            mPoolObjs.TryGetValue(varFilePath, out info);
            return info;
        }

        /// <summary>
        /// 从对象池中移除一个对象
        /// </summary>
        /// <param name="varOriObjInfo"></param>
        public void RemoveOriginalObjInfo(OriginalObjInfo varOriObjInfo)
        {
            if (varOriObjInfo == null)
            {
                return;
            }
            if (mPoolObjs == null || mPoolObjs.Count == 0)
            {
                return;
            }
            mPoolObjs.Remove(varOriObjInfo.mFilePath);
        }

        /// <summary>
        /// 获取对象池中的某一缓存对象
        /// </summary>
        /// <param name="varFilePath"></param>
        /// <returns></returns>
        public CacheObjInfo GetCacheObjInfo(string varFilePath)
        {
            return GetCacheObjInfo(varFilePath, ObjFlag.CloneWhenUse);
        }

        /// <summary>
        /// 获取对象池中的某一缓存对象,varIsUseOriObj:是否获取源对象
        /// </summary>
        /// <param name="varFilePath"></param>
        /// <returns></returns>
        public CacheObjInfo GetCacheObjInfo(string varFilePath, ObjFlag varResult)
        {
            if (string.IsNullOrEmpty(varFilePath))
            {
                Helper.LogError("GameObjPool GetCacheObjInfo: Error caused by null varFilePath");
                return null;
            }
            OriginalObjInfo oriInfo = GetOriginalObjInfo(varFilePath);
            if (oriInfo == null)
            {
                Helper.LogError("GameObjPool GetCacheObjInfo: Error caused by null OriginalObjInfo insatance,filePath: " + varFilePath);
                return null;
            }
            return oriInfo.GetCacheObj(varResult);
        }
        public static GameObjPool GetSingleton()
        {
            if (mInstance == null)
            {
                mInstance = new GameObjPool();
            }
            return mInstance;
        }

        //public void Init()
        //{
        //    ResourceLoadParam param = new ResourceLoadParam();
        //    string fileName = Helper.GetFileNameFromFullPath(GAMEOBJPOOL_CONFIG_FILEPATH);
        //    param.keyID = GAMEOBJPOOL_CONFIG_FILEPATH;
        //    param.name = fileName;
        //    ResourceManager.GetSingleton().LoadAssetBundle(GAMEOBJPOOL_CONFIG_FILEPATH, fileName, typeof(TextAsset), OnLoadConfigCallback, param);
        //}

        /// <summary>
        /// 加载/获得 某个对象物体
        /// </summary>
        /// <param name="varTask"></param>
        public void LoadObj(LoadTask varTask)
        {
            if (varTask == null)
            {
                Helper.LogError("GameObjPool LoadObj:Error caused by null LoadTask instance");
                return;
            }
            if (varTask.mType == typeof(Texture) || varTask.mType == typeof(Texture2D) || varTask.mType == typeof(Texture3D))
            {
                varTask.mObjFlag = ObjFlag.UseOriObj;
            }
            if (string.IsNullOrEmpty(varTask.mFilePath))
            {
                Helper.LogError("GameObjPool LoadObj:Error caused by null LoadTask.mFilePath");
                return;
            }
            if (IsExistObjInfo(varTask.mFilePath))
            {
                varTask.Callback(GetCacheObjInfo(varTask.mFilePath, varTask.mObjFlag));
            }
            else
            {
                if (IsInLoading(varTask.mFilePath) == false)
                {
                    TaskQueue queue = new TaskQueue(varTask.mFilePath, varTask);
                    AddTaskQueue(queue);
                    queue.Excute();
                }
                else
                {
                    AddLoadTaskToQueue(varTask);
                }
            }
        }

        /// <summary>
        /// 加载/获取 一个或多个对象物体
        /// </summary>
        /// <param name="varTasks"></param>
        public void LoadObj(List<LoadTask> varTasks, Handle varHandle)
        {
            if (varTasks == null || varTasks.Count == 0)
            {
                Helper.LogError("GameObjPool LoadObj:Error caused by null List<LoadTask> instance");
                return;
            }
            varHandle.TaskCount = varTasks.Count;
            for (int i = 0; i < varTasks.Count; i++)
            {
                LoadTask task = varTasks[i];
                if (task == null)
                {
                    continue;
                }
                task.pHandle = varHandle;
                LoadObj(task);
            }
            varTasks.Clear();
            varTasks = null;
        }


        public void GetObj()
        {

        }

        #endregion
    }
}
