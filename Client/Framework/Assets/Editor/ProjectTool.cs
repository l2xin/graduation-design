using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ProjectTool
{
    private static bool PROFILE_VERSION = false;
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


    private static string ANDROID_PATH = "Bin/hlfxq.apk";
    public static void BuildForAndroid()
    {
        SetRDMVersion();
        SetProductName("");
        SetAPKName();
        SetKeyStore();
        BuildOptions ops = SetBuildAPKOption();
        BuildPipeline.BuildPlayer(GetBuildScenes(), ANDROID_PATH, BuildTarget.Android, ops);
        SetProductName("AirPlane");//revert product name
    }

    public static void BuildForAndroidForETC()
    {
        SetRDMVersion();
        SetProductName(" ");
        SetAPKName();
        SetKeyStore();
        BuildOptions ops = SetBuildAPKOption();
        BuildPipeline.BuildPlayer(GetBuildScenes(), ANDROID_PATH, BuildTarget.Android, ops);
        SetProductName("AirPlane");//revert product name
    }

    private static void SetRDMVersion()
    {
        string majorVersion = System.Environment.GetEnvironmentVariable("MajorVersion");
        string minorVersion = System.Environment.GetEnvironmentVariable("MinorVersion");
        string fixVersion = System.Environment.GetEnvironmentVariable("FixVersion");
        string versionNumStr = majorVersion + "." + minorVersion + "." + fixVersion;
        Debug.Log("Set RDM Version:" + versionNumStr);
        PlayerSettings.bundleVersion = versionNumStr;

        SetAndroidBundleVersionCode();
    }

    private static void SetAndroidBundleVersionCode()
    {
        string versionCodeStr = System.Environment.GetEnvironmentVariable("VersionCode");
        if (string.IsNullOrEmpty(versionCodeStr))
        {
            return;
        }

        int versionCode;
        if (Int32.TryParse(versionCodeStr, out versionCode))
        {
            PlayerSettings.Android.bundleVersionCode = versionCode;
        }
    }
    private static void SetProductName(string name)
    {
        // PlayerSettings.productName = name;
    }
    private static void SetAPKName()
    {
        string buildNo = System.Environment.GetEnvironmentVariable("BuildNo");
        string privateScriptSymbols = System.Environment.GetEnvironmentVariable("ScriptingDefineSymbols");
        Debug.Log("privateScriptSymbols is:" + privateScriptSymbols);
        if (null != privateScriptSymbols)
        {
            Debug.Log("[BuildScript]We get privateScriptSymbols from environment:" + privateScriptSymbols);
            privateScriptSymbols = privateScriptSymbols.Replace('|', ';');
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, privateScriptSymbols);
        PlayerSettings.bundleIdentifier = "com.tencent.tmgp.hlfxq";

        if (null != privateScriptSymbols && privateScriptSymbols.IndexOf("LIVE_BUILD") >= 0)
        {
            if (null != privateScriptSymbols && privateScriptSymbols.IndexOf("RELEASE_BUILD") >= 0)
            {
                // release
                Debug.Log("[BuildScript]We find by macros, it's release version.");
                ANDROID_PATH = "Bin/hlfxq_" + PlayerSettings.bundleVersion + "_" + PlayerSettings.Android.bundleVersionCode + "_Build" + buildNo + "_Live_Release.apk";
            }
            else
            {
                Debug.Log("[BuildScript]We find by macros, it's debug version.");
                ANDROID_PATH = "Bin/hlfxq_" + PlayerSettings.bundleVersion + "_" + PlayerSettings.Android.bundleVersionCode + "_Build" + buildNo + "_Live_Debug.apk";
            }
        }
        else
        {
            if (null != privateScriptSymbols && privateScriptSymbols.IndexOf("RELEASE_BUILD") >= 0)
            {
                // release
                Debug.Log("[BuildScript]We find by macros, it's release version.");
                ANDROID_PATH = "Bin/hlfxq_" + PlayerSettings.bundleVersion + "_" + PlayerSettings.Android.bundleVersionCode + "_Build" + buildNo + "_Internal_Release.apk";
            }
            else
            {
                Debug.Log("[BuildScript]We find by macros, it's debug version.");
                ANDROID_PATH = "Bin/hlfxq_" + PlayerSettings.bundleVersion + "_" + PlayerSettings.Android.bundleVersionCode + "_Build" + buildNo + "_Internal_Debug.apk";
            }
        }



        if (null != privateScriptSymbols && privateScriptSymbols.IndexOf("PROFILE_VERSION") >= 0)
        {
            PROFILE_VERSION = true;
        }
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
		PlayerSettings.bundleIdentifier = "com.tencent.tmgp.hlfxq";
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

    public static void BuildForIOS()
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
		PlayerSettings.bundleIdentifier = "com.tencent.hlfxq";


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
}