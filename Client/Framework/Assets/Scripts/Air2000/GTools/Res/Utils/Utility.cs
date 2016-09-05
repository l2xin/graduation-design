/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Utility.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 14:53:48
            // Modify History:
            //
//----------------------------------------------------------------*/
//#define OPEN_RESOURCEMANAGER_LOG_ALL
//#define OPEN_RESOURCEMANAGER_LOG_DEBUG
//#define OPEN_RESOURCEMANAGER_LOG_WARNING
#define OPEN_RESOURCEMANAGER_LOG_ERROR

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GTools.Res
{
    public class Utility
    {
        public const string AssetBundlesOutputPath = "AssetBundles";
        public static string BundleSceneOutputPath = "AssetBundles/" + GetPlatformName() + "/" + "BundleScenes/";
        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            // return the activeBuildTarget
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssetBundles(Application.platform);
#endif
        }

#if UNITY_EDITOR
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.WebPlayer:
                    return "WebPlayer";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "OSX";
                default:
                    return null;
            }
        }
#endif

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    return "WebPlayer";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                default:
                    return null;
            }
        }

        public static string GetFileNameFromFullPath(string varPath)
        {
            if (string.IsNullOrEmpty(varPath))
            {
                return null;
            };
            string[] paths = varPath.Split(new char[] { '/' });
            if (paths.Length == 0)
            {
                return null;
            }
            return paths[paths.Length - 1];
        }

        public static string GetFileNameWithoutExtFromFullPath(string varPath)
        {
            string tempFileName = GetFileNameFromFullPath(varPath);
            if (string.IsNullOrEmpty(tempFileName)) return string.Empty;
            string[] paths = tempFileName.Split(new char[] { '.' });
            if (paths.Length == 0)
            {
                return null;
            }
            if (paths.Length == 1)
            {
                return paths[0];
            }
            return paths[paths.Length - 2];
        }

        public static void LogError(string msg)
        {
#if OPEN_RESOURCEMANAGER_LOG_ALL
            Debug.LogError("[ResourceManager] " + msg); return;
#elif OPEN_RESOURCEMANAGER_LOG_ERROR
            Debug.LogError("[ResourceManager] " + msg);
#endif
        }
        public static void LogDebug(string msg)
        {
#if OPEN_RESOURCEMANAGER_LOG_ALL
            Debug.Log("[ResourceManager] " + msg); return;
#elif OPEN_RESOURCEMANAGER_LOG_DEBUG
            Debug.Log("[ResourceManager] " + msg);
#endif
        }
        public static void LogWarning(string msg)
        {
#if OPEN_RESOURCEMANAGER_LOG_ALL
            Debug.LogWarning("[ResourceManager] " + msg); return;
#elif OPEN_RESOURCEMANAGER_LOG_WARNING
            Debug.LogWarning("[ResourceManager] " + msg);
#endif
        }
    }
}
