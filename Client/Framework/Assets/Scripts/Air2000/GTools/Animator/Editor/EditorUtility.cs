/*----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            // 
            // FileName: EditorUtility.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/19 15:55:51
            // Modify History:
            //
//----------------------------------------------------------------*/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Air2000.Animator
{
    public class EditorUtility
    {
        public static bool DrawHeader(string text) { return DrawHeader(text, text, false, false); }
        public static bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, false); }
        public static bool DrawHeader(string text, bool detailded) { return DrawHeader(text, text, detailded, !detailded); }
        public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            try
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
            catch { return true; }
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
            try
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
            catch { }

        }
        public static List<Type> GetAllSubClass(Type varParentType, List<Type> types)
        {
            if (varParentType == null) return null;
            if (types == null)
            {
                types = new List<Type>();
            }
            Assembly ass = Assembly.GetAssembly(varParentType);
            foreach (Type child in ass.GetTypes())
            {
                if (child.BaseType == varParentType)
                {
                    if (child.IsAbstract == false)
                    {
                        types.Add(child);
                    }
                    GetAllSubClass(child, types);
                }
            }
            return types;
        }
        public static string GetTypeNameWithoutNamespcae(string varTypeName)
        {
            if (string.IsNullOrEmpty(varTypeName))
            {
                return string.Empty;
            }
            string[] pathArray = varTypeName.Split(new char[] { '.' });
            if (pathArray == null || pathArray.Length == 0)
            {
                return string.Empty;
            }
            return pathArray[pathArray.Length - 1];
        }
        public static string GenerateID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToString(buffer, 0);
        }
    }
}
#endif
