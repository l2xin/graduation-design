/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: WindowManager.cs
			// Describle: 窗口管理
			// Created By:  彭国安
			// Date&Time:  2015/3/26 10:34:18
            // Modify History:
            // 陈伟超_20150330:打开，关闭，隐藏window, 调整将最大深度的窗口层级, 保证只有一个窗口能响应事件
            // 陈伟超_20150402:窗口获取加入null判断; 每个窗口的结构应该是UIRoot->Camera, GameObject, 每次打开界面取出GameObject做为窗口对象;
			// 陈伟超_20150403:修改窗口移动到最前面和窗口移动到最后面逻辑， 打开已经存在的窗口，则将此窗口移动到最前面，重新设置层级;
			// 钟建辉_20150414:加入公用弹框的打开逻辑，增加window节点和Dialog节点用于存放不同类型的窗口;
			// 陈伟超_20150420:打开窗口和销毁窗口发事件通知蒙板变化;
			// 陈伟超_20150427:增加接口获取已经激活的窗口数量;
            // 钟建辉_20150513:修改为获取UI下面的所有panel（包括隐藏的）;
			// 钟建辉_20150610:加入UIRoot的最小和最大的屏幕高度属性;
			// 陈伟超_20150610:返回当前缩放比的屏幕分辨率;
			// 卢涛_20150611:OpenWindow增加资源路径依赖;
			// 陈伟超_20150813:存储所有窗口的panel，记录它们的深度, 避免反复调用GetComponenInChildren;
            //
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Air2000;
using Air2000.UI;
using GTools.Res;
using GTools.ImageEffect;

namespace Air2000
{
    /// <summary>
    /// 蒙板
    /// </summary>
    public class BlackMask
    {
        private static BlackMask mInstance;//类实例
        private BlackMaskAlpha mMaskType = BlackMaskAlpha.BMA_Dark;  // 默认85%的蒙板;

        public enum BlackMaskAlpha
        {
            BMA_Dark,
            BMA_Medium,
        }

        public static BlackMask GetSingleton()
        {
            if (mInstance == null)
            {
                mInstance = new BlackMask();
            }
            return mInstance;
        }

        public BlackMaskAlpha pMaskType
        {
            set
            {
                mMaskType = value;
            }
        }

        public float pAlpha
        {
            get
            {
                if (mMaskType == BlackMaskAlpha.BMA_Dark)
                {
                    return 0.85f;
                }
                else
                {
                    return 0.6f;
                }
            }
        }

        /// <summary>
        /// 是否显示蒙板
        /// </summary>
        public bool pIsShowMask
        {
            get { return (pActiveWinNum > 1); }
        }

        public int pActiveWinNum
        {
            get;
            set;
        }

        /// <summary>
        /// 上一个打开界面的最大深度;
        /// </summary>
        public int pMaxDepth
        {
            get;
            set;
        }

        /// <summary>
        /// 临时变量存储窗口深度;
        /// </summary>
        public List<int> pDepthList
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 打开窗口的回调
    /// </summary>
    /// <param name="view">打开的窗口</param>
    /// <param name="param">回传的参数</param>
    public delegate void OpenWindowCallBack(GameObject view, GTools.Res.ResourceLoadParam param);

    /// <summary>
    /// 窗口管理
    /// </summary>
    public class WindowManager
    {
        /// <summary>
        ///所有打开的Dialog的集合.
        /// </summary>
        private Dictionary<string, GameObject> mDialogs;
        /// <summary>
        ///所有加载了的Dialog原始的集合.
        /// </summary>
        private Dictionary<string, GameObject> mAllLoadDialogs;
        /// <summary>
        /// 忽略清除的dialog名字集合.
        /// </summary>
        private List<string> mIgnoreClearDialogs;
        /// <summary>
        /// 所有Dialog的父节点.
        /// </summary>
        private GameObject mDialogRoot = null;
        /// <summary>
        /// Dialog的最小Depth.
        /// </summary>
        private static readonly int mDialogMinDepth = 1000;
        /// <summary>
        /// 记录Dialog的最大Depth.
        /// </summary>
        private int mDialogMaxDepth = 1000;
        /// <summary>
        ///记录每一个Dialog的Depth.
        /// </summary>
        private Dictionary<string, int> mAllDialogsDeth = new Dictionary<string, int>();
        /// <summary>
        ///所有加载了的window的集合 
        /// </summary>
        public Dictionary<string, GameObject> mWindows;

        /// <summary>
        /// window的所有panel.
        /// </summary>
        private Dictionary<string, UIPanel[]> mWinPanels;

        /// <summary>
        /// 所有Window的父节点.
        /// </summary>
        private GameObject mWindowRoot = null;
        /// <summary>
        ///记录每一个窗口的Depth
        /// </summary>
        private Dictionary<string, int> winNameToFirstDepth = new Dictionary<string, int>();
        //private static Dictionary<string,int> winNameTLastDepth = new Dictionary<string,int> ();
        private static readonly int INIT_DEPTH = 0;
        /// <summary>
        //记录最大Depth
        /// </summary>
        private int mMaxDepth = INIT_DEPTH;

        /// <summary>
        //窗口单例
        /// </summary>
        static WindowManager mSingle = null;
        GameObject mRootObject = null;

        /// <summary>
        /// The m_ user interface root.
        /// </summary>
        private UIRoot mUIRoot = null;

        private int mRootMaxHeight = 1224;

        private int mRootMinHeight = 720;

        private UIPanel mRootPanel = null;

        private float mScreenScale;
        /// <summary>
        /// UI层级.
        /// </summary>
        private int mUILayer;

        /// <summary>
        /// 自定义UI层级
        /// </summary>
        private int mCustomUILayer;

        /// <summary>
        /// Dialog层级.
        /// </summary>
        private int mDialogLayer;
        /// <summary>
        /// UI摄像机UICamera实例对象.
        /// </summary>
        private UICamera mUICamera;
        /// <summary>
        /// UI摄像机Camera实例对象.
        /// </summary>
        private Camera mCamera;

        public Camera pCamera
        {
            get { return mCamera; }
        }

        public UICamera pUICamera
        {
            get
            {
                return mUICamera;
            }
        }

        GlobalEventManager mGlobalEventManager;  // 监听界面打开/关闭;
        GlobalEventQueue mGlobalEventQueue;

        public GameObject pRootObject
        {
            get
            {
                return mRootObject;
            }
        }

        public int pRootMaxHeight
        {
            get { return mRootMaxHeight; }
        }

        public int pRootMinHeight
        {
            get { return mRootMinHeight; }
        }
        /// <summary>
        /// 获取当前分辨率的屏幕缩放比.
        /// </summary>
        /// <value>The p screen scale.</value>
        public float pScreenScale
        {
            get
            {
                //mScreenScale = 1.0f;
                //if (Screen.height > mRootMaxHeight)
                //{
                //    mScreenScale = (float)mRootMaxHeight / (float)Screen.height;
                //}
                //else if (Screen.height < mRootMinHeight)
                //{
                //    mScreenScale = (float)mRootMinHeight / (float)Screen.height;
                //}

                float temscale = (float)mRootMinHeight / (float)Screen.height;

                return temscale;
                //return mScreenScale;
            }
        }
        /// <summary>
        /// 返回当前缩放比的屏幕分辨率.
        /// </summary>
        /// <value>The p screen resolution.</value>
        public Vector2 pScreenResolution
        {
            get
            {
                Vector2 tempScreenVec = Vector2.zero;
                tempScreenVec.x = Screen.width;
                tempScreenVec.y = Screen.height;

                if (mRootMinHeight > tempScreenVec.y)
                {
                    tempScreenVec.x = tempScreenVec.x / tempScreenVec.y * mRootMinHeight;
                    tempScreenVec.y = mRootMinHeight;
                }
                else if (mRootMaxHeight < tempScreenVec.y)
                {
                    tempScreenVec.x = tempScreenVec.x / tempScreenVec.y * mRootMaxHeight;
                    tempScreenVec.y = mRootMaxHeight;
                }

                return tempScreenVec;
            }
        }

        /// <summary>
        //窗口单例实现方法
        /// </summary>
        static public WindowManager GetSingleton()
        {
            if (mSingle == null)
            {
                mSingle = new WindowManager();
            }
            return mSingle;

        }


        /// <summary>
        /// Windows the manager.
        /// 构造一个NGUI最基本的对象UIRoot并设置好相关参数
        /// </summary>
        WindowManager()
        {
            mWindows = new Dictionary<string, GameObject>();
            mDialogs = new Dictionary<string, GameObject>();
            mAllLoadDialogs = new Dictionary<string, GameObject>();
            InitIgnoreDialogs();
            mCustomUILayer = LayerMask.NameToLayer("FXQUI");
            mDialogLayer = LayerMask.NameToLayer("FXQDialog");
            mUILayer = LayerMask.NameToLayer("UI");

            mRootObject = new GameObject("UIRootObject");
            mUIRoot = mRootObject.AddComponent<UIRoot>();
            mRootPanel = mRootObject.AddComponent<UIPanel>();

            mMaxDepth = mRootPanel.depth;
            mUIRoot.scalingStyle = UIRoot.Scaling.ConstrainedOnMobiles;
            //mUIRoot.maximumHeight = mRootMaxHeight;
            //mUIRoot.minimumHeight = mRootMinHeight;
            mUIRoot.manualWidth = mRootMaxHeight;
            mUIRoot.manualHeight = mRootMinHeight;
            mRootObject.layer = mUILayer;

            GameObject tempCamObject = new GameObject("UICamera");
            tempCamObject.layer = mUILayer;
            tempCamObject.transform.localPosition = Vector3.back;
            tempCamObject.transform.SetParent(mRootObject.transform);

            mCamera = tempCamObject.AddComponent<Camera>();
            mCamera.orthographic = true;
            mCamera.orthographicSize = 1.0f;
            mCamera.cullingMask = Helper.OnlyIncluding(mCustomUILayer, mUILayer, mDialogLayer);
            mCamera.nearClipPlane = -10f;
            mCamera.depth = 1;
            mCamera.clearFlags = CameraClearFlags.Nothing;
            UIGaussianBlurEffect effect = tempCamObject.AddComponent<UIGaussianBlurEffect>();
            effect.BlurSize = 4;
            effect.TextureSample = 2;

            /*---创建Window窗口父节点---*/
            mWindowRoot = new GameObject("WindowRoot");
            mWindowRoot.transform.SetParent(mRootObject.transform);
            mWindowRoot.layer = mUILayer;

            /*---创建Dialog窗口父节点---*/
            mDialogRoot = new GameObject("DialogRoot");
            mDialogRoot.transform.SetParent(mRootObject.transform);
            mDialogRoot.layer = mDialogLayer;

            GameObject.DontDestroyOnLoad(mRootObject);

            mUICamera = tempCamObject.AddComponent<UICamera>();

            mUICamera.eventReceiverMask = Helper.EverythingBut(mUILayer);
        }

        private void InitIgnoreDialogs()
        {
            mIgnoreClearDialogs = new List<string>();
            mIgnoreClearDialogs.Add("Scene_Guide/guide");
            mIgnoreClearDialogs.Add("Scene_Guide/guideHand");
            mIgnoreClearDialogs.Add("Scene_Guide/unlock");
            mIgnoreClearDialogs.Add("Scene_Drama/ui_drama");
            mIgnoreClearDialogs.Add("Scene_Load/Load");
        }

        public void SetCameraClearBack(bool b)
        {
            if (mUICamera != null && mUICamera.cachedCamera != null)
            {
                if (b)
                {
                    mUICamera.cachedCamera.clearFlags = CameraClearFlags.Color;
                    mUICamera.cachedCamera.backgroundColor = Color.black;
                }
                else
                {
                    mUICamera.cachedCamera.clearFlags = CameraClearFlags.Nothing;

                }
            }
        }

        private GameObject SetUI(GameObject varObj)
        {
            if (varObj == null)
            {
                Helper.LogError("window load Failed");
                return null;
            }

            varObj = GameObject.Instantiate(varObj) as GameObject;

            Transform tempLoadObjTrans = varObj.transform;
            if (tempLoadObjTrans.childCount <= 1)
            {
                GameObject.DestroyObject(varObj);
                return null;
            }

            Transform temRoot = tempLoadObjTrans.GetChild(0); // LoadObject.transform.FindChild (varWindowName);
            Camera tempCam = temRoot.GetComponent<Camera>();
            if (tempCam != null)
            {
                temRoot = tempLoadObjTrans.GetChild(1);
            }

            if (temRoot == null)
            {
                Helper.LogError("load window can't find the RootObject:");
                GameObject.DestroyObject(varObj);
                return null;
            }

            GameObject tempWindow = temRoot.gameObject;

            temRoot.SetParent(mWindowRoot.transform);
            temRoot.localScale = Vector3.one;
            GameObject.DestroyObject(varObj);

            return tempWindow;
        }

        // 只是打开一个窗口，不做其他事;
        public void LoadWindow(string varWindowName, OpenWindowCallBack varCall, ResourceLoadParam varParam)
        {
            if (string.IsNullOrEmpty(varWindowName))
            {
                return;
            }

            string windowpath = Helper.Format("ui/{0}", varWindowName);

            LoadAssetCallback callback = delegate(UnityEngine.Object obj, ResourceLoadParam varParam1)
            {
                GameObject LoadObject = obj as GameObject;
                GameObject tempWindow = SetUI(LoadObject);
                if (varCall != null)
                {
                    varCall(tempWindow, varParam);
                }
            };

            int tempIndex = varWindowName.LastIndexOf("/");
            if (tempIndex >= 0)
            {
                //ResourceManager.GetSingleton().LoadUIAssetBundle(windowpath, varWindowName.Substring(tempIndex + 1), typeof(GameObject), callback);
                ResourceManager.LoadAssetAsync(windowpath, varWindowName.Substring(tempIndex + 1), typeof(GameObject), callback);

            }
            else
            {
                //ResourceManager.GetSingleton().LoadUIAssetBundle(windowpath, varWindowName, typeof(GameObject), callback);
                ResourceManager.LoadAssetAsync(windowpath, varWindowName, typeof(GameObject), callback);
            }
        }

        public void OpenWindow(string bundleName, string assetName, string key = null, OpenWindowCallBack callback = null, ResourceLoadParam param = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = bundleName + "/" + assetName;
            }
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(bundleName) || string.IsNullOrEmpty(assetName))
            {
                if (callback != null)
                {
                    callback(null, param);
                }
                return;
            }
            GameObject openWindow = GetWindowByName(key);
            if (openWindow != null)
            {
                if (openWindow.activeSelf == false)
                {
                    MoveWindowToFront(key);
                    WindowChange();
                }
                if (callback != null)
                {
                    callback(openWindow, param);
                }
                return;
            }
            LoadAssetCallback OpenViewCallback = delegate(UnityEngine.Object obj, ResourceLoadParam resParam)
           {
               GameObject LoadObject = obj as GameObject;
               if (LoadObject == null)
               {
                   Utility.LogError("Open window fail");
               }
               openWindow = SetUI(LoadObject);
               if (openWindow == null)
               {
                   Utility.LogError("Open Window fail 1");
                   if (callback != null)
                   {
                       callback(openWindow, param);
                   }
                   return;
               }

               if (mWindows.ContainsKey(key) == false)
               {
                   mWindows.Add(key, openWindow);
               }

               SetOtherWinToUI(openWindow);

               openWindow.SetActive(true);

               SetWindowDepth(openWindow, key);

               WindowChange();
               Utility.LogDebug("Open Window success");
               if (callback != null)
               {
                   callback(openWindow, param);
               }
           };

            Utility.LogDebug("Open Window: " + key);
            if (ResourceManager.IsResourceMode)
            {
                ViewResAdapter.Instance.LoadWindowWithResource(key, OpenViewCallback);
            }
            else
            {
                ViewResAdapter.Instance.LoadWindowWithAssetBundle(bundleName, assetName, OpenViewCallback);
            }

            //int tempIndex = varWindowName.LastIndexOf("/");
            //if (tempIndex >= 0)
            //{
            //    Utility.LogDebug("Open Window: " + windowpath);
            //    ViewResAdapter.Instance.LoadWindow(varWindowName.Substring(tempIndex + 1), OpenViewCallback);
            //}
            //else
            //{
            //    Utility.LogDebug("Open Window: " + windowpath);
            //    ViewResAdapter.Instance.LoadWindow(bundleName, assetName, OpenViewCallback);
            //}
        }

        /// <summary>
        /// 打开一个窗口，放在最前面.
        /// </summary>
        /// <returns>The window.</returns>
        /// <param name="varWindowName">Variable window name.</param>
        //public void OpenWindow(string varWindowName, OpenWindowCallBack varCall, ResourceLoadParam varParam)
        //{
        //    if (string.IsNullOrEmpty(varWindowName))
        //    {
        //        return;
        //    }

        //    GameObject openWindow = GetWindowByName(varWindowName);
        //    if (openWindow != null)
        //    {
        //        if (openWindow.activeSelf == false)
        //        {
        //            MoveWindowToFront(varWindowName);
        //            WindowChange();
        //        }

        //        if (varCall != null)// && varCall.Target!= null) 
        //        {
        //            varCall(openWindow, varParam);
        //        }
        //        return;
        //        //return openWindow;
        //    }
        //    string windowpath = Helper.Format("UI/{0}", varWindowName);

        //    //if (ResourceManager.GetSingleton().IsInObjectLoadList(windowpath))
        //    //{
        //    //    return;
        //    //}
        //    if (GTools.Res.ViewResAdapter.Instance.IsInProgress(windowpath, null, null) != null) { return; }

        //    GTools.Res.LoadAssetCallback callback = delegate(UnityEngine.Object obj, GTools.Res.ResourceLoadParam varParam1)
        //    {
        //        GameObject LoadObject = obj as GameObject;
        //        if (LoadObject == null)
        //        {
        //            Utility.LogError("Open window fail");
        //        }
        //        openWindow = SetUI(LoadObject);
        //        if (openWindow == null)
        //        {
        //            Utility.LogError("Open Window fail 1");
        //            if (varCall != null)
        //            {
        //                varCall(openWindow, varParam);
        //            }
        //            return;
        //        }

        //        if (mWindows.ContainsKey(varWindowName) == false)
        //        {
        //            mWindows.Add(varWindowName, openWindow);
        //        }

        //        SetOtherWinToUI(openWindow);

        //        openWindow.SetActive(true);

        //        SetWindowDepth(openWindow, varWindowName);

        //        WindowChange();
        //        Utility.LogDebug("Open Window success");
        //        if (varCall != null)
        //        {
        //            varCall(openWindow, varParam);
        //        }
        //    };


        //    int tempIndex = varWindowName.LastIndexOf("/");
        //    if (tempIndex >= 0)
        //    {
        //        Utility.LogDebug("Open Window: " + windowpath);
        //        //GTools.Res.ViewResAdapter.Instance.LoadViewAssetAsync(windowpath, null, varWindowName.Substring(tempIndex + 1), typeof(GameObject), callback);
        //        //GTools.Res.ResourceManager.LoadAssetAsync(windowpath, varWindowName.Substring(tempIndex + 1), typeof(GameObject), callback);
        //        //ViewResAdapter.Instance.LoadWindow(varWindowName.Substring(tempIndex + 1), callback);
        //    }
        //    else
        //    {
        //        Utility.LogDebug("Open Window: " + windowpath);
        //        //GTools.Res.ViewResAdapter.Instance.LoadViewAssetAsync(windowpath, null, varWindowName, typeof(GameObject), callback);
        //        //ViewResAdapter.Instance.LoadWindow(varWindowName, callback);
        //        //GTools.Res.ResourceManager.LoadAssetAsync(windowpath, varWindowName, typeof(GameObject), callback);
        //    }
        //}
        public void OpenWindow(string viewKey, OpenWindowCallBack callback = null, ResourceLoadParam param = null)
        {
            if (string.IsNullOrEmpty(viewKey) == false)
            {
                int strIndex = viewKey.LastIndexOf("/");
                string bundleName, assetName;
                if (strIndex >= 0)
                {
                    bundleName = viewKey.Substring(0, strIndex);
                    assetName = viewKey.Substring(strIndex + 1);
                }
                else
                {
                    bundleName = viewKey;
                    assetName = viewKey;
                }
                OpenWindow(bundleName, assetName, viewKey, callback, param);
            }
            else
            {
                if (callback != null)
                {
                    callback(null, param);
                }
            }
        }

        /// <summary>
        /// [兼容旧版本] 打开一个窗口，放在最前面.
        /// </summary>
        /// <returns>The window.</returns>
        /// <param name="viewKey">Variable window name.</param>
        //public void OpenWindow(string viewKey)
        //{
        //    if (string.IsNullOrEmpty(viewKey) == false)
        //    {
        //        int strIndex = viewKey.LastIndexOf("/");
        //        string bundleName, assetName;
        //        if (strIndex >= 0)
        //        {
        //            bundleName = viewKey.Substring(0, strIndex);
        //            assetName = viewKey.Substring(strIndex + 1);
        //        }
        //        else
        //        {
        //            bundleName = viewKey;
        //            assetName = viewKey;
        //        }
        //        OpenWindow(bundleName, assetName, viewKey);
        //    }
        //}

        /// <summary>
        /// 从Resources打开一个窗口, 放在最前面.
        /// </summary>
        /// <returns>The window from resources.</returns>
        /// <param name="varWindowName">Variable window name.</param>
        public GameObject OpenWindowFromResources(string varWindowName)
        {
            if (string.IsNullOrEmpty(varWindowName))
            {
                return null;
            }

            GameObject tempOpenWindow = GetWindowByName(varWindowName);
            if (tempOpenWindow != null)
            {
                if (tempOpenWindow.activeSelf == false)
                {
                    MoveWindowToFront(varWindowName);
                    WindowChange();
                }

                return tempOpenWindow;
            }
            GameObject tempObj = Resources.Load<GameObject>(varWindowName);
            tempOpenWindow = SetUI(tempObj);
            if (tempOpenWindow == null)
            {
                return null;
            }

            if (mWindows.ContainsKey(varWindowName) == false)
            {
                mWindows.Add(varWindowName, tempOpenWindow);
            }

            SetOtherWinToUI(tempOpenWindow);

            tempOpenWindow.SetActive(true);

            SetWindowDepth(tempOpenWindow, varWindowName);

            WindowChange();

            return tempOpenWindow;
        }

        /// <summary>
        /// 从Resources加载一个窗口，不做其他事.
        /// </summary>
        public GameObject LoadWindowFromResources(string varWindowName)
        {
            GameObject tempObj = Resources.Load<GameObject>(varWindowName);
            GameObject tempOpenWindow = SetUI(tempObj);

            return tempOpenWindow;
        }
        /// <summary>
        ///打开一个小的提示公用弹框.
        /// </summary>
        public void OpenSmallTipsDialog(string varContents)
        {
            //AudioManager.GetSingleton().PlayUIAudio(AudioName.AN_Warning);
            OpenWindowCallBack tempCallback = delegate(GameObject varObj, ResourceLoadParam varParam)
            {
                if (this == null)
                {
                    return;
                }

                if (varObj != null)
                {
                    SmallTipsDialog tempScript = varObj.GetComponent<SmallTipsDialog>();
                    if (tempScript == null)
                    {
                        tempScript = varObj.AddComponent<SmallTipsDialog>();
                    }
                    tempScript.SetContents(varContents);
                }
            };
            OpenDialog(SmallTipsDialog.mDialogName, false, false, tempCallback);
        }

        /// <summary>
        ///打开一个小的游戏内提示公用弹框.
        /// </summary>
        public void OpenBattleSmallDialog(string varContents)
        {
            //AudioManager.GetSingleton().PlayUIAudio(AudioName.AN_Warning);
            OpenWindowCallBack tempCallback = delegate(GameObject varObj, ResourceLoadParam varParam)
            {
                if (this == null)
                {
                    return;
                }

                if (varObj != null)
                {
                    BattleSmallDialog tempScript = varObj.GetComponent<BattleSmallDialog>();
                    if (tempScript == null)
                    {
                        tempScript = varObj.AddComponent<BattleSmallDialog>();
                    }
                    tempScript.SetContents(varContents);
                }
            };
#if ASSETBUNDLE_MODE
            OpenDialog("BattleSmallDialog", false, false, tempCallback);
#else
            OpenDialog("UI/Dialog/BattleSmallDialog", false, false, tempCallback);
#endif
        }
        /// <summary>
        ///打开一个标准的提示公用弹框(无操作按钮，5秒钟自动消失).
        /// </summary>
        public void OpenStandardDialog(string varContents)
        {
            OpenStandardDialog(varContents, null);
        }

        public void OpenStandardDialog(string varContents, EventDelegate.Callback varCloseCallback)
        {
            OpenWindowCallBack tempCallback = delegate(GameObject varObj, ResourceLoadParam varParam)
            {
                if (this == null)
                {
                    return;
                }

                if (varObj != null)
                {
                    StandardDialog tempScript = varObj.GetComponent<StandardDialog>();
                    if (tempScript == null)
                    {
                        tempScript = varObj.AddComponent<StandardDialog>();
                    }
                    tempScript.SetContents(varContents);
                    tempScript.SetCloseCallback(varCloseCallback);
                }
            };
#if ASSETBUNDLE_MODE
            OpenDialog("StandardDialog", tempCallback);
#else
            OpenDialog("UI/Dialog/StandardDialog", tempCallback);
#endif
        }

        /// <summary>
        /// 打开网络提示弹框.
        /// </summary>
        public void OpenNetDialog()
        {
            if (!mDialogs.ContainsKey(NetDialog.mDialogName))
            {
                OpenDialog(NetDialog.mDialogName, null);
            }
        }

        /// <summary>
        ///打开一个二次确认公用弹框.
        /// </summary>
        public void OpenConfirmDialog(string varContents, DialogActionDelegate varCallBack, bool varIsShowCancle = true)
        {
            //默认按钮名字设置为 确 定;
            string tempBtnName = Localization.Get("useful sure");
            OpenConfirmDialog(varContents, tempBtnName, varCallBack, null, varIsShowCancle);
        }
        /// <summary>
        ///打开一个二次确认公用弹框.
        /// </summary>
        public void OpenConfirmDialog(string varContents, DialogActionDelegate varCallBack, object varParam)
        {
            //默认按钮名字设置为 确 定;
            string tempBtnName = Localization.Get("useful sure");
            OpenConfirmDialog(varContents, tempBtnName, varCallBack, varParam);
        }
        /// <summary>
        ///打开一个二次确认公用弹框.
        /// </summary>
        public void OpenConfirmDialog(string varContents, string varBtnName, DialogActionDelegate varCallBack, object varParam, bool varIsShowCancle = true)
        {
            //默认标题名字设置为 提 示;
            string tempTitleName = Localization.Get("PublicDialog_1");
            string tempBtnNameCancel = Localization.Get("useful cancle");
            OpenConfirmDialog(tempTitleName, varContents, varBtnName, tempBtnNameCancel, varCallBack, varParam, varIsShowCancle);
        }
        /// <summary>
        ///打开一个二次确认公用弹框.
        /// </summary>
        public void OpenConfirmDialog(string varTitle, string varContents, string varBtnOKName, string varBtnNameCancel, DialogActionDelegate varCallBack, object varParam, bool varIsShowCancle)
        {

            OpenWindowCallBack tempCallback = delegate(GameObject varObj, ResourceLoadParam varParam2)
            {
                if (this == null)
                {
                    return;
                }

                if (varObj != null)
                {
                    ConfirmDialog tempScript = UIHelper.GetComponent<ConfirmDialog>(varObj.transform, "Animation/ConfirmDialog");
                    if (tempScript == null)
                    {
                        tempScript = varObj.AddComponent<ConfirmDialog>();
                    }
                    tempScript.ShowDiaogInfo(varTitle, varContents, varBtnOKName, varBtnNameCancel, varCallBack, varParam, varIsShowCancle);
                }
            };

            OpenDialog(ConfirmDialog.mDialogName, tempCallback);
        }

        public void OpenDialog(string varDialogName, OpenWindowCallBack varCallback)
        {
            OpenDialog(varDialogName, false, true, varCallback);
        }

        private GameObject OpenDialogCallback(string varDialogName, bool varStopBackEvent, GameObject varLoadObject)
        {
            GameObject tempLoadObject = GameObject.Instantiate(varLoadObject) as GameObject;
            if (tempLoadObject.transform.childCount == 0)
            {
                GameObject.DestroyObject(tempLoadObject);
                return null;
            }

            Transform tempRoot = tempLoadObject.transform.GetChild(0);
            for (int i = 0; i < tempLoadObject.transform.childCount; i++)
            {
                tempRoot = tempLoadObject.transform.GetChild(i);
                Camera tempCamera = tempRoot.GetComponent<Camera>();
                if (tempCamera != null)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            if (tempRoot == null)
            {
                Helper.LogError("load Dialog can't find the RootObject:" + varDialogName);
                GameObject.DestroyObject(tempLoadObject);
                return null;
            }

            GameObject tempDialog = tempRoot.gameObject;
            tempRoot.SetParent(mDialogRoot.transform);
            tempRoot.localScale = Vector3.one;
            if (mDialogs.ContainsKey(varDialogName))
            {
                CloseDialog(varDialogName);
            }
            mDialogs.Add(varDialogName, tempDialog);
            tempDialog.SetActive(true);
            if (varStopBackEvent)
            {
                SetUICameraEventNotReceiverMask(mUILayer, mCustomUILayer);
            }
            mDialogMaxDepth = OffsetDepth(tempDialog, mDialogMaxDepth);
            NGUITools.SetLayer(tempDialog, mDialogLayer);
            GameObject.DestroyObject(tempLoadObject);

            return tempDialog;
        }

        /// <summary>
        ///打开一个新的Dialog窗口.
        /// </summary>
        public void OpenDialog(string viewName, bool closeOthers, bool stopBackEvent, OpenWindowCallBack openCallback)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return;
            }
            if (closeOthers)
            {
                //关闭所有Dialog;
                CloseAllDialogs();
            }
            GameObject tempLoadObject = GetDialogCacheByName(viewName);
            if (tempLoadObject == null)
            {
                LoadAssetCallback tempCallback = delegate(UnityEngine.Object obj, ResourceLoadParam varParam)
                {
                    GameObject tempObj = obj as GameObject;
                    if (tempObj == null)
                    {
                        Helper.LogError("open Dialog Failed, Resources.Load Fail,file name: " + viewName);
                        if (openCallback != null)
                        {
                            openCallback(null, null);
                        }
                        return;
                    }

                    if (!mAllLoadDialogs.ContainsKey(viewName))
                    {
                        mAllLoadDialogs.Add(viewName, tempObj);
                    }

                    tempLoadObject = OpenDialogCallback(viewName, stopBackEvent, tempObj);
                    if (openCallback != null)
                    {
                        openCallback(tempLoadObject, null);
                    }
                };

                //string tempDialogPath = string.Format("ui/{0}", viewName);

                //int tempIndex = viewName.LastIndexOf("/");
                //if (tempIndex >= 0)
                //{
                //    //ResourceManager.GetSingleton().LoadUIAssetBundle(tempDialogPath, varDialogName.Substring(tempIndex + 1), typeof(GameObject), tempCallback);
                //    //ResourceManager.LoadAssetAsync(tempDialogPath, varDialogName.Substring(tempIndex + 1), typeof(GameObject), tempCallback);
                //}
                //else
                //{
                //    //ResourceManager.GetSingleton().LoadUIAssetBundle(tempDialogPath, varDialogName, typeof(GameObject), tempCallback);
                //    //ResourceManager.LoadAssetAsync(tempDialogPath, varDialogName, typeof(GameObject), tempCallback);
                //}
                if (ResourceManager.IsResourceMode)
                {
                    ViewResAdapter.Instance.LoadWindowWithResource(viewName, tempCallback);
                }
                else
                {
                    ViewResAdapter.Instance.LoadWindowWithAssetBundle("ui/common/dialog", viewName, tempCallback);
                }
            }
            else
            {
                tempLoadObject = OpenDialogCallback(viewName, stopBackEvent, tempLoadObject);

                if (openCallback != null)
                {
                    openCallback(tempLoadObject, null);
                }
            }
        }
        /// <summary>
        /// 打开一个提示弹窗
        /// </summary>
        /// <param name="tipTitle"></param>
        /// <param name="context"></param>
        /// <param name="call"></param>
        public void OpenTipDialog(string tipTitle, string context, DialogTip.BtnCallback call)
        {
            OpenWindowCallBack windowCall = delegate(GameObject view, ResourceLoadParam param)
{
    if (view != null)
    {
        DialogTip temp = view.GetComponent<DialogTip>();
        if (temp != null)
        {
            temp.mCall = call;
            temp.context = context;
            temp.tipTitle = tipTitle;
        }
    }
};
            OpenDialog(DialogTip.Path, windowCall);

        }

        public GameObject OpenDialogFromResources(string varDialogName, bool varCloseOthers, bool varStopBackEvent)
        {
            if (string.IsNullOrEmpty(varDialogName))
            {
                return null;
            }
            if (varCloseOthers)
            {
                //关闭所有Dialog;
                CloseAllDialogs();
            }

            GameObject tempLoadObject = GetDialogCacheByName(varDialogName);
            if (tempLoadObject == null)
            {
                GameObject tempObj = Resources.Load<GameObject>(varDialogName);
                if (tempObj == null)
                {
                    Helper.LogError("open Dialog Failed, Resources.Load Fail,file name: " + varDialogName);
                    return null;
                }

                if (!mAllLoadDialogs.ContainsKey(varDialogName))
                {
                    mAllLoadDialogs.Add(varDialogName, tempObj);
                }

                tempLoadObject = OpenDialogCallback(varDialogName, varStopBackEvent, tempObj);
            }
            else
            {
                tempLoadObject = OpenDialogCallback(varDialogName, varStopBackEvent, tempLoadObject);
            }
            return tempLoadObject;
        }

        /// <summary>
        /// 设置Camera需要渲染的层.
        /// </summary>
        /// <param name="varlayers">需要渲染的层数组.</param>
        public void SetCameraCullingMaskLayer(params int[] varlayers)
        {
            if (mCamera != null)
            {
                mCamera.cullingMask = Helper.OnlyIncluding(varlayers);
            }
        }
        /// <summary>
        /// 设置UICamera不接收事件的层.
        /// </summary>
        /// <param name="varlayers">不接收事件的层数组.</param>
        public void SetUICameraEventNotReceiverMask(params int[] varlayers)
        {
            if (mUICamera != null)
            {
                mUICamera.eventReceiverMask = Helper.EverythingBut(varlayers);
            }
        }
        /// <summary>
        /// 设置UICamera接收事件的层.
        /// </summary>
        /// <param name="varlayers">不接收事件的层数组.</param>
        public void SetUICameraEventReceiverMask(params int[] varlayers)
        {
            if (mUICamera != null)
            {
                mUICamera.eventReceiverMask += Helper.OnlyIncluding(varlayers);
            }
        }
        /// <summary>
        /// 通过窗口名得到一个窗口GameObject.
        /// </summary>
        /// <returns>The window by name.</returns>
        /// <param name="varWindowName">Variable window name.</param>
        public GameObject GetWindowByName(string varWindowName)
        {
            if (string.IsNullOrEmpty(varWindowName))
            {
                return null;
            }
            GameObject obj = null;
            if (mWindows.TryGetValue(varWindowName, out obj))
            {
                return obj;
            }

            return null;
        }
        /// <summary>
        /// 通过Dialog名得到一个DialogGameObject.
        /// </summary>
        /// <returns>The Dialog by name.</returns>
        /// <param name="varDialogName">Variable Dialog name.</param>
        public GameObject GetDialogByName(string varDialogName)
        {
            if (string.IsNullOrEmpty(varDialogName))
            {
                return null;
            }
            GameObject tempobj = null;
            if (mDialogs.TryGetValue(varDialogName, out tempobj))
            {
                return tempobj;
            }

            return null;
        }
        /// <summary>
        /// 通过Dialog名得到一个已经加载过的Dialog缓存对象.
        /// </summary>
        /// <returns>The Dialog by name.</returns>
        /// <param name="varDialogName">Variable Dialog name.</param>
        public GameObject GetDialogCacheByName(string varDialogName)
        {
            if (string.IsNullOrEmpty(varDialogName) || mAllLoadDialogs == null)
            {
                return null;
            }
            GameObject tempobj = null;
            if (mAllLoadDialogs.TryGetValue(varDialogName, out tempobj))
            {
                return tempobj;
            }

            return null;
        }
        /// <summary>
        /// 销毁一个窗口.
        /// </summary>
        /// <returns><c>true</c>, if widow by name was destoryed, <c>false</c> otherwise.</returns>
        /// <param name="varWindowName">Variable window name.</param>
        public bool DestoryWidowByName(string varWindowName)
        {
            if (string.IsNullOrEmpty(varWindowName))
            {
                return false;
            }
            if (mWindows.ContainsKey(varWindowName))
            {
                GameObject window = GetWindowByName(varWindowName);
                GameObject.Destroy(window);
                RemoveWindowPanels(varWindowName);
                mWindows.Remove(varWindowName);
                winNameToFirstDepth.Remove(varWindowName);

                // 把剩余窗口中深度最大的层级重置为QiumoUI;
                int tempMaxDepth;
                GameObject tempWin = GetOpenWinMaxDepth(out tempMaxDepth);
                if (tempMaxDepth > 0)
                {
                    mMaxDepth = tempMaxDepth;
                    mMaxDepth += 1;
                }

                if (tempWin != null)
                {
                    NGUITools.SetLayer(tempWin, mCustomUILayer);
                }

                WindowChange();

                NetWorkEventManager.GetSingleton().NotifyNetWorkEvent(new NetWorkEvent(NetWorkEventType.NE_NotifyRedChangePoint));
            }
            else
            {
                Helper.Log(Helper.Format("can not found such window {0} to class", varWindowName));
            }
            return true;
        }

        /// <summary>
        /// 激死一个窗口.
        /// </summary>
        /// <returns><c>true</c>, if window was closed, <c>false</c> otherwise.</returns>
        /// <param name="varWindowName">Variable window name.</param>
        public bool CloseWindow(string varWindowName)
        {
            if (string.IsNullOrEmpty(varWindowName))
            {
                return false;
            }
            GameObject needCloseWindow = GetWindowByName(varWindowName);
            if (needCloseWindow == null)
            {
                return false;
            }
            // 将要隐藏的窗口深度还原;
            UIPanel[] tempPanels = GetWindowPanels(varWindowName);
            if (tempPanels == null)
            {
                tempPanels = needCloseWindow.GetComponentsInChildren<UIPanel>(true);
                RecordWindowPanels(varWindowName, tempPanels);
            }

            if (tempPanels != null)
            {
                if (tempPanels.Length > 0)
                {
                    int tempMinDepth = mMaxDepth;
                    for (int i = 0; i < tempPanels.Length; i++)
                    {
                        if (tempPanels[i].depth < tempMinDepth)
                        {
                            tempMinDepth = tempPanels[i].depth;
                        }
                    }
                    for (int i = 0; i < tempPanels.Length; i++)
                    {
                        tempPanels[i].depth = tempPanels[i].depth - tempMinDepth + 1;
                    }
                }
            }

            needCloseWindow.SetActive(false);

            // 把剩余窗口中深度最大的层级重置为QiumoUI;
            int tempMaxDepth;
            GameObject tempWin = GetOpenWinMaxDepth(out tempMaxDepth);
            if (tempMaxDepth > 0)
            {
                mMaxDepth = tempMaxDepth;
                mMaxDepth += 1;
            }

            if (tempWin != null)
            {
                NGUITools.SetLayer(tempWin, mCustomUILayer);
            }

            WindowChange();

            return true;
        }
        /// <summary>
        /// 关闭所有的Dialog窗口.
        /// 指引剧情等除外.
        /// </summary>
        public void CloseAllDialogs()
        {
            if (mDialogs == null)
            {
                return;
            }

            Dictionary<string, GameObject> tempDialogs = new Dictionary<string, GameObject>(mDialogs);

            Dictionary<string, GameObject>.Enumerator tempEnumerator = tempDialogs.GetEnumerator();

            for (int i = 0; i < tempDialogs.Count; i++)
            {
                tempEnumerator.MoveNext();
                KeyValuePair<string, GameObject> tempKvp = tempEnumerator.Current;
                if (tempKvp.Value != null)
                {
                    if (mIgnoreClearDialogs.Contains(tempKvp.Key))
                    {
                        continue;
                    }
                    GameObject.Destroy(tempKvp.Value);

                    mDialogs.Remove(tempKvp.Key);
                }
            }
            if (mDialogs.Count <= 0)
            {
                SetUICameraEventNotReceiverMask(mUILayer, mDialogLayer);
                mDialogMaxDepth = mDialogMinDepth;
            }

        }
        /// <summary>
        /// 清除所有的Dialog窗口.
        /// </summary>
        public void ClearAllDialogs()
        {
            if (mDialogs == null)
            {
                SetUICameraEventNotReceiverMask(mUILayer, mDialogLayer);
                return;
            }
            Dictionary<string, GameObject>.Enumerator tempEnumerator = mDialogs.GetEnumerator();

            for (int i = 0; i < mDialogs.Count; i++)
            {
                tempEnumerator.MoveNext();
                KeyValuePair<string, GameObject> tempKvp = tempEnumerator.Current;
                if (tempKvp.Value != null)
                {
                    GameObject.Destroy(tempKvp.Value);
                }
            }
            mDialogs.Clear();
            SetUICameraEventNotReceiverMask(mUILayer, mDialogLayer);
            mDialogMaxDepth = mDialogMinDepth;
        }
        /// <summary>
        /// 清空以加载过的Dialog的缓存.
        /// </summary>
        public void ClearAllLoadDialogsCache()
        {
            if (mAllLoadDialogs == null)
            {
                return;
            }
            Dictionary<string, GameObject>.Enumerator tempEnumerator = mAllLoadDialogs.GetEnumerator();
            for (int i = 0; i < mAllLoadDialogs.Count; i++)
            {
                tempEnumerator.MoveNext();
                KeyValuePair<string, GameObject> tempKvp = tempEnumerator.Current;
                if (tempKvp.Value != null)
                {
                    GameObject.Destroy(tempKvp.Value);
                }
            }
            mAllLoadDialogs.Clear();
        }
        /// <summary>
        /// 关闭一个Dialog弹框.
        /// </summary>
        /// <returns><c>true</c>, if dialog was closed, <c>false</c> otherwise.</returns>
        /// <param name="varDialogName">Variable dialog name.</param>
        public bool CloseDialog(string varDialogName)
        {
            if (string.IsNullOrEmpty(varDialogName))
            {
                return false;
            }
            if (mDialogs == null)
            {
                return false;
            }
            GameObject tempDialog = null;
            if (mDialogs.TryGetValue(varDialogName, out tempDialog))
            {
                mDialogs.Remove(varDialogName);
                GameObject.Destroy(tempDialog);
            }
            if (mDialogs.Count == 0)
            {
                mDialogMaxDepth = mDialogMinDepth;
                //重置UICamera的接收事件层为仅仅不接收UI层事件;
                SetUICameraEventNotReceiverMask(mUILayer, mDialogLayer);
                return false;
            }
            return true;
        }

        public void ClearLoadDialogsCache(string varDialogName)
        {
            if (mAllLoadDialogs == null || mAllLoadDialogs.Count == 0)
            {
                return;
            }

            GameObject tempObj = null;
            if (mAllLoadDialogs.TryGetValue(varDialogName, out tempObj))
            {
                mAllLoadDialogs.Remove(varDialogName);
                //				GameObject.Destroy(tempObj);
                GameObject.DestroyImmediate(tempObj, true);
            }
        }

        // 其他窗口层级置为UI;
        private void SetOtherWinToUI(GameObject varObj)
        {
            if (mWindows.Count > 0)
            {
                Dictionary<string, GameObject>.Enumerator tempHadOpenWin = mWindows.GetEnumerator();
                for (int i = 0; i < mWindows.Count; i++)
                {
                    tempHadOpenWin.MoveNext();
                    KeyValuePair<string, GameObject> tempKvp = tempHadOpenWin.Current;
                    GameObject tempOtherWin = tempKvp.Value;
                    if (tempOtherWin == null)
                    {
                        continue;
                    }

                    if (tempOtherWin != varObj)
                    {
                        NGUITools.SetLayer(tempKvp.Value, mUILayer);
                    }
                }

                NGUITools.SetLayer(varObj, mCustomUILayer);
            }
        }
        /// <summary>
        /// 将窗口移到最前面.
        /// </summary>
        /// <returns><c>true</c>, if window to front was moved, <c>false</c> otherwise.</returns>
        /// <param name="varWindowName">Variable window name.</param>
        public bool MoveWindowToFront(string varWindowName)
        {
            GameObject go = GetWindowByName(varWindowName);
            return MoveWindowToFront(go, varWindowName);
        }

        private bool MoveWindowToFront(GameObject varObj, string varWindowName)
        {
            if (varObj == null)
            {
                return false;
            }

            varObj.SetActive(true);

            if (string.IsNullOrEmpty(varWindowName))
            {
                return false;
            }

            if (mWindows.Count <= 1)
            {
                return false;
            }

            if (winNameToFirstDepth.ContainsKey(varWindowName))
            {
                //				if (winNameToFirstDepth [varWindowName] < mMaxDepth) 
                //				{
                //
                //				}

                int tempOffsetDepth = mMaxDepth; // - winNameToFirstDepth[varWindowName];

                UIPanel[] ps = GetWindowPanels(varWindowName);
                if (ps == null)
                {
                    ps = varObj.GetComponentsInChildren<UIPanel>(true);
                    RecordWindowPanels(varWindowName, ps);
                }

                int tempMaxDepth = 0;
                for (int i = 0; i < ps.Length; i++)
                {
                    UIPanel p = ps[i];
                    p.depth += tempOffsetDepth;
                    if (p.depth > tempMaxDepth)
                    {
                        tempMaxDepth = p.depth;
                    }
                }
                if (tempMaxDepth > mMaxDepth)
                {
                    mMaxDepth = tempMaxDepth;
                }
                winNameToFirstDepth[varWindowName] = mMaxDepth;
                mMaxDepth += 1;
            }

            SetOtherWinToUI(varObj);

            return true;
        }

        /// <summary>
        /// 将窗口移到最后面.
        /// </summary>
        /// <returns><c>true</c>, if window to back was moved, <c>false</c> otherwise.</returns>
        /// <param name="varWindowName">Variable window name.</param>
        public bool MoveWindowToBack(string varWindowName)
        {
            if (string.IsNullOrEmpty(varWindowName))
            {
                return false;
            }

            if (mWindows.Count <= 1)
            {
                return false;
            }

            GameObject tempWin = GetWindowByName(varWindowName);
            if (tempWin == null)
            {
                return false;
            }

            if (winNameToFirstDepth.ContainsKey(varWindowName) == false)
            {
                return false;
            }

            int tempCurWinDepth = winNameToFirstDepth[varWindowName];
            int tempCurWinOffsetDepth = 0;

            List<string> tempMinWins = new List<string>();
            List<int> tempOffsetDepths = new List<int>();

            // 找出比当前窗口深度小的窗口, 计算它们往前移动的偏移量;
            Dictionary<string, int>.Enumerator tempDic = winNameToFirstDepth.GetEnumerator();
            for (int i = 0; i < winNameToFirstDepth.Count; i++)
            {
                tempDic.MoveNext();
                KeyValuePair<string, int> tempKvp = tempDic.Current;
                if (tempKvp.Value < tempCurWinDepth)
                {
                    int tempOffsetDepth = tempCurWinDepth - tempKvp.Value;
                    tempMinWins.Add(tempKvp.Key);
                    tempOffsetDepths.Add(tempOffsetDepth);

                    if (tempCurWinOffsetDepth < tempOffsetDepth)
                    {
                        tempCurWinOffsetDepth = tempOffsetDepth;
                    }
                }
            }

            // 比当前窗口深度小的窗口都往前移;
            for (int i = 0; i < tempMinWins.Count; i++)
            {
                GameObject tempObj = null;
                if (mWindows.TryGetValue(tempMinWins[i], out tempObj))
                {
                    if (tempObj == null || tempObj.activeSelf == false)
                    {
                        continue;
                    }

                    int tempOffsetDepth = tempOffsetDepths[i];
                    UIPanel[] tempWinPanels = tempObj.GetComponentsInChildren<UIPanel>(true);
                    for (int j = 0; j < tempWinPanels.Length; j++)
                    {
                        tempWinPanels[j].depth += tempOffsetDepth;
                    }
                }
            }

            // 当前窗口往后移动指定偏移量;
            UIPanel[] tempPanels = GetWindowPanels(varWindowName);
            if (tempPanels == null)
            {
                tempPanels = tempWin.GetComponentsInChildren<UIPanel>(true);
                RecordWindowPanels(varWindowName, tempPanels);
            }

            for (int i = 0; i < tempPanels.Length; i++)
            {
                tempPanels[i].depth -= tempCurWinOffsetDepth;
            }

            return true;
        }

        /// <summary>
        //打开一个新窗口时，设置新窗口的Detph
        /// </summary>
        private void SetWindowDepth(GameObject go, string varWindowName)
        {
            mMaxDepth = OffsetDepth(go, varWindowName, mMaxDepth);
            if (winNameToFirstDepth.ContainsKey(varWindowName))
            {
                winNameToFirstDepth[varWindowName] = mMaxDepth;
            }
            else
            {
                winNameToFirstDepth.Add(varWindowName, mMaxDepth);
            }
            mMaxDepth += 1;
        }

        /// <summary>
        //获得窗口所有子Uipanel并且改变其Depth最大
        /// </summary>
        private int OffsetDepth(GameObject gameObject, int depth)
        {
            UIPanel[] ps = gameObject.GetComponentsInChildren<UIPanel>(true);
            int maxDepth = 0;
            for (int i = 0; i < ps.Length; i++)
            {
                UIPanel p = ps[i];
                p.depth += depth;
                if (p.depth > maxDepth)
                {
                    maxDepth = p.depth;
                }
            }
            return maxDepth;
        }

        private int OffsetDepth(GameObject gameObject, string varWindowName, int depth)
        {
            UIPanel[] ps = GetWindowPanels(varWindowName);
            if (ps == null)
            {
                ps = gameObject.GetComponentsInChildren<UIPanel>(true);
                RecordWindowPanels(varWindowName, ps);
            }

            int maxDepth = 0;
            for (int i = 0; i < ps.Length; i++)
            {
                UIPanel p = ps[i];
                p.depth += depth;
                if (p.depth > maxDepth)
                {
                    maxDepth = p.depth;
                }
            }
            return maxDepth;
        }

        /// <summary>
        /// 从所有激活的窗口中查找最大深度的窗口.
        /// </summary>
        /// <returns>The open window max depth.</returns>
        private GameObject GetOpenWinMaxDepth(out int varMaxDepth)
        {
            varMaxDepth = 0;

            Dictionary<string, int>.Enumerator tempDic = winNameToFirstDepth.GetEnumerator();
            int tempMaxDepth = -1;
            GameObject tempWin = null;

            for (int i = 0; i < winNameToFirstDepth.Count; i++)
            {
                tempDic.MoveNext();
                KeyValuePair<string, int> tempKvp = tempDic.Current;
                GameObject tempObj = null;
                mWindows.TryGetValue(tempKvp.Key, out tempObj);
                //				if (tempKvp.Value > varMaxDepth)
                //				{
                //					varMaxDepth = tempKvp.Value;
                //				}

                if (tempObj != null && tempObj.activeSelf)
                {
                    if (tempKvp.Value > tempMaxDepth)
                    {
                        tempWin = tempObj;
                        tempMaxDepth = tempKvp.Value;
                    }
                }
            }

            if (tempMaxDepth > varMaxDepth)
            {
                varMaxDepth = tempMaxDepth;
            }
            return tempWin;
        }

        /// <summary>
        /// 关闭所有窗口，不包括指定名字的窗口.
        /// </summary>
        /// <param name="varWinArray">Variable window array.</param>
        public void DestroyAllWinBut(params string[] varWinArray)
        {
            if (varWinArray == null || varWinArray.Length == 0)
            {
                return;
            }

            List<string> tempRemove = null;

            int tempMaxDepth = 0;

            Dictionary<string, GameObject>.Enumerator tempHadOpenWin = mWindows.GetEnumerator();
            for (int i = 0; i < mWindows.Count; i++)
            {
                tempHadOpenWin.MoveNext();
                KeyValuePair<string, GameObject> tempKvp = tempHadOpenWin.Current;
                string tempKey = tempKvp.Key;

                int j = 0;
                while (j < varWinArray.Length)
                {
                    if (varWinArray[j] == tempKey)
                    {
                        int tempDepth = winNameToFirstDepth[tempKey];
                        if (tempMaxDepth < tempDepth)
                        {
                            tempMaxDepth = tempDepth;
                        }

                        NGUITools.SetLayer(tempKvp.Value, mCustomUILayer);

                        break;
                    }

                    j++;
                }
                if (j >= varWinArray.Length)
                {
                    if (tempRemove == null)
                    {
                        tempRemove = new List<string>();
                    }
                    tempRemove.Add(tempKey);
                }
            }

            if (tempRemove == null)
            {
                return;
            }
            for (int i = 0; i < tempRemove.Count; i++)
            {
                GameObject tempGo = null;
                string tempKey = tempRemove[i];
                if (mWindows.TryGetValue(tempKey, out tempGo))
                {
                    RemoveWindowPanels(tempKey);
                    mWindows.Remove(tempKey);
                    GameObject.Destroy(tempGo);
                }
                winNameToFirstDepth.Remove(tempKey);
            }

            if (tempMaxDepth > 0)
            {
                mMaxDepth = tempMaxDepth + 1;
            }

            WindowChange();
        }

        /// <summary>
        /// 关闭所有窗口除了聊天窗口.
        /// </summary>
        public void DestroyAllWin()
        {
            DestroyAllWin(false);
        }

        public void DestroyAllWin(bool varDestroyAll)
        {
            if (varDestroyAll)
            {
                if (mWindows.Count > 0)
                {
                    Dictionary<string, GameObject>.Enumerator tempHadOpenWin = mWindows.GetEnumerator();
                    for (int i = 0; i < mWindows.Count; i++)
                    {
                        tempHadOpenWin.MoveNext();
                        KeyValuePair<string, GameObject> tempKvp = tempHadOpenWin.Current;
                        if (tempKvp.Value != null)
                        {
                            GameObject.Destroy(tempKvp.Value);
                        }
                    }

                    if (mWinPanels != null)
                    {
                        mWinPanels.Clear();
                    }
                    mWindows.Clear();
                }

                if (winNameToFirstDepth.Count > 0)
                {
                    winNameToFirstDepth.Clear();
                }

                mMaxDepth = INIT_DEPTH;

                return;
            }
        }

        /// <summary>
        /// 获取激活的窗口.
        /// </summary>
        /// <returns>The active window.</returns>
        public int GetActiveWindow(out int varDepth)
        {
            varDepth = 0;
            if (mWindows.Count == 0)
            {
                return 0;
            }

            List<int> tempList = BlackMask.GetSingleton().pDepthList;
            if (tempList == null)
            {
                tempList = new List<int>();
            }
            else
            {
                tempList.Clear();
            }

            int tempActiveNum = 0;
            Dictionary<string, GameObject>.Enumerator tempHadOpenWin = mWindows.GetEnumerator();
            for (int i = 0; i < mWindows.Count; i++)
            {
                tempHadOpenWin.MoveNext();
                KeyValuePair<string, GameObject> tempKvp = tempHadOpenWin.Current;
                GameObject tempOtherWin = tempKvp.Value;
                if (tempOtherWin == null || !tempOtherWin.activeSelf)
                {
                    continue;
                }

                if (winNameToFirstDepth != null)
                {
                    int tempDepth = 0;
                    winNameToFirstDepth.TryGetValue(tempKvp.Key, out tempDepth);
                    tempList.Add(tempDepth);
                }

                ++tempActiveNum;
            }

            if (tempList.Count > 1)
            {
                tempList.Sort(SortByDepth);
                varDepth = tempList[1];
            }

            return tempActiveNum;
        }

        public int GetActiveWindow()
        {
            if (mWindows.Count == 0)
            {
                return 0;
            }

            int tempActiveNum = 0;
            Dictionary<string, GameObject>.Enumerator tempHadOpenWin = mWindows.GetEnumerator();
            for (int i = 0; i < mWindows.Count; i++)
            {
                tempHadOpenWin.MoveNext();
                KeyValuePair<string, GameObject> tempKvp = tempHadOpenWin.Current;
                GameObject tempOtherWin = tempKvp.Value;
                if (tempOtherWin == null || !tempOtherWin.activeSelf)
                {
                    continue;
                }

                ++tempActiveNum;
            }

            return tempActiveNum;
        }

        //public void RegisterUIChange(GlobalEventType varType, EventFuntion varFunc)
        //{
        //    if (mGlobalEventManager == null)
        //    {
        //        mGlobalEventManager = GlobalEventManager.GetSingleton();
        //    }
        //    if (mGlobalEventQueue == null)
        //    {
        //        mGlobalEventQueue = new GlobalEventQueue();
        //        mGlobalEventQueue.AddEvent(varType, varFunc);
        //    }
        //}

        //public void UnRegisterUIChange(WindowState varType, EventFuntion varFun)
        //{
        //    if (mGlobalEventManager == null)
        //    {
        //        return;
        //    }
        //    mSystemEvent.UnRegisterMsgHandler((int)varType, varFun);
        //}

        private void WindowChange()
        {
            int tempDepth = 0;
            int tempActiveWinNum = GetActiveWindow(out tempDepth);
            BlackMask tempMask = BlackMask.GetSingleton();
            tempMask.pActiveWinNum = tempActiveWinNum;
            tempMask.pMaxDepth = tempDepth;

            //if (mSystemEvent == null)
            //{
            //    return;
            //}
            //mSystemEvent.NotifyEvent((int)WindowState.WS_SetMainMask, tempMask);
        }

        //public void NotifyMaskChange(object varParam)
        //{
        //    if (mSystemEvent == null)
        //    {
        //        return;
        //    }
        //    mSystemEvent.NotifyEvent((int)WindowState.WS_SetMainMask, varParam);
        //}

        public int SortByDepth(int varDepth1, int varDepth2)
        {
            return -varDepth1.CompareTo(varDepth2);
        }

        /// 记录窗口panel的深度开始.
        private void RecordWindowPanels(string varWindowName, UIPanel[] varPanels)
        {
            if (mWinPanels == null)
            {
                mWinPanels = new Dictionary<string, UIPanel[]>();
                mWinPanels.Add(varWindowName, varPanels);

                return;
            }

            UIPanel[] tempPanels = null;
            bool tempB = mWinPanels.TryGetValue(varWindowName, out tempPanels);
            tempPanels = varPanels;
            if (!tempB)
            {
                mWinPanels.Add(varWindowName, tempPanels);
            }
        }

        private void RemoveWindowPanels(string varWindowName)
        {
            if (mWinPanels == null)
            {
                return;
            }

            if (mWinPanels.ContainsKey(varWindowName))
            {
                mWinPanels.Remove(varWindowName);
            }
        }

        private UIPanel[] GetWindowPanels(string varWindowName)
        {
            if (mWinPanels == null)
            {
                return null;
            }

            UIPanel[] tempPanels = null;
            mWinPanels.TryGetValue(varWindowName, out tempPanels);

            return tempPanels;
        }




        /// <summary>
        /// 返回NGUI坐标根据模型坐标.
        /// </summary>
        /// <param name="varTra"></param>
        /// <returns></returns>
        public Vector3 GetPos(Transform varTra)
        {
            if (varTra == null)
            {
                return Vector3.zero;
            }
            return GetPos(varTra.position);
        }


        public Vector3 GetPos(Vector3 varVec3)
        {
            GameObject tempCameraObj = GameObject.Find("Main Camera");
            if (tempCameraObj == null)
            {
                Helper.LogError("WindowManager: GetPos() tempCameraObj is NULL");
                return Vector3.zero;
            }

            Camera tempCamera = tempCameraObj.GetComponent<Camera>();
            if (tempCamera == null)
            {
                Helper.LogError("WindowManager: GetPos() tempCamera is NULL");
                return Vector3.zero;
            }

            // 1. 使用透视摄像机把世界坐标转换到屏幕坐标
            Vector3 pos = tempCamera.WorldToScreenPoint(varVec3);

            pos.z = 0f;   //把z设置为0
            pos.x -= (Screen.width / 2.0f);
            pos.y -= (Screen.height / 2.0f);
            pos *= pScreenScale;

           
            return pos;
        }


        public Vector3 WorldToScreenPoint(Vector3 position, bool mutiplyScreenScale = false)
        {
            GameObject tempCameraObj = GameObject.Find("Main Camera");
            if (tempCameraObj == null)
            {
                Helper.LogError("WindowManager: GetPos() tempCameraObj is NULL");
                return Vector3.zero;
            }
            Camera tempCamera = tempCameraObj.GetComponent<Camera>();
            if (tempCamera == null)
            {
                Helper.LogError("WindowManager: GetPos() tempCamera is NULL");
                return Vector3.zero;
            }
            Vector3 pos = tempCamera.WorldToScreenPoint(position);
            pos.z = 0f;
            pos.x -= (Screen.width / 2.0f);
            pos.y -= (Screen.height / 2.0f);
            if (mutiplyScreenScale)
            {
                pos *= pScreenScale;
            }
            return pos;
        }

    }


}