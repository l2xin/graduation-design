/*----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            // 
            // FileName: ResourceManagerEditorWindow.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 16:44:53
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using UnityEditor;

namespace Air2000.Res
{
    public class ResourceManagerEditorWindow : EditorWindow
    {
        #region declarations
        public static string AssetBundleConfigName = ResContext.ResourceManagerConfigName + ".asset";
        public static string AssetBundleEditorResFolder = "Assets/AssetBundle/Editor/Res/";
        public static string AssetBundleResFolder = ResContext.ResourceManagerResFolder;

        public static ResourceManagerOptions mOptionsData = null;
        #endregion

        #region functions

        #region gui functions
        private void OnGUI()
        {
            //if (mOptionsData == null)
            //{
            //    mOptionsData = LoadFile();
            //}

            #region options
            //if (GTEditorHelper.DrawHeader("Options"))
            //{
            //    GTEditorHelper.BeginContents(false);
            //    GUILayout.BeginHorizontal(GUILayout.Width(300));
            //    EditorGUILayout.LabelField("Current Mode", GUILayout.Width(100));
            //    mOptionsData.AssetBundleMode = (AssetBundleMode)EditorGUILayout.EnumPopup(mOptionsData.AssetBundleMode, GUILayout.Width(180));
            //    GUILayout.EndHorizontal();

            //    GTEditorHelper.EndContents();

            //    //try
            //    //{
            //    //    EditorUtility.SetDirty(mOptionsData);
            //    //}
            //    //catch { }
            //}
            #endregion

            #region build operation
            GUILayout.Space(3.0f);

            if (EditorUtility.DrawHeader("Builder"))
            {
                EditorUtility.BeginContents(false);
                if (GUILayout.Button("Build Scenes", GUILayout.Width(295)))
                {
                    AssetBundleBuilder.BuildBundleScenes();
                }
                if (GUILayout.Button("Build AssetBundles", GUILayout.Width(295)))
                {
                    AssetBundleBuilder.BuildAssetBundles();
                }
                if (GUILayout.Button("Build Standalone Player", GUILayout.Width(295)))
                {
                    AssetBundleBuilder.BuildStandalonePlayer();
                }
                EditorUtility.EndContents();
            }
            #endregion


            #region ease mode
            GUILayout.Space(3.0f);
            if (EditorUtility.DrawHeader("Ease"))
            {
                EditorUtility.BeginContents(false);
                if (GUILayout.Button("Move Resources To R", GUILayout.Width(295)))
                {
                    AssetBundleBuilder.MoveResourcesToR();
                }
                if (GUILayout.Button("Move R TO Resources", GUILayout.Width(295)))
                {
                    AssetBundleBuilder.MoveRToResources();
                }
                EditorUtility.EndContents();
            }
            #endregion
        }
        #endregion

        #region utility
        public static ResourceManagerOptions LoadFile()
        {
            ResourceManagerOptions optionsData = (ResourceManagerOptions)Resources.Load(AssetBundleConfigName);
            if (optionsData)
            {
                return optionsData;
            }
            string filePath = AssetBundleResFolder + AssetBundleConfigName;
            optionsData = ScriptableObject.CreateInstance<ResourceManagerOptions>();
            AssetDatabase.CreateAsset(optionsData, filePath);
            AssetDatabase.Refresh();
            return optionsData;
        }
        #endregion

        #region menu item
        [MenuItem("GTools/ResourceManager", false)]
        public static void OpenWindow()
        {
            GetWindowWithRect<ResourceManagerEditorWindow>(new Rect(10, 10, 300, 250), false, "Resource");
        }
        #endregion
        #endregion
    }
}
