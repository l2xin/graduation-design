/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: UITweenAction.cs
			// Describle: adapter for NGUI tween
			// Created By:  hsu
			// Date&Time:  2016/5/13 14:41:42
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Air2000.Animator
{
    [Serializable]
    public abstract class UITweenAction : ClipAction
    {
        public bool Reset = true;
#if UNITY_EDITOR
        public override void DisplayEditorView()
        {
            base.DisplayEditorView();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Reset", GUILayout.Width(100));
            Reset = EditorGUILayout.Toggle(Reset);
            GUILayout.EndHorizontal();
        }
#endif
    }
}
