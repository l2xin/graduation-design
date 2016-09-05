/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: MotionPlugin.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/5 13:31:28
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GTools.Character
{
    [Serializable]
    public class MotionPlugin
    {
        #region [delegate & event]
        public delegate void BeginDelegate(MotionPlugin action);
        public delegate void UpdateDelegate(MotionPlugin action);
        public delegate void EndDelegate(MotionPlugin action);
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

        [NonSerialized]
        public Status CurrentStatus = Status.UnAction;
        public string IdentifyName;
        public float BeginTime;
        public float EndTime;
        public bool PlayOnMotionEnd = false;
        public bool DontStopOnMotionEnd;
        [NonSerialized]
        public MotionMachine Machine;
        [NonSerialized]
        public Motion Motion;
        public Timer.Handle DelayHandler;

        #region motionmachine behaviour function
        public virtual void OnMachineAwake(MotionMachine machine, Motion motion) { Machine = machine; Motion = motion; }
        public virtual void OnMachineStart(MotionMachine machine, Motion motion) { }
        public virtual void OnMachineEnable(MotionMachine machine, Motion motion) { }
        public virtual void OnMachineDisable(MotionMachine machine, Motion motion) { }
        public virtual void OnMachineDestroy(MotionMachine machine, Motion motion) { }
        #endregion
        public virtual void OnMotionBegin(Motion motion) { if (DelayHandler != null && DelayHandler.Active) DelayHandler.Cancel(); }
        public virtual void OnBegin(Motion motion) { if (OnBeginDelegate != null)OnBeginDelegate(this); CurrentStatus = Status.Action; }
        public virtual void OnUpdate(Motion motion) { if (OnUpdateDelegate != null)OnUpdateDelegate(this); }
        public virtual void OnEnd(Motion motion) { CurrentStatus = Status.UnAction; if (OnEndDelegate != null)OnEndDelegate(this); }
        public virtual void OnMotionEnd(Motion motion)
        {
            if (PlayOnMotionEnd && EndTime > 0)
            {
                if (DelayHandler != null && DelayHandler.Active) DelayHandler.Cancel();
                DelayHandler = new Timer.Handle();
                Timer.In(EndTime, OnTimerEnd);
                OnTimerBegin();
            }
            else if (DontStopOnMotionEnd)
            {
                float deltaTime = EndTime - motion.CurrentTime;
                if (deltaTime <= 0)
                {
                    OnEnd(motion);
                }
                else
                {
                    if (DelayHandler != null && DelayHandler.Active) DelayHandler.Cancel();
                    DelayHandler = new Timer.Handle();
                    Timer.In(deltaTime, OnTimerEnd);
                }
            }
        }
        public virtual void OnTimerBegin() { CurrentStatus = Status.Action; if (OnBeginDelegate != null)OnBeginDelegate(this); }
        public virtual void OnTimerContinue() { }
        public virtual void OnTimerEnd() { CurrentStatus = Status.UnAction; if (OnEndDelegate != null)OnEndDelegate(this); }
        public virtual void Clone(MotionPlugin obj)
        {
            if (obj != null)
            {
                BeginTime = obj.BeginTime;
                EndTime = obj.EndTime;
                IdentifyName = obj.IdentifyName;
            }
        }
        public virtual void Destroy() { }
        #region editor function
#if UNITY_EDITOR
        public virtual void DisplayEditorView(Motion motion)
        {
            Motion = motion;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("IdentifyName", GUILayout.Width(140));
            IdentifyName = EditorGUILayout.TextField(IdentifyName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("PlayOnMotionEnd", GUILayout.Width(140));
            PlayOnMotionEnd = EditorGUILayout.Toggle(PlayOnMotionEnd);
            GUILayout.EndHorizontal();

            if (PlayOnMotionEnd == false)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("DontStopOnMotionEnd", GUILayout.Width(140));
                DontStopOnMotionEnd = EditorGUILayout.Toggle(DontStopOnMotionEnd);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("BeginTime", GUILayout.Width(140));
                BeginTime = EditorGUILayout.FloatField(BeginTime);
                GUILayout.EndHorizontal();

            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EndTime", GUILayout.Width(140));
            EndTime = EditorGUILayout.FloatField(EndTime);
            GUILayout.EndHorizontal();

            GUILayout.Space(3.0f);
        }
        public virtual void OnPreReplaceAnimation(Motion motion) { }
        public virtual void OnReplacedAnimation(Motion motion) { }
        public virtual void UpdateDependency(Motion motion) { }
#endif
        #endregion
    }
}
