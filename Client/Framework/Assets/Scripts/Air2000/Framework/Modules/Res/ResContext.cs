using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using Air2000.Modular;

namespace Air2000.Res
{
    #region [enum] ResourceMode
    public enum ResourceMode
    {
        Debug_Resources,
        Debug_SimulativeAssetBundle,
        Debug_AssetBundleServer,
        Debug_PackagedAssetBundle,

        Release_Resources,
        Release_PackagedAssetBundle,
        Release_AssetBundleServer,
    }
    #endregion

    #region [delegate]
    public delegate void LoadAssetCallback(UnityEngine.Object obj, ResourceLoadParam param);
    public delegate void AssetBundleManagerInitializeFinish();
    #endregion

    #region [class] ResourceManager
    public class ResContext : ServiceContext
    {
        #region [Fields]
        private static string AssetBundleDownloadingURL = string.Empty;
        private static string[] mActiveVariants = { };
        private static AssetBundleManifest AssetBundleManifest = null;

        private static ResourceMode mResourceMode;
        private static ResourceManagerOptions mOptionsData;
        public static string ResourceManagerConfigName = "ResourceManagerOptions";
        public static string ResourceManagerResFolder = "Assets/Scripts/Air2000/ResourceManager/Editor/Res";

        private static Dictionary<string, LoadedAssetBundle> LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        private static Dictionary<string, WWW> Downloadings = new Dictionary<string, WWW>();
        private static Dictionary<string, string> DownloadingErrors = new Dictionary<string, string>();
        private static List<LoadOperation> InProgressOperations = new List<LoadOperation>();
        private static List<LoadOperation> WaitingOperations = new List<LoadOperation>();
        private static List<LoadOperation> UIOperations = new List<LoadOperation>();
        private static List<AssetBundleLoadLevelOperation> LoadLevelOperations = new List<AssetBundleLoadLevelOperation>();
        private static Dictionary<string, string[]> Dependencies = new Dictionary<string, string[]>();
        public static event AssetBundleManagerInitializeFinish ListenInitializeFinish;
        private static StringBuilder mBuilder = new StringBuilder();
#if UNITY_EDITOR
        private static GameObject WaitingOperationDebugObj;
#endif
        public static string[] ActiveVariants
        {
            get { return mActiveVariants; }
            set { mActiveVariants = value; }
        }
        public static AssetBundleManifest pAssetBundleManifest
        {
            get { return AssetBundleManifest; }
            set { AssetBundleManifest = value; }
        }
        public static ResourceManagerOptions OptionsData
        {
            get
            {
                if (mOptionsData == null)
                {
                    mOptionsData = ReadOptionsData();
                }
                return mOptionsData;
            }
            set { mOptionsData = value; }
        }
        public static ResContext Instance { set; get; }
        public static bool IsInitialized { get; set; }
        public static ResourceMode pAssetBundleMode
        {
            get
            {
#if ASSETBUNDLE_MODE
                return ResourceMode.Debug_PackagedAssetBundle;
#else
                return ResourceMode.Debug_Resources;
#endif
                return ResourceMode.Debug_PackagedAssetBundle;
                //return ResourceMode.Debug_SimulativeAssetBundle;
                if (Application.isMobilePlatform || Application.isConsolePlatform)
                {
                    return ResourceMode.Debug_PackagedAssetBundle;
                    //mAssetBundleMode = AssetBundleMode.Release_PackagedAssetBundle;
                    //return mAssetBundleMode;
                }
                else
                {
                    return ResourceMode.Debug_SimulativeAssetBundle;
                }
                //if (mOptionsData == null) { mOptionsData = ReadOptionsData(); }

                //if (mOptionsData != null) { return mOptionsData.AssetBundleMode; }
                //else { return mAssetBundleMode; }
            }
            set { mResourceMode = value; }
        }
        public static bool IsResourceMode
        {
            get
            {
#if ASSETBUNDLE_MODE
                return false;
#else
                return true;
#endif
                //if (pAssetBundleMode == ResourceMode.Release_Resources
                //    || pAssetBundleMode == ResourceMode.Debug_Resources || pAssetBundleMode == ResourceMode.Debug_SimulativeAssetBundle)
                //{
                //    return true;
                //}
                //else { return false; }
            }
        }
        #endregion

        #region [Functions]

        #region monobehaviour
        void Awake()
        {
            Instance = this;
            Air2000.AppEnterance.GetInstance().StartCoroutine(LoadLooper());
        }
        void OnEnable() { }
        void Start() { }
        void OnDisable() { }
        void OnDestroy() { }
        private static bool IsLooperActive = false;
        public static int MAX_LOAD_COUNT = 10;
        public static bool BoolFastMode = false;
        public static List<LoadOperation> CurrentOperations = new List<LoadOperation>();
        IEnumerator LoadLooper()
        {
            while (true)
            {
                //if (mLoadLevelOperations.Count > 0)
                //{
                //    AssetBundleLoadLevelOperation operation = mLoadLevelOperations[0];
                //    mInProgressOperations.Add(operation);
                //    operation.Execute();
                //    ResourceManagerUtility.LogDebug("Looper yield return new Coroutine(AssetBundleLoadLevelOperation)  LevelName: " + operation.mLevelName);
                //    yield return StartCoroutine(operation);
                //    ResourceManagerUtility.LogDebug("Looper success  LevelName: " + operation.mLevelName);
                //    try
                //    {
                //        operation.Callback(operation.GetAsset());
                //    }
                //    catch (Exception e)
                //    {
                //        if (operation.GetType().IsSubclassOf(typeof(LoadAssetOperation)))
                //        {
                //            ResourceManagerUtility.LogError("Looper success,but some exception is occuried in callback, LevelName: " + operation.mLevelName + " Exception: " + e.Message);
                //        }
                //    }
                //    mLoadLevelOperations.Remove(operation);
                //}
                if (UIOperations.Count > 0)
                {
                    LoadOperation operation = UIOperations[0];
                    InProgressOperations.Add(operation);
                    operation.Execute();
                    Utility.LogDebug("Looper yield return new coroutine  " + (operation as LoadAssetOperation).AssetBundleName);
                    yield return AppEnterance.GetInstance().StartCoroutine(operation);
                    Utility.LogDebug("Looper success  " + (operation as LoadAssetOperation).AssetBundleName);
                    try
                    {
                        operation.Finish(operation.GetAsset());
                    }
                    catch (Exception e)
                    {
                        if (operation.GetType().IsSubclassOf(typeof(LoadAssetOperation)))
                        {
                            Utility.LogError("Looper success,but some exception occuried: " + (operation as LoadAssetOperation).AssetBundleName + " Exception: " + e.Message);
                        }
                    }
                    UIOperations.Remove(operation);
                    if (UIOperations.Count > 0)
                    {
                        yield return null;
                    }
                }
                if (BoolFastMode)
                {
                    if (CurrentOperations.Count > 0)
                    {
                        for (int i = 0; i < CurrentOperations.Count; i++)
                        {
                            LoadOperation operation = CurrentOperations[i];
                            if (operation.IsDone())
                            {
                                try
                                {
                                    //ResourceManagerUtility.LogDebug("Looper success  " + (operation as LoadAssetOperation).AssetBundleName);
                                    operation.Finish(operation.GetAsset());
                                }
                                catch (Exception e)
                                {
                                    if (operation.GetType().IsSubclassOf(typeof(LoadAssetOperation)))
                                    {
                                        Utility.LogError("Looper success,but some exception occuried: " + (operation as LoadAssetOperation).AssetBundleName + " Exception: " + e.Message);
                                    }
                                }
                                WaitingOperations.Remove(operation);
                                CurrentOperations.Remove(operation);
                                i--;
                            }
                            //else
                            //{
                            //    i++;
                            //}
                        }
                    }
                    if (CurrentOperations.Count == MAX_LOAD_COUNT)
                    {
                        Utility.LogError("Full task, current waiting count: " + WaitingOperations.Count);
                        yield return null;
                    }
                }
                if (WaitingOperations.Count > 0)
                {
                    if (BoolFastMode)
                    {
                        int finalCount = Mathf.Min(MAX_LOAD_COUNT - CurrentOperations.Count, WaitingOperations.Count);
                        for (int i = 0; i < finalCount; i++)
                        {
                            LoadOperation operation = WaitingOperations[i];
                            InProgressOperations.Add(operation);
                            CurrentOperations.Add(operation);
                            operation.Execute();
                            Utility.LogDebug("add task:   final count: " + finalCount + "  name : " + (operation as LoadAssetOperation).AssetBundleName);
                            AppEnterance.GetInstance().StartCoroutine(operation);
                        }
                        yield return null;
                    }
                    else
                    {
                        LoadOperation operation = WaitingOperations[0];
                        InProgressOperations.Add(operation);
                        operation.Execute();
                        Utility.LogDebug("Looper yield return new coroutine  " + (operation as LoadAssetOperation).AssetBundleName);
                        yield return AppEnterance.GetInstance().StartCoroutine(operation);
                        Utility.LogDebug("Looper success  " + (operation as LoadAssetOperation).AssetBundleName);
                        try
                        {
                            operation.Finish(operation.GetAsset());
                        }
                        catch (Exception e)
                        {
                            LoadAssetOperation loadAssetOperation = operation as LoadAssetOperation;
                            if (loadAssetOperation != null && loadAssetOperation.GetType().IsSubclassOf(typeof(LoadAssetOperation)))
                            {
                                if (string.IsNullOrEmpty(loadAssetOperation.AssetBundleName) == false && string.IsNullOrEmpty(loadAssetOperation.AssetName) == false)
                                {
                                    Utility.LogError("Looper success,but some exception occuried, Task key: " + loadAssetOperation.AssetBundleName + "/" + loadAssetOperation.AssetName + ", Exception: " + e.Message + ", Track: " + e.StackTrace);
                                }
                                else
                                {
                                    Utility.LogError("Looper success,but some exception occuried, Exception: " + e.Message + ", Track: " + e.StackTrace);
                                }
                            }
                        }
                        WaitingOperations.Remove(operation);
                    }
                }
                yield return null;
            }
        }
        void Update()
        {
            var keysToRemove = new List<string>();
            foreach (var keyValue in Downloadings)
            {
                WWW download = keyValue.Value;
                if (download.error != null)
                {
                    string str = default(string);
                    if (DownloadingErrors.TryGetValue(keyValue.Key, out str) == false)
                    {
                        Utility.LogError("WWW ERROR:  " + download.error);
                        DownloadingErrors.Add(keyValue.Key, string.Format("Failed downloading bundle {0} from {1}: {2}", keyValue.Key, download.url, download.error));
                        keysToRemove.Add(keyValue.Key);
                    }
                    continue;
                }
                if (download.isDone)
                {
                    AssetBundle bundle = download.assetBundle;
                    if (bundle == null)
                    {
                        string str = default(string);
                        if (DownloadingErrors.TryGetValue(keyValue.Key, out str) == false)
                        {
                            DownloadingErrors.Add(keyValue.Key, string.Format("{0} is not a valid asset bundle.", keyValue.Key));
                            keysToRemove.Add(keyValue.Key);
                        }
                        continue;
                    }
                    LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle));
                    keysToRemove.Add(keyValue.Key);
                }
            }
            foreach (var key in keysToRemove)
            {
                WWW download = Downloadings[key];
                Downloadings.Remove(key);
                download.Dispose();
            }
            for (int i = 0; i < InProgressOperations.Count;)
            {
                if (InProgressOperations[i].Update() == false)
                {
                    InProgressOperations.RemoveAt(i);
                }
                else
                    i++;
            }
//#if UNITY_EDITOR
//            if (WaitingOperationDebugObj == null)
//            {
//                WaitingOperationDebugObj = new GameObject();
//                WaitingOperationDebugObj.transform.SetParent(transform);
//            }
//            WaitingOperationDebugObj.name = "Waiting Operations " + WaitingOperations.Count;
//#endif
        }
        #endregion

        #region utility
        public static string GetStreamingAssetsPath()
        {
            if (Application.isEditor)
                return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
            else if (Application.isWebPlayer)
                return System.IO.Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/") + "/StreamingAssets";
            else if (Application.isMobilePlatform || Application.isConsolePlatform)
                return Application.streamingAssetsPath;
            else // For standalone player.
                return "file://" + Application.streamingAssetsPath;
        }
        public static void SetMobilePlatformAssetBundleLocalPath() { SetSourceAssetBundleDirectory("AssetBundles/" + Utility.GetPlatformName() + "/"); }
        public static void SetSourceAssetBundleDirectory(string relativePath) { AssetBundleDownloadingURL = GetStreamingAssetsPath() + "/" + relativePath; Utility.LogDebug("SetSourceAssetBundleDirectory: " + AssetBundleDownloadingURL); }
        public static void SetSourceAssetBundleURL(string absolutePath) { AssetBundleDownloadingURL = absolutePath + Utility.GetPlatformName() + "/"; }
        public static void SetDevelopmentAssetBundleServer()
        {
#if UNITY_EDITOR
            if (pAssetBundleMode == ResourceMode.Debug_SimulativeAssetBundle) return;
#endif
            TextAsset urlConfig = Resources.Load("AssetBundleServerURL") as TextAsset;
            string url = (urlConfig != null) ? urlConfig.text.Trim() : null;
            if (url == null || url.Length == 0)
            {
                Utility.LogError("Development Server URL could not be found.");
                //AssetBundleManager.SetSourceAssetBundleURL("http://localhost:7888/" + UnityHelper.GetPlatformName() + "/");
            }
            else
            {
                ResContext.SetSourceAssetBundleURL(url);
            }
        }
        protected static string RemapVariantName(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetBundleName)) return string.Empty;
            string[] bundlesWithVariant = AssetBundleManifest.GetAllAssetBundlesWithVariant();

            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                if (curSplit[0] != split[0])
                    continue;
                int found = System.Array.IndexOf(mActiveVariants, curSplit[1]);

                // If there is no active variant found. We still want to use the first 
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }
            if (bestFit == int.MaxValue - 1)
            {
                Utility.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
            }
            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex];
            }
            else
            {
                return assetBundleName;
            }
        }
        public static ResourceManagerOptions ReadOptionsData()
        {
            ResourceManagerOptions optionsData = (ResourceManagerOptions)Resources.Load(ResourceManagerConfigName);
            //if (optionsData == null)
            //{
            //    LogError("Fatal error: No AssetBundleOptionsData,please create it and re build,named: " + AssetBundleConfigName);
            //}
            return optionsData;
        }
        public static void InitStringBuilder(ref System.Text.StringBuilder builder, params System.Object[] array)
        {
            if (builder == null)
            {
                builder = new System.Text.StringBuilder();
            }
            else
            {
                builder.Remove(0, builder.Length);
            }
            if (array != null && array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    builder.Append(array[i]);
                }
            }
        }
        //        public static string GetDataPath()
        //        {
        //            if (IsResourceMode)
        //            {
        //#if UNITY_EDITOR

        //#if UNITY_ANDROID
        //                InitStringBuilder(ref mBuilder, Application.persistentDataPath, "/Android/res");
        //#elif UNITY_IPHONE
        //                InitStringBuilder(ref mBuilder, Application.persistentDataPath, "/OSXEditor/res");
        //#else
        //                InitStringBuilder(ref mBuilder, Application.persistentDataPath, "/Win/res");
        //#endif
        //                return mBuilder.ToString();
        //#endif

        //                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        //                {
        //                    InitStringBuilder(ref mBuilder, Application.dataPath, "/../AssetBundles/", Application.platform.ToString(), "/res");
        //                    return mBuilder.ToString();
        //                }
        //                else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //                {
        //                    InitStringBuilder(ref mBuilder, Application.dataPath, "/res");  // ***.app/res
        //                    return mBuilder.ToString();
        //                }
        //                else if (Application.platform == RuntimePlatform.Android)
        //                {
        //                    InitStringBuilder(ref mBuilder, Air2000.GameApplicationInformation.GetSingleton().TGetSDKCardPath(), "/", Air2000.GameApplicationInformation.GetSingleton().TGetPackID(), "/res");
        //                    return mBuilder.ToString();
        //                }
        //                else
        //                {
        //                    return mBuilder.ToString();
        //                }
        //            }
        //            else
        //            {
        //                InitStringBuilder(ref mBuilder, Application.dataPath, "/Resources");
        //                return mBuilder.ToString();
        //            }
        //        }
        public static string GetSavePath()
        {
#if UNITY_EDITOR

#if UNITY_ANDROID
            InitStringBuilder(ref mBuilder, Application.persistentDataPath, "/Android/saveData");
#elif UNITY_IPHONE
			 InitStringBuilder(ref mBuilder, Application.persistentDataPath, "/OSXEditor/saveData");
#else
            InitStringBuilder(ref mBuilder, Application.persistentDataPath, "/Win/saveData");
#endif
            return mBuilder.ToString();

#else
            string path = Application.persistentDataPath;
            //if (IsResourceMode)
            //{
            //    if (Application.platform == RuntimePlatform.WindowsPlayer)
            //    {
            //        return path;
            //    }
            //    else if (Application.platform == RuntimePlatform.IPhonePlayer)
            //    {
            //        return path;  // /Documents
            //    }
            //    else if (Application.platform == RuntimePlatform.Android)
            //    {
            //        //InitStringBuilder(ref mBuilder, NCSpeedLight.GameApplicationInformation.GetSingleton().TGetSDKCardPath(), "/", NCSpeedLight.GameApplicationInformation.GetSingleton().TGetPackID());
            //        //return mBuilder.ToString();
            //        ResourceManagerUtility.LogDebug("GetSavePath : " + path);
            //        return path;
            //    }
            //}
            return path;
#endif
        }

        #endregion

        #region initialize
        public static AssetBundleLoadManifestOperation Initialize()
        {
            return Initialize(Utility.GetPlatformName());
        }
        public static AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName)
        {
            if (IsInitialized)
            {
                Utility.LogWarning("Already initialized");
                return null;
            }
            //if (OptionsData == null) { Log("Open AssetBundleOptions data"); }
            Utility.LogDebug("Resource Mode: " + pAssetBundleMode.ToString());
            //            if (Application.isConsolePlatform || Application.isMobilePlatform)
            //            {
            //                SetMobilePlatformAssetBundleLocalPath();
            //            }
            if (IsResourceMode == false)
            {
                SetMobilePlatformAssetBundleLocalPath();
            }
            GameObject go = new GameObject("ResManager [Mode: " + pAssetBundleMode + "]");
            GameObject.DontDestroyOnLoad(go);
            go.SetActive(true);
            //go.AddComponent<ResContext>();

            if (pAssetBundleMode == ResourceMode.Debug_SimulativeAssetBundle
                || pAssetBundleMode == ResourceMode.Debug_Resources || pAssetBundleMode == ResourceMode.Release_Resources)
            {
                if (ListenInitializeFinish != null)
                {
                    ListenInitializeFinish();
                }
                return null;
            }
            LoadAssetBundle(manifestAssetBundleName, true);
            var operation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
            operation.Callback = Initialized;
            WaitingOperations.Add(operation);
            return operation;
        }
        protected static void Initialized(object obj, object param)
        {
            IsInitialized = true;
            if (ListenInitializeFinish != null)
            {
                ListenInitializeFinish();
            }
        }
        #endregion

        #region asset bundle and dependency
        public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
        {
            if (DownloadingErrors.TryGetValue(assetBundleName, out error))
                return null;
            LoadedAssetBundle bundle = null;
            LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
                return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!Dependencies.TryGetValue(assetBundleName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                if (DownloadingErrors.TryGetValue(assetBundleName, out error))
                    return bundle;

                // Wait all the dependent assetBundles being loaded.
                LoadedAssetBundle dependentBundle;
                LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                    return null;
            }
            return bundle;
        }
        protected static void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
        {
            Utility.LogDebug("Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);
#if UNITY_EDITOR
            if (pAssetBundleMode == ResourceMode.Debug_SimulativeAssetBundle)
                return;
#endif
            if (!isLoadingAssetBundleManifest)
            {
                if (AssetBundleManifest == null)
                {
                    Utility.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                    return;
                }
            }
            bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);
            if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
                LoadDependencies(assetBundleName);
        }
        protected static bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest)
        {
            // Already loaded.
            LoadedAssetBundle bundle = null;
            LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                bundle.RefCount++;
                return true;
            }

            // @TODO: Do we need to consider the referenced count of WWWs?
            // In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
            // But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
            if (Downloadings.ContainsKey(assetBundleName))
                return true;

            WWW download = null;
            string url = AssetBundleDownloadingURL + assetBundleName;

            // For manifest assetbundle, always download it as we don't have hash for it.
            if (isLoadingAssetBundleManifest)
                download = new WWW(url);
            else
                download = WWW.LoadFromCacheOrDownload(url, AssetBundleManifest.GetAssetBundleHash(assetBundleName), 0);

            Downloadings.Add(assetBundleName, download);

            return false;
        }
        protected static void LoadDependencies(string assetBundleName)
        {
            if (AssetBundleManifest == null)
            {
                Utility.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }
            string[] dependencies = AssetBundleManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
                return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            Dependencies.Add(assetBundleName, dependencies);
            for (int i = 0; i < dependencies.Length; i++)
                LoadAssetBundleInternal(dependencies[i], false);
        }
        public static void UnloadAssetBundle(string assetBundleName)
        {
            //#if UNITY_EDITOR
            //            return;
            //#else
            if (IsResourceMode) return;
            //#endif
            UnloadAssetBundleInternal(assetBundleName);
            UnloadDependencies(assetBundleName);
        }
        protected static void UnloadDependencies(string assetBundleName)
        {
            string[] dependencies = null;
            if (!Dependencies.TryGetValue(assetBundleName, out dependencies))
                return;
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency);
            }
            Dependencies.Remove(assetBundleName);
        }
        protected static void UnloadAssetBundleInternal(string assetBundleName)
        {
            string error;
            LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (bundle == null)
                return;
            if (--bundle.RefCount <= 0)
            {
                bundle.AssetBundle.Unload(false);
                LoadedAssetBundles.Remove(assetBundleName);
                Utility.LogDebug(assetBundleName + " has been unloaded successfully");
            }
        }
        #endregion

        #region load method
        public static LoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type, LoadAssetCallback callback = null, object callbackParam = null)
        {
            return LoadAssetAsync(assetBundleName, assetName, type, false, callback, callbackParam);
        }
        public static LoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type, bool preferential, LoadAssetCallback callback = null, object callbackParam = null)
        {
            LoadAssetOperation operation = null;
            if (pAssetBundleMode == ResourceMode.Debug_SimulativeAssetBundle)
            {
#if UNITY_EDITOR
                operation = new AssetBundleLoadAssetOperationSimulation(null);

                if (string.IsNullOrEmpty(assetName) || assetName.Equals("NULL") || assetName.Equals("null"))
                {

                }
                else
                {
                    string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                    if (assetPaths.Length == 0)
                    {
                        Utility.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                        if (callback != null)
                        {
                            callback(null, callbackParam as ResourceLoadParam);
                        }
                        return null;
                    }
                    UnityEngine.Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
                    operation.LoadedAsset = target;
                }
                operation.Callback = callback;
                operation.CallbackParam = callbackParam;
                if (preferential)
                {
                    WaitingOperations.Insert(0, operation);
                }
                else
                {
                    WaitingOperations.Add(operation);
                }
                return operation;
#endif
            }
            else if (pAssetBundleMode == ResourceMode.Release_Resources || pAssetBundleMode == ResourceMode.Debug_Resources)
            {
                operation = new ResourceLoadAssetOperation() { AssetName = assetName, AssetBundleName = assetBundleName, AssetType = type };
                operation.Callback = callback;
                operation.CallbackParam = callbackParam;
                if (preferential)
                {
                    WaitingOperations.Insert(0, operation);
                }
                else
                {
                    WaitingOperations.Add(operation);
                }
                Utility.LogDebug("LoadAssetAsync: " + assetBundleName + "   mWaitingOperations: " + WaitingOperations.Count);
            }
            else
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundle(assetBundleName);
                operation = new AssetBundleLoadAssetOperation(assetBundleName, assetName, type);
                operation.Callback = callback;
                operation.CallbackParam = callbackParam;
                if (preferential)
                {
                    WaitingOperations.Insert(0, operation);
                }
                else
                {
                    WaitingOperations.Add(operation);
                }
            }
            return operation;
        }
        public static LoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
        {
            Utility.LogDebug("Loading " + levelName + " from " + assetBundleName + " bundle");
            LoadOperation operation = null;
            if (pAssetBundleMode == ResourceMode.Debug_SimulativeAssetBundle
                || pAssetBundleMode == ResourceMode.Debug_Resources || pAssetBundleMode == ResourceMode.Release_Resources)
            {
                operation = new LoadLevelSimulationOperation(assetBundleName, levelName, isAdditive);
            }
            else
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundle(assetBundleName);
                operation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);
                InProgressOperations.Add(operation);
            }
            return operation;
        }
        #endregion

        #region other
        public static LoadAssetOperation IsInProgress(string assetbundleName, string assetName)
        {
            if (InProgressOperations != null && InProgressOperations.Count > 0)
            {
                for (int i = 0; i < InProgressOperations.Count; i++)
                {
                    LoadAssetOperation operation = InProgressOperations[i] as LoadAssetOperation;
                    if (operation == null) continue;
                    if (IsResourceMode)
                    {
                        if (string.IsNullOrEmpty(operation.AssetBundleName)) continue;
                        if (operation.AssetBundleName.Equals(assetbundleName)) return operation;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(operation.AssetBundleName) || string.IsNullOrEmpty(operation.AssetName)) continue;
                        if (operation.AssetBundleName.Equals(assetbundleName) && operation.AssetName.Equals(assetName)) return operation;
                    }
                }
            }
            if (WaitingOperations != null && WaitingOperations.Count > 0)
            {
                for (int i = 0; i < WaitingOperations.Count; i++)
                {
                    LoadAssetOperation operation = WaitingOperations[i] as LoadAssetOperation;
                    if (operation == null) continue;
                    if (IsResourceMode)
                    {
                        if (string.IsNullOrEmpty(operation.AssetBundleName)) continue;
                        if (operation.AssetBundleName.Equals(assetbundleName)) return operation;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(operation.AssetBundleName) || string.IsNullOrEmpty(operation.AssetName)) continue;
                        if (operation.AssetBundleName.Equals(assetbundleName) && operation.AssetName.Equals(assetName)) return operation;
                    }
                }
            }
            if (UIOperations != null && UIOperations.Count > 0)
            {
                for (int i = 0; i < UIOperations.Count; i++)
                {
                    LoadAssetOperation operation = UIOperations[i] as LoadAssetOperation;
                    if (operation == null) continue;
                    if (IsResourceMode)
                    {
                        if (string.IsNullOrEmpty(operation.AssetBundleName)) continue;
                        if (operation.AssetBundleName.Equals(assetbundleName)) return operation;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(operation.AssetBundleName) || string.IsNullOrEmpty(operation.AssetName)) continue;
                        if (operation.AssetBundleName.Equals(assetbundleName) && operation.AssetName.Equals(assetName)) return operation;
                    }
                }
            }
            return null;
        }
        #endregion

        #endregion
    }
    #endregion
}