/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ClipAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/13 14:04:30
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
    public class ClipAction
    {
        #region [delegate & event]
        public delegate void BeginDelegate(ClipAction action);
        public delegate void UpdateDelegate(ClipAction action);
        public delegate void EndDelegate(ClipAction action);
        public event BeginDelegate OnBeginDelegate;
        public event UpdateDelegate OnUpdateDelegate;
        public event EndDelegate OnEndDelegate;
        #endregion

        #region [class] Status
        public enum Status
        {
            UnAction,
            Action,
            Updating,
            WaittingForLastActionEnd,
            StopOnNextFrame,
            Error,
        }
        #endregion
        public virtual string DisplayName { get { return "Not implement"; } }

        [HideInInspector]
        [NonSerialized]
        public Status m_Status = Status.UnAction;
        public string IdentifyName;
        public float BeginTime;
        public float EndTime;
        public virtual void OnClipInited(Clip clip) { }
        public virtual void OnClipBegin(Clip clip) { }
        public virtual void OnBegin(Clip clip) { if (OnBeginDelegate != null)OnBeginDelegate(this); m_Status = Status.Action; }
        public virtual void OnUpdate(Clip clip) { if (OnUpdateDelegate != null)OnUpdateDelegate(this); }
        public virtual void OnEnd(Clip clip) { m_Status = Status.UnAction; if (OnEndDelegate != null)OnEndDelegate(this); }
        public virtual void OnClipEnd(Clip clip) { }
        public virtual void Clone(ClipAction obj)
        {
            if (obj != null)
            {
                BeginTime = obj.BeginTime;
                EndTime = obj.EndTime;
                IdentifyName = obj.IdentifyName;
            }
        }
        #region editor function
#if UNITY_EDITOR
        public virtual void DisplayEditorView()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("IdentifyName", GUILayout.Width(100));
            IdentifyName = EditorGUILayout.TextField(IdentifyName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("BeginTime", GUILayout.Width(100));
            BeginTime = EditorGUILayout.FloatField(BeginTime);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EndTime", GUILayout.Width(100));
            EndTime = EditorGUILayout.FloatField(EndTime);
            GUILayout.EndHorizontal();
        }
#endif
        #endregion
    }
}
