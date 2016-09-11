/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: PlayEffectAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/18 12:31:14
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Air2000.Animator
{
    [Serializable]
    public class PlayEffectAction : ClipAction
    {
        public GameObject EffectObj;
        public Transform Parent;
        public bool DontChangeTransform = true;
        public bool IgnoreScale = false;
        public Vector3 LocalScale;
        public Vector3 Position;
        public Vector3 EulerAngles;

        [HideInInspector]
        [NonSerialized]
        public Transform OriginalParent;

        public override string DisplayName
        {
            get
            {
                return "[PlayEffect] " + IdentifyName;
            }
        }

        public override void OnBegin(Clip clip)
        {
            base.OnBegin(clip);
            if (EffectObj == null)
            {
                m_Status = Status.Error;
                return;
            }
            OriginalParent = EffectObj.transform.parent;
            if (Parent != null)
            {
                EffectObj.transform.SetParent(null);
            }
            if (DontChangeTransform == false)
            {
                if (IgnoreScale)
                {
                    EffectObj.transform.localPosition = Position;
                    EffectObj.transform.localScale = LocalScale;
                    EffectObj.transform.eulerAngles = EulerAngles;
                }
                else
                {
                    EffectObj.transform.position = Position;
                    EffectObj.transform.localScale = LocalScale;
                    EffectObj.transform.eulerAngles = EulerAngles;
                }
            }
            EffectObj.SetActive(true);
        }

        public override void OnEnd(Clip clip)
        {
            base.OnEnd(clip);
            if (EffectObj != null)
            {
                EffectObj.transform.SetParent(OriginalParent);
                EffectObj.SetActive(false);
            }
        }

        public override void Clone(ClipAction obj)
        {
            base.Clone(obj);
            PlayEffectAction action = obj as PlayEffectAction;
            if (action != null)
            {
                EffectObj = action.EffectObj;
                Parent = action.Parent;
                DontChangeTransform = action.DontChangeTransform;
                LocalScale = action.LocalScale;
                Position = action.Position;
                EulerAngles = action.EulerAngles;
            }
        }

        public void SetEffectObj(GameObject obj)
        {
            if (EffectObj != null)
            {
                EffectObj.transform.SetParent(OriginalParent);
                EffectObj.SetActive(false);
            }
            EffectObj = obj;
        }

#if UNITY_EDITOR
        public override void DisplayEditorView()
        {
            base.DisplayEditorView();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EffectObj", GUILayout.Width(100));
            EffectObj = (GameObject)EditorGUILayout.ObjectField(EffectObj, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parent", GUILayout.Width(100));
            Parent = (Transform)EditorGUILayout.ObjectField(Parent, typeof(Transform), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("DontChangeTransform", GUILayout.Width(100));
            DontChangeTransform = EditorGUILayout.Toggle(DontChangeTransform);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("IgnoreScale", GUILayout.Width(100));
            IgnoreScale = EditorGUILayout.Toggle(IgnoreScale);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Position", GUILayout.Width(100));
            Position = EditorGUILayout.Vector3Field("", Position);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EulerAngles", GUILayout.Width(100));
            EulerAngles = EditorGUILayout.Vector3Field("", EulerAngles);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("LocalScale", GUILayout.Width(100));
            LocalScale = EditorGUILayout.Vector3Field("", LocalScale);
            GUILayout.EndHorizontal();
        }
#endif
    }
}
