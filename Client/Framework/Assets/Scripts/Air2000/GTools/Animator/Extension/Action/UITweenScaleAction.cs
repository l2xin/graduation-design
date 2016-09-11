/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: UITweenScaleAction.cs
			// Describle:  
			// Created By:  hsu
			// Date&Time:  2016/5/14 10:51:04
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
    public class UITweenScaleAction : UITweenAction
    {
        public override string DisplayName
        {
            get
            {
                return "[TweenScale] " + IdentifyName;
            }
        }
        public TweenScale Tweener;

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
                Tweener.Toggle();
                //  Tweener.enabled = false;
            }
            if (Reset)
            {
                if (Tweener != null && Tweener.gameObject != null)
                {
                    Tweener.gameObject.transform.localScale = Tweener.from;
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
            UITweenScaleAction action = obj as UITweenScaleAction;
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
            Tweener = (TweenScale)EditorGUILayout.ObjectField(Tweener, typeof(TweenScale), true);
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
