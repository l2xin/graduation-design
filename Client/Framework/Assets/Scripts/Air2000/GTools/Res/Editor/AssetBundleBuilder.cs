/*----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            // 
            // FileName: AssetBundleBuilder.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 18:39:34
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Air2000.Res
{
    public class AssetBundleBuilder
    {
        public static string overloadedDevelopmentServerURL = "";

        public static void BuildAssetBundles(bool deleteOld = false)
        {
            string outputPath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformName());
            if (Directory.Exists(outputPath))
            {
                if (deleteOld)
                {
                    Directory.Delete(outputPath, true);
                    Directory.CreateDirectory(outputPath);
                }
            }
            else
            {
                Directory.CreateDirectory(outputPath);
            }
            Caching.CleanCache();
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            AssetBundleBuilder.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundlesOutputPath));
        }

        public static void WriteServerURL()
        {
            string downloadURL;
            if (string.IsNullOrEmpty(overloadedDevelopmentServerURL) == false)
            {
                downloadURL = overloadedDevelopmentServerURL;
            }
            else
            {
                IPHostEntry host;
                string localIP = "";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
                downloadURL = "http://" + localIP + ":7888/";
            }

            string assetBundleManagerResourcesDirectory = "Assets/AssetBundleManager/Resources";
            string assetBundleUrlPath = Path.Combine(assetBundleManagerResourcesDirectory, "AssetBundleServerURL.bytes");
            Directory.CreateDirectory(assetBundleManagerResourcesDirectory);
            File.WriteAllText(assetBundleUrlPath, downloadURL);
            AssetDatabase.Refresh();
        }

        public static void BuildPlayer()
        {
            var outputPath = UnityEditor.EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0)
                return;
            string[] levels = GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                Debug.Log("Nothing to build.");
                return;
            }
            string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null)
                return;
            AssetBundleBuilder.BuildAssetBundles();
            WriteServerURL();
            BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
        }

        public static void BuildStandalonePlayer()
        {
            var outputPath = UnityEditor.EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0)
                return;

            string[] levels = GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                Debug.Log("Nothing to build.");
                return;
            }
            string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null)
                return;

            // Build and copy AssetBundles.
            AssetBundleBuilder.BuildAssetBundles();
            AssetBundleBuilder.BuildBundleScenes();
            AssetBundleBuilder.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundlesOutputPath));
            AssetDatabase.Refresh();

            BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            MoveResourcesToR();
            BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
            MoveRToResources();
        }

        public static void BuildBundleScenes()
        {
            if (ResContext.OptionsData == null) return;
            string[] levels = ResContext.OptionsData.BundleScenes.ToArray();
            if (levels.Length == 0)
            {
                Debug.Log("Nothing to build.");
                return;
            }
            for (int i = 0; i < levels.Length; i++)
            {
                string levelName = levels[i];
                if (string.IsNullOrEmpty(levelName)) continue;
                string[] array = new string[] { levelName };
                string outputPath = Utility.BundleSceneOutputPath;
                if (Directory.Exists(outputPath) == false)
                {
                    Directory.CreateDirectory(outputPath);
                }
                outputPath += Utility.GetFileNameWithoutExtFromFullPath(levelName);
                Debug.Log("Building BundleScenes: " + outputPath);
                BuildPipeline.BuildPlayer(array, outputPath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.BuildAdditionalStreamedScenes);
            }
            AssetBundleBuilder.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundlesOutputPath));
            AssetDatabase.Refresh();
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "/test.apk";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "/test.exe";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "/test.app";
                case BuildTarget.WebPlayer:
                case BuildTarget.WebPlayerStreamed:
                case BuildTarget.WebGL:
                    return "";
                // Add more build targets for your own.
                default:
                    Debug.Log("Target not implemented.");
                    return null;
            }
        }

        static void CopyAssetBundlesTo(string outputPath)
        {
            // Clear streaming assets folder.
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
            Directory.CreateDirectory(outputPath);

            string outputFolder = Utility.GetPlatformName();

            // Setup the source folder for assetbundles.
            var source = Path.Combine(Path.Combine(System.Environment.CurrentDirectory, Utility.AssetBundlesOutputPath), outputFolder);
            if (!System.IO.Directory.Exists(source))
                Debug.Log("No assetBundle output folder, try to build the assetBundles first.");

            // Setup the destination folder for assetbundles.
            var destination = System.IO.Path.Combine(outputPath, outputFolder);
            if (System.IO.Directory.Exists(destination))
                FileUtil.DeleteFileOrDirectory(destination);

            FileUtil.CopyFileOrDirectory(source, destination);
        }

        static string[] GetLevelsFromBuildSettings()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }

        public static void MoveResourcesToR()
        {
            string tempProjectPath = Application.dataPath;
            string tempDirectory = string.Format("{0}/R", tempProjectPath);
            if (Directory.Exists(tempDirectory) == false)
            {
                AssetDatabase.CreateFolder("Assets", "R");
            }
            tempDirectory = string.Format("{0}/Resources", tempProjectPath);
            try
            {
                string[] tempArray = Directory.GetDirectories(tempDirectory);
                for (int i = 0; i < tempArray.Length; i++)
                {
                    tempDirectory = tempArray[i];
                    tempDirectory = tempDirectory.Replace("\\", "/");
                    int tempIndex = tempDirectory.LastIndexOf("/");
                    if (tempIndex < 0)
                    {
                        continue;
                    }
                    string tempFolderName = tempDirectory.Substring(tempIndex + 1);
                    if (tempFolderName == "majorUI")
                    {
                        continue;
                    }
                    string tempOldPath = string.Format("Assets/Resources/{0}", tempFolderName);
                    string tempNewPath = string.Format("Assets/R/{0}", tempFolderName);
                    AssetDatabase.MoveAsset(tempOldPath, tempNewPath);
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.LogError("Resources Folder is not exist !");
                Debug.LogError(e.Message);
            }
        }

        public static void MoveRToResources()
        {
            string tempProjectPath = Application.dataPath;
            string tempDirectory = string.Format("{0}/Resources", tempProjectPath);
            if (Directory.Exists(tempDirectory) == false)
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            tempDirectory = string.Format("{0}/R", tempProjectPath);
            try
            {
                string[] tempArray = Directory.GetDirectories(tempDirectory);

                for (int i = 0; i < tempArray.Length; i++)
                {
                    tempDirectory = tempArray[i];
                    tempDirectory = tempDirectory.Replace("\\", "/");
                    int tempIndex = tempDirectory.LastIndexOf("/");
                    if (tempIndex < 0)
                    {
                        continue;
                    }
                    string tempFolderName = tempDirectory.Substring(tempIndex + 1);
                    string tempOldPath = string.Format("Assets/R/{0}", tempFolderName);
                    string tempNewPath = string.Format("Assets/Resources/{0}", tempFolderName);
                    AssetDatabase.MoveAsset(tempOldPath, tempNewPath);
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.LogError("R Folder is not exist !");
                Debug.LogError(e.Message);
            }
        }
    }
}
