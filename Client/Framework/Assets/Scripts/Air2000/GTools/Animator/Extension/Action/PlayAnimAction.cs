/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: PlayAnimAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/18 12:30:53
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
    public class PlayAnimAction : ClipAction
    {
        public Animation AnimationObj;
        public string ClipName;
        public float CrossFade;
        public float Speed;
        public bool Loop;

        [NonSerialized]
        public AnimationState AnimState;
        [NonSerialized]
        public AnimationClip AnimClip;
        [NonSerialized]
        public float OriginalSpeed;
        [NonSerialized]
        public WrapMode OriginalWrapMode;

        public override string DisplayName
        {
            get
            {
                return "[PlayAnim] " + IdentifyName;
            }
        }
        public override void OnBegin(Clip clip)
        {
            base.OnBegin(clip);
            if (AnimationObj == null)
            {
                m_Status = Status.Error;
                return;
            }
            if (string.IsNullOrEmpty(ClipName))
            {
                m_Status = Status.Error;
                return;
            }
            AnimClip = AnimationObj.GetClip(ClipName);
            if (AnimClip == null)
            {
                m_Status = Status.Error;
                return;
            }
            AnimState = AnimationObj[ClipName];
            if (AnimState == null)
            {
                m_Status = Status.Error;
                return;
            }
            OriginalWrapMode = AnimClip.wrapMode;
            OriginalSpeed = AnimState.speed;


            if (Loop)
            {
                AnimClip.wrapMode = WrapMode.Loop;
            }
            if (Speed <= 0)
            {
                Speed = 1;
            }
            AnimState.speed = Speed;
            if (CrossFade < 0)
            {
                CrossFade = 0;
            }
            AnimationObj.CrossFade(ClipName, CrossFade);
        }

        public override void OnEnd(Clip clip)
        {
            base.OnEnd(clip);
            if (AnimationObj != null)
            {
                if (AnimClip != null)
                {
                    AnimClip.wrapMode = OriginalWrapMode;
                }
                if (AnimState != null)
                {
                    AnimState.speed = OriginalSpeed;
                }
                AnimationObj.Stop();
            }
        }

        public override void Clone(ClipAction obj)
        {
            base.Clone(obj);
            PlayAnimAction action = obj as PlayAnimAction;
            if (obj != null)
            {
                AnimationObj = action.AnimationObj;
                ClipName = action.ClipName;
                CrossFade = action.CrossFade;
                Speed = action.Speed;
                Loop = action.Loop;
            }
        }

#if UNITY_EDITOR
        public override void DisplayEditorView()
        {
            base.DisplayEditorView();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AnimationObj", GUILayout.Width(100));
            AnimationObj = (Animation)EditorGUILayout.ObjectField(AnimationObj, typeof(Animation), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ClipName", GUILayout.Width(100));
            ClipName = EditorGUILayout.TextField(ClipName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("CrossFade", GUILayout.Width(100));
            CrossFade = EditorGUILayout.FloatField(CrossFade);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speed", GUILayout.Width(100));
            Speed = EditorGUILayout.FloatField(Speed);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Loop", GUILayout.Width(100));
            Loop = EditorGUILayout.Toggle(Loop);
            GUILayout.EndHorizontal();
        }
#endif
    }
}
