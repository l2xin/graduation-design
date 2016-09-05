using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
#endif
using System.IO;

public static class XCodePostProcess
{
#if UNITY_EDITOR
    private const string key_Capability_InAppPurchase = "com.apple.InAppPurchase";
    private const string key_Capability_Keychain = "com.apple.Keychain";
    private const string key_Capability_GameCenter = "com.apple.GameCenter";

    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {

#if UNITY_5
        if (target != BuildTarget.iOS)
        {
#else
			if (target != BuildTarget.iPhone)
			{
#endif
            Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
            return;
        }

        Debug.Log("pathToBuiltProject:" + pathToBuiltProject);
        string path = Path.GetFullPath(pathToBuiltProject);
        Debug.Log("pathToBuiltProject full path:" + path);

        // Create a new project object from build target
        XCProject project = new XCProject(path);

        // Find and run through all projmods files to patch the project.Please pay attention that ALL projmods files in your project folder will be excuted!
        string[] files = null;
        if (isV5())
        {

            project.AddFrameworkSearchPaths("$(SRCROOT)/Frameworks/Plugins/iOS");

            files = Directory.GetFiles(Application.dataPath, "*.projmods", SearchOption.AllDirectories);

        }
        else
        {
            files = Directory.GetFiles(Application.dataPath, "*.projmods", SearchOption.AllDirectories);

        }


        foreach (string file in files)
        {

            UnityEngine.Debug.Log("ProjMod File: " + file);
            project.ApplyMod(file);

        }

        System.IO.File.Copy(Application.dataPath + "/../fxq.entitlements", pathToBuiltProject + "/Unity-iPhone/fxq.entitlements", true);

        project.project.SetSystemCapabilities(key_Capability_InAppPurchase, "1");
        project.project.SetSystemCapabilities(key_Capability_Keychain, "1");
        project.project.SetSystemCapabilities(key_Capability_GameCenter, "0");

        project.AddFile(Application.dataPath + "/../TSDKConfig", null, "SOURCE_ROOT", true, false);


        project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Distribution: Shenzhen Tencent Computer Systems Company Limited (9TV4ZYSS4J)", "Release");
        project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution: Shenzhen Tencent Computer Systems Company Limited (9TV4ZYSS4J)", "Release");
        project.overwriteBuildSetting("CODE_SIGN_IDENTITY", "iPhone Developer: txjsj Agent (87N52B8C25)", "Debug");
        project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Developer: txjsj Agent (87N52B8C25)", "Debug");
        project.overwriteBuildSetting("CODE_SIGN_ENTITLEMENTS", "Unity-iPhone/fxq.entitlements", "Debug");
        project.overwriteBuildSetting("CODE_SIGN_ENTITLEMENTS", "Unity-iPhone/fxq.entitlements", "Release");
        project.overwriteBuildSetting("ARCHS", "$(ARCHS_STANDARD)", "Debug");
        project.overwriteBuildSetting("ARCHS", "$(ARCHS_STANDARD)", "Release");

        project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Debug");
        project.overwriteBuildSetting("ENABLE_BITCODE", "NO", "Release");

        //TODO implement generic settings as a module option
        //project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "Release");

        EditorPlist(path);

        EditorCode(path);

        // Finally save the xcode project
        project.Save();

    }


    static bool isV5()
    {
        return Application.unityVersion[0] == '5';
    }

    private static void EditorPlist(string filePath)
    {
        XCPlist list = new XCPlist(filePath);

        string PlistAdd = @"
				<key>APPKEY_DENGTA</key>
				<string>0I200A6H7A04ZKTA</string>
				<key>CFBundleURLTypes</key>
				<array>
					<dict>
						<key>CFBundleTypeRole</key>
						<string>Editor</string>
						<key>CFBundleURLName</key>
						<string>weixin</string>
						<key>CFBundleURLSchemes</key>
						<array>
							<string>" + "WXAPP_ID" + @"</string>
						</array>
					</dict>
					<dict>
						<key>CFBundleTypeRole</key>
						<string>Editor</string>
						<key>CFBundleURLName</key>
						<string>tencentopenapi</string>
						<key>CFBundleURLSchemes</key>
						<array>
							<string>tencent" + "QQ_APPID" + @"</string>
						</array>
					</dict>
					<dict>
						<key>CFBundleTypeRole</key>
						<string>Editor</string>
						<key>CFBundleURLName</key>
						<string>QQ</string>
						<key>CFBundleURLSchemes</key>
						<array>
							<string>QQ" + "QQ_APPID_x8" + @"</string>
						</array>
					</dict>
					<dict>
						<key>CFBundleTypeRole</key>
						<string>Editor</string>
						<key>CFBundleURLName</key>
						<string>QQLaunch</string>
						<key>CFBundleURLSchemes</key>
						<array>
							<string>QQ" + "QQ_APPID" + @"</string>
						</array>
					</dict>
				</array>
				<key>CHANNEL_DENGTA</key>
				<string>10001</string>
				<key>LSApplicationCategoryType</key>
					<string></string>
					<key>LSApplicationQueriesSchemes</key>
					<array>
						<string>mqq</string>
						<string>mqqapi</string>
						<string>wtloginmqq2</string>
						<string>mqqopensdkapiV3</string>
						<string>mqqopensdkapiV2</string>
						<string>mqqwpa</string>
						<string>mqqOpensdkSSoLogin</string>
						<string>mqqgamebindinggroup</string>
						<string>mqqopensdkfriend</string>
						<string>mqzone</string>
						<string>weixin</string>
						<string>wechat</string>
					</array>
				<key>LSRequiresIPhoneOS</key>
				<true/>
				<key>MSDKKey</key>
				<string>" + "MSDKKey" + @"</string>
				<key>MSDK_OfferId</key>
				<string>" + "OfferID" + @"</string>
				<key>MSDK_URL</key>
				<string>" + "MsdkUrl" + @"</string>
				<key>QQAppID</key>
				<string>" + "QQ_APPID" + @"</string>
				<key>QQAppKey</key>
				<string>" + "QQ_KEY" + @"</string>
				<key>WXAppID</key>
				<string>" + "WX_APPID" + @"</string>
				<key>WXAppKey</key>
				<string>" + "WX_Key" + @"</string>
				<key>NeedNotice</key>
				<true/>
				<key>NoticeTime</key>
				<integer>600</integer>";

        list.AddKey(PlistAdd);

        list.Save();
    }

    private static void EditorCode(string filePath)
    {
        XClass UnityAppController = new XClass(filePath + "/Classes/UnityAppController.mm");

        UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"", "#include <apollo/ApolloApplication.h>");
        UnityAppController.WriteBelow("UnityCleanup();\n", "    [[ApolloApplication sharedInstance] applicationWillTerminate:application];");
        UnityAppController.WriteBelow("- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation\n{", "    [[ApolloApplication sharedInstance] handleOpenURL:url];");

        if (isV5() == true)
        {
            UnityAppController.WriteBelow("::printf(\"-> applicationWillEnterForeground()\\n\");", "    [[ApolloApplication sharedInstance] applicationWillEnterForeground:application];");
            UnityAppController.WriteBelow("::printf(\"-> applicationWillResignActive()\\n\");", "    [[ApolloApplication sharedInstance] applicationWillResignActive:application];");
            UnityAppController.WriteBelow("::printf(\"-> applicationDidEnterBackground()\\n\");", "    [[ApolloApplication sharedInstance] applicationDidEnterBackground:application];");
            UnityAppController.WriteBelow("::printf(\"-> applicationDidBecomeActive()\\n\");", "    [[ApolloApplication sharedInstance] applicationDidBecomeActive:application];");
            UnityAppController.WriteBelow("::printf(\"-> applicationWillTerminate()\\n\");", "    [[ApolloApplication sharedInstance] applicationWillTerminate:application];");
            UnityAppController.WriteBelow("\treturn YES;\n}", "- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url\r{\r    return [[ApolloApplication sharedInstance] handleOpenURL:url];\r} \n\n");
            UnityAppController.WriteBelow("[[NSNotificationCenter defaultCenter] postNotificationName:name object:UnityGetGLViewController()];\n",
                                           "\r[[ApolloApplication sharedInstance]setRootVC:UnityGetGLViewController()];");

        }
        else
        {
            UnityAppController.WriteBelow("printf_console(\"-> applicationWillEnterForeground()\\n\");", "    [[ApolloApplication sharedInstance] applicationWillEnterForeground:application];");
            UnityAppController.WriteBelow("printf_console(\"-> applicationWillResignActive()\\n\");", "    [[ApolloApplication sharedInstance] applicationWillResignActive:application];");
            UnityAppController.WriteBelow("UnityInitApplicationNoGraphics([[[NSBundle mainBundle] bundlePath]UTF8String]);\n", "    [[ApolloApplication sharedInstance] setRootVC:UnityGetGLViewController()];");
            UnityAppController.WriteBelow("[[ApolloApplication sharedInstance] applicationWillTerminate:application];\n\n}", "- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url\r{\r    return [[ApolloApplication sharedInstance] handleOpenURL:url];\r}");

        }
    }
#endif

    public static void Log(string message)
    {
        UnityEngine.Debug.Log("PostProcess: " + message);
    }
}
