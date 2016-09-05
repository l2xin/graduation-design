/*----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            // 
            // FileName: BuildTool.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/16 17:45:56
            // Modify History:
            //
//----------------------------------------------------------------*/
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class BuildTool
{
    private static bool PROFILE_VERSION = false;
    private static string ANDROID_APK_PATH = "Bin/FXQ.apk";
   
    [MenuItem("Assets/Build Tool/Build Android (Standalone)")]
    public static void BuildAndroid()
    {
        SetAPKName();
        SetKeyStore();
        BuildOptions ops = SetBuildAPKOption();
        SetProductName("欢乐飞行棋");
        BuildPipeline.BuildPlayer(GetBuildScenes(), ANDROID_APK_PATH, BuildTarget.Android, ops);
        SetProductName("AirPlane");
    }
    public static void BuildIOS()
    {
        //设置编译选项
        string privateScriptSymbols = System.Environment.GetEnvironmentVariable("ScriptingDefineSymbols");
        Debug.Log("privateScriptSymbols is:" + privateScriptSymbols);
        if (null != privateScriptSymbols)
        {
            Debug.Log("[BuildScript]We get privateScriptSymbols from environment:" + privateScriptSymbols);
            privateScriptSymbols = privateScriptSymbols.Replace('|', ';');
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, privateScriptSymbols);

        //设置版本号
        string majorVersion = System.Environment.GetEnvironmentVariable("MajorVersion");
        string minorVersion = System.Environment.GetEnvironmentVariable("MinorVersion");
        string fixVersion = System.Environment.GetEnvironmentVariable("FixVersion");
        string versionNumStr = majorVersion + "." + minorVersion + "." + fixVersion;
        Debug.Log("Set RDM Version:" + versionNumStr);
        PlayerSettings.bundleVersion = versionNumStr;


        BuildOptions ops = BuildOptions.None;
        if (PROFILE_VERSION)
        {
            ops |= BuildOptions.Development;
            ops |= BuildOptions.AllowDebugging;
            ops |= BuildOptions.ConnectWithProfiler;
        }
        else
        {
            ops |= BuildOptions.None;
        }
        BuildPipeline.BuildPlayer(GetBuildScenes(), projectName, BuildTarget.iOS, ops);
    }
    private static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
            {
                continue;
            }
            if (e.enabled)
            {
                names.Add(e.path);
            }
        }
        return names.ToArray();
    }
    private static string GetCommandLineArg(string name)
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].ToLower() == name)
            {
                if (args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
        }
        return "";
    }
    private static void SetAPKName()
    {
        int apkIndex = 1;
        string datetime = DateTime.Now.ToString("yyyyMMdd");
        DirectoryInfo binDirectory = new DirectoryInfo(@"Bin\");
        FileInfo[] fileInfos = binDirectory.GetFiles();
        if (fileInfos != null && fileInfos.Length > 0)
        {
            for (int i = 0; i < fileInfos.Length; i++)
            {
                FileInfo fileInfo = fileInfos[i];
                if (fileInfo == null) continue;
                string fileName = fileInfo.Name;
                if (string.IsNullOrEmpty(fileName)) continue;

                string[] dotArray = fileName.Split(new char[] { '.' });
                if (dotArray == null || dotArray.Length == 0)
                {
                    continue;
                }
                if (dotArray.Length < 2) continue;
                string newName = dotArray[dotArray.Length - 2];
                if (string.IsNullOrEmpty(newName)) continue;
                string[] strArray = newName.Split(new char[] { '_' });
                if (strArray.Length == 2)
                {
                    string tempdate = strArray[0];
                    if (string.IsNullOrEmpty(tempdate)) continue;
                    if (tempdate.EndsWith(datetime))
                    {
                        int tempIndex = 0;
                        int.TryParse(strArray[1], out tempIndex);
                        if (tempIndex >= apkIndex)
                        {
                            apkIndex++;
                        }
                    }
                }
            }
        }
        if (apkIndex == 0) apkIndex = 1;
        ANDROID_APK_PATH = "Bin/FXQ" + datetime + "_" + apkIndex + ".apk";
    }
    private static void SetProductName(string name)
    {
        PlayerSettings.productName = name;
    }
    private static void SetKeyStore()
    {

        PlayerSettings.Android.keystoreName = "HLFXQ.keystore";
        PlayerSettings.Android.keystorePass = "qwer1234";
        PlayerSettings.Android.keyaliasName = "fxq";
        PlayerSettings.Android.keyaliasPass = "qwer1234";
    }
    private static BuildOptions SetBuildAPKOption()
    {
        PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
        string androidsdkPath = GetCommandLineArg("-androidsdkroot");
        if (!string.IsNullOrEmpty(androidsdkPath))
        {
            EditorPrefs.SetString("AndroidSdkRoot", androidsdkPath);
        }
        else
        {
            Debug.Log("-androidsdkroot has no value.");
        }

        BuildOptions ops = BuildOptions.None;
        if (PROFILE_VERSION)
        {
            ops |= BuildOptions.Development;
            ops |= BuildOptions.AllowDebugging;
            ops |= BuildOptions.ConnectWithProfiler;
        }
        else
        {
            ops |= BuildOptions.None;
        }
        return ops;
    }
    public static string projectName
    {
        get
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("project"))
                {
                    return arg.Split("-"[0])[1];
                }
            }
            return "test";
        }
    }
}
