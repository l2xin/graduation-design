/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: CameraShakePlugin.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/8 14:34:06
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
    public class CameraShakePlugin : MotionPlugin
    {
        public override string DisplayName
        {
            get
            {
                return "[CameraShake] ";
            }
        }
        public Vector3 Direction;
        //public float ShakeTime;
        public override void OnBegin(Motion motion)
        {
            base.OnBegin(motion);
            Camera camera = Camera.main;
            if (camera == null)
            {
                CurrentStatus = Status.StopOnNextFrame; return;
            }
            float shakeTime = EndTime - BeginTime;
            if (shakeTime <= 0)
            {
                CurrentStatus = Status.StopOnNextFrame; return;
            }
            iTween.Stop(camera.gameObject);
            iTween.PunchPosition(camera.gameObject, Direction, shakeTime);
        }
        public override void OnUpdate(Motion motion)
        {
            base.OnUpdate(motion);
        }
        public override void OnEnd(Motion motion)
        {
            base.OnEnd(motion);
        }
#if UNITY_EDITOR
        public override void DisplayEditorView(Motion motion)
        {
            base.DisplayEditorView(motion);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Direction", GUILayout.Width(140));
            Direction = EditorGUILayout.Vector3Field("", Direction);
            GUILayout.EndHorizontal();
        }
#endif
    }
}
