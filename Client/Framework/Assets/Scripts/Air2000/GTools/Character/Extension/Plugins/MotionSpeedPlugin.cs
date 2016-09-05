/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: MotionSpeedPlugin.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/8 8:33:11
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GTools.Character
{
    [Serializable]
    public class MotionSpeedPlugin : MotionPlugin
    {
        public override string DisplayName
        {
            get
            {
                return "[MotionSpeed] ";
            }
        }
        public float TargetSpeed;
        [NonSerialized]
        public float OriginalSpeed;

        public override void OnBegin(Motion motion)
        {
            base.OnBegin(motion);
            if (Motion == null || TargetSpeed < 0)
            {
                CurrentStatus = Status.StopOnNextFrame; return;
            }
            AnimationState state = Motion.GetAnimationState();
            if (state == null)
            {
                CurrentStatus = Status.StopOnNextFrame; return;
            }
            OriginalSpeed = state.speed;
            state.speed = TargetSpeed;
        }
        public override void OnEnd(Motion motion)
        {
            base.OnEnd(motion);
            if (Motion != null)
            {
                AnimationState state = Motion.GetAnimationState();
                if (state != null)
                {
                    state.speed = OriginalSpeed;
                }
            }
        }
        public override void Clone(MotionPlugin obj)
        {
            base.Clone(obj);
            MotionSpeedPlugin plugin = obj as MotionSpeedPlugin;
            if (plugin != null)
            {
                TargetSpeed = plugin.TargetSpeed;
            }
        }

#if UNITY_EDITOR
        public override void DisplayEditorView(Motion motion)
        {
            base.DisplayEditorView(motion);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TargetSpeed", GUILayout.Width(140));
            TargetSpeed = EditorGUILayout.FloatField(TargetSpeed);
            GUILayout.EndHorizontal();
        }
#endif
    }
}
