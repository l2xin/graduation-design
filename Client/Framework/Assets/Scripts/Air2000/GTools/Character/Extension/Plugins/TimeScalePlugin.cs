/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: TimeScalePlugin.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/8 14:57:02
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

namespace Air2000.Character
{
    [Serializable]
    public class TimeScalePlugin : MotionPlugin
    {
        public override string DisplayName
        {
            get
            {
                return "[TimeScale] ";
            }
        }
        public float TimeScale;

#if UNITY_EDITOR
        public float LastTimeScale=-1f;
#endif

        public override void OnBegin(Motion motion)
        {
            base.OnBegin(motion);
            Time.timeScale = TimeScale;
        }
        public override void OnUpdate(Motion motion)
        {
            base.OnUpdate(motion);
        }
        public override void OnEnd(Motion motion)
        {
            base.OnEnd(motion);
            Time.timeScale = 1;
        }
        public override void Clone(MotionPlugin obj)
        {
            base.Clone(obj);
        }
        public override void Destroy()
        {
            base.Destroy();
        }
#if UNITY_EDITOR
        public override void DisplayEditorView(Motion motion)
        {
            base.DisplayEditorView(motion);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TimeScale", GUILayout.Width(140));
            TimeScale = EditorGUILayout.FloatField(TimeScale);
            //if (LastTimeScale != TimeScale)
            //{
            //    LastTimeScale = TimeScale;
            //    float deltaTime = EndTime - BeginTime;
            //    deltaTime *= TimeScale;
            //    EndTime = BeginTime + deltaTime;
            //}
            GUILayout.EndHorizontal();
        }
#endif
    }
}
