/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: EaseAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/13 17:52:14
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace GTools.Animator
{
    public enum ResetMode
    {
        DontReset,
        OnClipBegin,
        OnActionBegin,
    }
    [Serializable]
    public abstract class EaseAction : ClipAction
    {
        public float EaseTime;
        public iTween.EaseType EaseType;
        public ResetMode HowToReset;
        public bool Reset = true;
        public GameObject OverrideObject;
        [NonSerialized]
        public GameObject AnimateObj;
        public Transform Target;

        public Vector3 Vec3Target;

        public override void OnClipInited(Clip clip)
        {
            base.OnClipInited(clip);
            if (clip != null)
            {
                if (OverrideObject == null)
                {
                    AnimateObj = clip.AnimateObj;
                }
                else
                {
                    AnimateObj = OverrideObject;
                }
            }
        }
        public override void OnBegin(Clip clip)
        {
            base.OnBegin(clip);
        }
        public override void Clone(ClipAction obj)
        {
            base.Clone(obj);
            if (obj != null)
            {
                if (obj.GetType() == typeof(MoveAction))
                {
                    MoveAction action = obj as MoveAction;
                    AnimateObj = action.AnimateObj;
                    EaseTime = action.EaseTime;
                    EaseType = action.EaseType;
                    HowToReset = action.HowToReset;
                    Target = action.Target;
                    Vec3Target = action.Vec3Target;
                }
                else if (obj.GetType() == typeof(ScaleAction))
                {
                    ScaleAction action = obj as ScaleAction;
                    AnimateObj = action.AnimateObj;
                    EaseTime = action.EaseTime;
                    EaseType = action.EaseType;
                    HowToReset = action.HowToReset;
                    Target = action.Target;
                    Vec3Target = action.Vec3Target;
                }
                else if (obj.GetType() == typeof(RotateAction))
                {
                    RotateAction action = obj as RotateAction;
                    AnimateObj = action.AnimateObj;
                    EaseTime = action.EaseTime;
                    EaseType = action.EaseType;
                    HowToReset = action.HowToReset;
                    Target = action.Target;
                    Vec3Target = action.Vec3Target;
                }
            }
        }

#if UNITY_EDITOR
        public override void DisplayEditorView()
        {
            base.DisplayEditorView();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EaseTime", GUILayout.Width(100));
            EaseTime = EditorGUILayout.FloatField(EaseTime);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EaseType", GUILayout.Width(100));
            EaseType = (iTween.EaseType)EditorGUILayout.EnumPopup(EaseType);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ResetMode", GUILayout.Width(100));
            HowToReset = (ResetMode)EditorGUILayout.EnumPopup(HowToReset);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Reset", GUILayout.Width(100));
            Reset = EditorGUILayout.Toggle(Reset);
            GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("ObjStatusOperation", GUILayout.Width(100));
            //ObjStatusOperation = (GameObjectStatusOperation)EditorGUILayout.EnumPopup(ObjStatusOperation);
            //GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("AnimateType", GUILayout.Width(100));
            //AnimateType = (EaseActionType)EditorGUILayout.EnumPopup(AnimateType);
            //GUILayout.EndHorizontal();

            GUILayout.Space(3.0f);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("OverrideObject", GUILayout.Width(100));
            OverrideObject = (GameObject)EditorGUILayout.ObjectField(OverrideObject, typeof(GameObject), true);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target", GUILayout.Width(100));
            Target = (Transform)EditorGUILayout.ObjectField(Target, typeof(Transform), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Vec3Target", GUILayout.Width(100));
            Vec3Target = EditorGUILayout.Vector3Field("", Vec3Target);
            GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("Calculator", GUILayout.Width(100));
            //Calculator = (DynamicCalculator)EditorGUILayout.ObjectField(Calculator, typeof(DynamicCalculator), true);
            //GUILayout.EndHorizontal();

            //  EditorGUILayout.HelpBox("It will be set to target while Target(Transform) is null", MessageType.Info);

        }
#endif
    }
}
