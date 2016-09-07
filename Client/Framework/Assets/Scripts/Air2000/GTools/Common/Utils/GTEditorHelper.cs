/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTEditorHelper.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/28 14:38:48
            // Modify History:
            //
//----------------------------------------------------------------*/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Air2000
{
    public static class GTEditorHelper
    {
        public static Color SeparatorDefaultColor = new Color(0.35f, 0.35f, 0.35f, 1.0f);

        public static void DrawSeparator(float width, float height = 1)
        {
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                Texture2D tex = EditorGUIUtility.whiteTexture;
                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.color = SeparatorDefaultColor;
                GUI.DrawTexture(new Rect(2f, rect.yMax, width, height), tex);
                GUI.color = Color.white;
            }
        }

        public static bool DrawMinimalisticHeader(string text) { return DrawHeader(text, text, false, true); }
        public static bool DrawHeader(string text) { return DrawHeader(text, text, false, false); }
        public static bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, false); }
        public static bool DrawHeader(string text, bool detailded) { return DrawHeader(text, text, detailded, !detailded); }
        public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            bool state = EditorPrefs.GetBool(key, true);

            if (!minimalistic) GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;

            if (minimalistic)
            {
                if (state) text = "\u25BC" + (char)0x200a + text;
                else text = "\u25BA" + (char)0x200a + text;

                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
                if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                text = "<b><size=11>" + text + "</size></b>";
                if (state) text = "\u25BC " + text;
                else text = "\u25BA " + text;
                if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            }

            if (GUI.changed) EditorPrefs.SetBool(key, state);

            if (!minimalistic) GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }

        private static bool EndHorizontal = false;
        public static void BeginContents() { BeginContents(false); }
        public static void BeginContents(bool minimalistic)
        {
            if (!minimalistic)
            {
                EndHorizontal = true;
                GUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
            }
            else
            {
                EndHorizontal = false;
                EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
                GUILayout.Space(10f);
            }
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }
        public static void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            if (EndHorizontal)
            {
                GUILayout.Space(3f);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(3f);
        }

        public static T IsAssetLegal<T>(TextAsset varTextAsset)
        {
            if (varTextAsset == null) { return default(T); }
            T t = LocalCfg.GetSingleton().DeserializeObjFromTextAsset<T>(varTextAsset);
            if (t == null)
            {
                UnityEngine.Object.DestroyImmediate(varTextAsset);
                return default(T);
            }
            return t;
        }

        public static T IsAssetLegal<T>(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return default(T);
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath(filePath, typeof(TextAsset)) as TextAsset;
            if (textAsset == null) { return default(T); }
            UnityEngine.Object obj = UnityEngine.Object.Instantiate(textAsset);
            textAsset = obj as TextAsset;
            if (textAsset == null) { UnityEngine.Object.DestroyImmediate(obj); return default(T); }
            return IsAssetLegal<T>(textAsset);
        }

        public static T IsAssetLegal<T>(UnityEngine.Object obj)
        {
            if (obj == null) return default(T);
            TextAsset textAsset = UnityEngine.Object.Instantiate(obj) as TextAsset;
            if (textAsset == null) { UnityEngine.Object.DestroyImmediate(obj); return default(T); }
            return IsAssetLegal<T>(textAsset);
        }
    }
}
#endif