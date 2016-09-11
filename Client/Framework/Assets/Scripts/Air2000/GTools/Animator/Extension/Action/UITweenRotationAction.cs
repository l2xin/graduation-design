/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: UITweenRotationAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/14 13:15:05
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
    public class UITweenRotationAction : UITweenAction
    {
        public override string DisplayName
        {
            get
            {
                return "[TweenRotation] " + IdentifyName;
            }
        }
        public TweenRotation Tweener;

        [HideInInspector]
        [NonSerialized]
        public EventDelegate TweenEndCallback;
        public override void OnBegin(Clip clip)
        {
            base.OnBegin(clip);
            if (Tweener == null) { m_Status = Status.Error; return; }
            if (TweenEndCallback == null)
            {
                TweenEndCallback = new EventDelegate() { methodName = "OnTweenEnd" };
            }
            Tweener.AddOnFinished(TweenEndCallback);
            Tweener.ResetToBeginning();
            Tweener.Toggle();
        }
        public void OnTweenEnd()
        {
            if (Tweener != null)
            {
                Tweener.RemoveOnFinished(TweenEndCallback);
                Tweener.enabled = false;
            }
            if (Reset)
            {
                if (Tweener != null && Tweener.gameObject != null)
                {
                    Tweener.gameObject.transform.eulerAngles = Tweener.from;
                }
            }
            m_Status = Status.StopOnNextFrame;
        }
        public override void OnEnd(Clip clip)
        {
            base.OnEnd(clip);
        }

        public override void Clone(ClipAction obj)
        {
            base.Clone(obj);
            UITweenRotationAction action = obj as UITweenRotationAction;
            if (action != null)
            {
                Reset = action.Reset;
                Tweener = action.Tweener;
            }
        }

#if UNITY_EDITOR
        public override void DisplayEditorView()
        {
            base.DisplayEditorView();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tweener", GUILayout.Width(100));
            Tweener = (TweenRotation)EditorGUILayout.ObjectField(Tweener, typeof(TweenRotation), true);
            GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("BeginTime", GUILayout.Width(100));
            //BeginTime = EditorGUILayout.FloatField(BeginTime);
            //GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("EndTime", GUILayout.Width(100));
            //EndTime = EditorGUILayout.FloatField(EndTime);
            //GUILayout.EndHorizontal();


        }
#endif
    }
}
