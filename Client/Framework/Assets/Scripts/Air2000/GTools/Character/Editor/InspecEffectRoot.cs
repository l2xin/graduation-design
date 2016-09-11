/*----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            // 
            // FileName: InspecEffectRoot.cs
			// Describle: 
			// Created By:  hsu
			// Date&Time:  2016/6/16 9:07:11
            // Modify History: 写代码一定要记得保存！！！！=-=
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Air2000.Character
{
    [CustomEditor(typeof(EffectRoot))]
    public class InspecEffectRoot : UnityEditor.Editor
    {
        public EffectRoot Instance;
        void OnEnable()
        {
            Instance = target as EffectRoot;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Instance == null) return;
            EditorUtility.BeginContents();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AssetBundle", GUILayout.Width(150f));
            Instance.AssetBundle = EditorGUILayout.TextField(Instance.AssetBundle);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1.0f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AssetName", GUILayout.Width(150f));
            Instance.AssetName = EditorGUILayout.TextField(Instance.AssetName);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1.0f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Effect (Read only)", GUILayout.Width(150f));
            EditorGUILayout.ObjectField("", Instance.Effect, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1.0f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("CacheImmediate", GUILayout.Width(150f));
            Instance.CacheImmediate = EditorGUILayout.Toggle(Instance.CacheImmediate);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1.0f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ActiveOnAwake", GUILayout.Width(150f));
            Instance.ActiveOnAwake = EditorGUILayout.Toggle(Instance.ActiveOnAwake);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1.0f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("DontFollowParent", GUILayout.Width(150f));
            Instance.DontFollowParent = EditorGUILayout.Toggle(Instance.DontFollowParent);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(1.0f);

            if (Instance.DontFollowParent)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Parent", GUILayout.Width(150f));
                Instance.Parent = (Transform)EditorGUILayout.ObjectField(Instance.Parent, typeof(Transform), true);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(1.0f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Position", GUILayout.Width(150f));
                Instance.Position = EditorGUILayout.Vector3Field("", Instance.Position);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(1.0f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("EulerAngles", GUILayout.Width(150f));
                Instance.EulerAngles = EditorGUILayout.Vector3Field("", Instance.EulerAngles);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(1.0f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("LocalScale", GUILayout.Width(150f));
                Instance.LocalScale = EditorGUILayout.Vector3Field("", Instance.LocalScale);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(1.0f);
            }

            EditorUtility.EndContents();
        }
    }
}
