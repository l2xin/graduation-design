/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTAnimator.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/13 14:01:05
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
    #region [class]GTAnimator
    public class GTAnimator : MonoBehaviour
    {
        #region [enum]Status
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

        #region [enum]PlayMode
        public enum PlayMode
        {
            StopPrevious,
            Waitting,
            DontPlayWhenBusying,
        }
        #endregion

        #region [class]PlayTask
        public class PlayTask
        {
            public Handler Handler;
            public string[] WannaPlayClips;
            public bool PlayAllClip;
            public PlayMode PlayMode;
            public bool PlayImmediately;
            public PlayTask() { }
            public PlayTask(Handler handler)
            {
                Handler = handler;
            }
        }

        #endregion

        #region [class]Handler
        public class Handler
        {
            #region [enum]HandlerMsgType
            public enum MsgType
            {
                OnAnimatorCannotPlay,
                OnAnimatorPreBegin,
                OnAnimatorBegin,
                OnAnimatorUpdate,
                OnAnimatorUpdatePercentage,
                OnAnimatorEnd,
                OnAnimatorPause,
                OnAnimatorUnPause,
                OnAnimatorStop,
            }
            #endregion

            #region [delegate & event]AnimatorStatusCallback
            public delegate void AnimatorStatusCallback(Handler.MsgType type, params object[] args);
            public AnimatorStatusCallback Callback;
            #endregion

            #region [Fields]
            #region  private
            private GTAnimator m_Animator;
            #endregion

            #region  public
            public GTAnimator Animator
            {
                get { return m_Animator; }
                set
                {
                    if (m_Animator != null)
                    {
                        ClearListen(m_Animator);
                    }
                    m_Animator = value;
                    if (m_Animator != null)
                    {
                        AddListen(m_Animator);
                    }
                }
            }
            #endregion
            #endregion

            #region [Function]

            #region listen of animator status
            public void OnAnimatorCannotPlay(GTAnimator animator)
            {
                if (Callback != null)
                {
                    Callback(MsgType.OnAnimatorCannotPlay, animator);
                }
                ClearListen(animator);
            }
            public void OnAnimatorPreBegin(GTAnimator animator, PlayTask task)
            {
                if (Callback != null)
                {
                    Callback(MsgType.OnAnimatorPreBegin, animator);
                }
            }
            public void OnAnimatorBegin(GTAnimator animator)
            {
                if (Callback != null)
                {
                    Callback(MsgType.OnAnimatorBegin, animator);
                }
            }
            public void OnAnimatorUpdate(GTAnimator animator)
            {
                if (Callback != null)
                {
                    Callback(MsgType.OnAnimatorUpdate, animator);
                }
            }
            public void OnAnimatorUpdatePercentage(GTAnimator animator, float currentPercentage, float currentPlayTime)
            {
                if (Callback != null)
                {
                    Callback(MsgType.OnAnimatorUpdatePercentage, animator, currentPercentage, currentPlayTime);
                }
            }
            public void OnAnimatorEnd(GTAnimator animator)
            {
                if (Callback != null)
                {
                    Callback(MsgType.OnAnimatorEnd, animator);
                }
                ClearListen(animator);
            }
            public void OnAnimatorStop(GTAnimator animator)
            {
                if (Callback != null)
                {
                    Callback(MsgType.OnAnimatorStop, animator);
                }
            }
            public void OnAnimatorPause(GTAnimator animator, float currentTime)
            {
                if (Callback != null)
                {
                    Callback(MsgType.OnAnimatorPause, animator, currentTime);
                }
            }
            public void OnAnimatorUnPause(GTAnimator animator, float currentTime, float pauseTime)
            {
                if (Callback != null)
                {
                    Callback(MsgType.OnAnimatorUnPause, animator, currentTime, pauseTime);
                }
            }
            /// <summary>
            /// clear listen when animator end or error stop
            /// </summary>
            /// <param name="animator"></param>
            private void ClearListen(GTAnimator animator)
            {
                if (animator != null)
                {
                    animator.CannotPlayDelegate -= OnAnimatorCannotPlay;
                    animator.PreBeginDelegate -= OnAnimatorPreBegin;
                    animator.BeginDelegate -= OnAnimatorBegin;
                    animator.UpdateDelegate -= OnAnimatorUpdate;
                    animator.UpdatePercentageDelegate -= OnAnimatorUpdatePercentage;
                    animator.EndDelegate -= OnAnimatorEnd;
                    animator.PauseDelegate -= OnAnimatorPause;
                    animator.UnPauseDelegate -= OnAnimatorUnPause;
                    animator.StopDelegate -= OnAnimatorStop;
                }
            }
            /// <summary>
            /// add listen when attach to an animator
            /// </summary>
            /// <param name="animator"></param>
            private void AddListen(GTAnimator animator)
            {
                if (animator != null)
                {
                    animator.CannotPlayDelegate += OnAnimatorCannotPlay;
                    animator.PreBeginDelegate += OnAnimatorPreBegin;
                    animator.BeginDelegate += OnAnimatorBegin;
                    animator.UpdateDelegate += OnAnimatorUpdate;
                    animator.UpdatePercentageDelegate += OnAnimatorUpdatePercentage;
                    animator.EndDelegate += OnAnimatorEnd;
                    animator.PauseDelegate += OnAnimatorPause;
                    animator.UnPauseDelegate += OnAnimatorUnPause;
                    animator.StopDelegate += OnAnimatorStop;
                }
            }
            #endregion

            #endregion
        }
        #endregion

        #region [delegate & event]
        public delegate void AnimatorCannotPlayDelegate(GTAnimator animator);
        public delegate void AnimatorPreBeginDelegate(GTAnimator animator, PlayTask task);
        public delegate void AnimatorBeginDelegate(GTAnimator animator);
        public delegate void AnimatorUpdateDelegate(GTAnimator animator);
        public delegate void AnimatorUpdatePercentageDelegate(GTAnimator animator, float currentPlayTime, float currentPecentage);
        public delegate void AnimatorEndDelegate(GTAnimator animator);
        public delegate void AnimatorPauseDelegate(GTAnimator animator, float currentTime);
        public delegate void AnimatorUnPauseDelegate(GTAnimator animator, float currentTime, float pauseTime);
        public delegate void AnimatorStopDelegate(GTAnimator animator);

        public event AnimatorCannotPlayDelegate CannotPlayDelegate;
        public event AnimatorPreBeginDelegate PreBeginDelegate;
        public event AnimatorBeginDelegate BeginDelegate;
        public event AnimatorUpdateDelegate UpdateDelegate;
        public event AnimatorUpdatePercentageDelegate UpdatePercentageDelegate;
        public event AnimatorEndDelegate EndDelegate;
        public event AnimatorPauseDelegate PauseDelegate;
        public event AnimatorUnPauseDelegate UnPauseDelegate;
        public event AnimatorStopDelegate StopDelegate;
        #endregion

        #region [Fields]

        #region Common static
        public static Dictionary<string, GTAnimator> AllAnimator = new Dictionary<string, GTAnimator>();
        public static void Add(string name, GTAnimator animator)
        {
            if (animator == null) return;
            GTAnimator o = null;
            if (AllAnimator.TryGetValue(name, out o))
            {
                return;
            }
            else
            {
                AllAnimator.Add(name, animator);
            }
        }
        public static void Remove(string name)
        {
            AllAnimator.Remove(name);
        }
        public static void Clear()
        {
            AllAnimator.Clear();
        }
        public static GTAnimator Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (AllAnimator == null || AllAnimator.Count == 0) return null;
            GTAnimator animator = null;
            AllAnimator.TryGetValue(name, out animator);
            return animator;
        }
        #endregion

        public string IdentifyName;
        public bool Add2Public;
        public float Speed = 1.0f;
        public bool IgnoreTimeScale;//not implement

        [HideInInspector]
        [NonSerialized]
        // not implement
        public bool PlayOnEnable = false;

        [HideInInspector]
        public List<Clip> Clips = new List<Clip>();
        [HideInInspector]
        [NonSerialized]
        public List<Clip> WaittingProcessClips = new List<Clip>();
        [HideInInspector]
        [NonSerialized]
        public List<Clip> ActiveClips = new List<Clip>();

        [HideInInspector]
        [NonSerialized]
        public bool IsPlay = false;

        [HideInInspector]
        [NonSerialized]
        public bool IsPause = false;

        [HideInInspector]
        [NonSerialized]
        public float CurrentTime;

        [HideInInspector]
        [NonSerialized]
        public float PauseTime;

        [HideInInspector]
        [NonSerialized]
        public List<PlayTask> PlayTaskQueue = new List<PlayTask>();

        [HideInInspector]
        [NonSerialized]
        public bool SupportQueued = true;

        [NonSerialized]
        [HideInInspector]
        public Status m_Status = Status.UnAction;
        #endregion

        #region [Function]

        #region monobehaviour
        void Awake()
        {
            if (Add2Public)
            {
                if (string.IsNullOrEmpty(IdentifyName))
                {
                    IdentifyName = Utility.GenerateID();
                }
                Add(IdentifyName, this);
            }
        }
        void Start()
        {
            if (Clips != null && Clips.Count > 0)
            {
                for (int i = 0; i < Clips.Count; i++)
                {
                    Clip clip = Clips[i];
                    if (clip == null)
                    {
                        Clips.RemoveAt(i);
                        i--;
                        continue;
                    }
                    clip.OnAnimatorInited(this);
                }
            }
        }
        void OnDestroy()
        {
            if (Add2Public)
            {
                Remove(IdentifyName);
            }
        }
        void OnEnable() { }
        void OnDisable()
        {
            if (IsPlay)
            {
                Stop();
            }
        }
        void Update()
        {
            if (IsPlay)
            {
                if (IsPause)
                {
                    PauseTime += Time.deltaTime;
                    return;
                }
                m_Status = Status.Updating;
                CurrentTime += (Time.deltaTime * Speed);
                if (UpdateDelegate != null)
                {
                    UpdateDelegate(this);
                }
                if (UpdatePercentageDelegate != null)
                {
                    UpdatePercentageDelegate(this, 0, CurrentTime);
                }
                if (WaittingProcessClips != null && WaittingProcessClips.Count > 0)
                {
                    for (int i = 0; i < WaittingProcessClips.Count; i++)
                    {
                        Clip clip = WaittingProcessClips[i];
                        if (clip == null)
                        {
                            WaittingProcessClips.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (clip.BeginTime <= CurrentTime && clip.m_Status == Clip.Status.UnAction)
                        {
                            clip.OnBegin(this);
                            ActiveClips.Add(clip);
                            WaittingProcessClips.Remove(clip);
                            i--;
                        }
                    }
                }
                if (ActiveClips != null && ActiveClips.Count > 0)
                {
                    for (int i = 0; i < ActiveClips.Count; i++)
                    {
                        Clip clip = ActiveClips[i];
                        if (clip == null)
                        {
                            ActiveClips.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (clip.EndTime <= CurrentTime)
                        {
                            clip.OnEnd(this);
                            ActiveClips.Remove(clip);
                            i--;
                        }
                        else if (clip.m_Status == Clip.Status.Error)
                        {
                            clip.OnEnd(this);
                            ActiveClips.Remove(clip);
                            i--;
                        }
                        else
                        {
                            clip.OnUpdate(this);
                        }
                    }
                }
                if (ActiveClips.Count == 0 && WaittingProcessClips.Count == 0)
                {
                    InternalStop();
                    return;
                }
            }
        }
        #endregion

        #region play control
        private void InternalPlay(PlayTask task)
        {
            Play(task);
        }
        private void InternalStop(bool forceStop = false)
        {
            IsPlay = false;
            m_Status = Status.UnAction;
            if (EndDelegate != null)
            {
                EndDelegate(this);
            }
            if (PlayTaskQueue != null && PlayTaskQueue.Count > 0)
            {
                //remove this first task
                PlayTaskQueue.RemoveAt(0);
            }
            if (forceStop == false)
            {
                if (PlayTaskQueue != null && PlayTaskQueue.Count > 0)
                {
                    PlayTask task = PlayTaskQueue[0];
                    PlayTaskQueue.RemoveAt(0);
                    InternalPlay(task);
                }
            }
            else
            {
                //if true,clear all waitting task
                PlayTaskQueue.Clear();
            }
        }
        public void Play()
        {
            PlayTask task = new PlayTask();
            task.PlayAllClip = true;
            task.PlayMode = PlayMode.Waitting;
            Play(task);
        }
        public void Play(PlayTask task)
        {
            if (task == null) return;
            if (IsPlay && task.PlayMode == PlayMode.DontPlayWhenBusying)
            {
                if (task.Handler != null)
                {
                    task.Handler.OnAnimatorCannotPlay(this);
                }
                return;
            }
            else if (IsPlay && task.PlayMode == PlayMode.Waitting)
            {
                if (SupportQueued == false)
                {
                    if (task.Handler != null)
                    {
                        task.Handler.OnAnimatorCannotPlay(this);
                    }
                    return;
                }
                else
                {
                    if (PlayTaskQueue == null)
                    {
                        PlayTaskQueue = new List<PlayTask>();
                    }
                    PlayTaskQueue.Add(task);
                }
            }
            else
            {
                PlayTaskQueue.Add(task);
                if (task.Handler != null)
                {
                    task.Handler.Animator = this;
                }
                if (PreBeginDelegate != null)
                {
                    PreBeginDelegate(this, task);
                }
                if (IsPlay && task.PlayMode == PlayMode.StopPrevious)
                {
                    Stop(false);
                }
                CurrentTime = 0.0f;
                if (task.PlayAllClip)
                {
                    WaittingProcessClips = new List<Clip>(Clips);
                }
                else
                {
                    if (task.WannaPlayClips == null || task.WannaPlayClips.Length == 0)
                    {
                        if (task.Handler != null)
                        {
                            task.Handler.OnAnimatorCannotPlay(this);
                        }
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < task.WannaPlayClips.Length; i++)
                        {
                            string clipName = task.WannaPlayClips[i];
                            if (string.IsNullOrEmpty(clipName)) continue;
                            for (int j = 0; j < Clips.Count; j++)
                            {
                                Clip clip = Clips[j];
                                if (clip == null) continue;
                                if (clip.Name == clipName)
                                {
                                    if (i == 0)
                                    {
                                        if (task.PlayImmediately)
                                        {
                                            CurrentTime = clip.BeginTime;
                                        }
                                    }
                                    WaittingProcessClips.Add(clip);
                                }
                            }
                        }
                    }
                }
                m_Status = Status.Action;
                IsPlay = true;
                IsPause = false;
                if (BeginDelegate != null)
                {
                    BeginDelegate(this);
                }
            }
        }
        public void Stop(bool clearAllTask = true)
        {
            if (WaittingProcessClips != null) WaittingProcessClips.Clear();
            if (ActiveClips.Count > 0)
            {
                for (int i = 0; i < ActiveClips.Count; i++)
                {
                    Clip clip = ActiveClips[i];
                    if (clip == null) continue;
                    clip.OnEnd(this);
                }
                ActiveClips.Clear();
            }
            if (StopDelegate != null)
            {
                StopDelegate(this);
            }
            InternalStop(clearAllTask);
        }
        public void Pause()
        {
            PauseTime = 0.0f;
            IsPause = true;
            if (PauseDelegate != null)
            {
                PauseDelegate(this, CurrentTime);
            }
        }
        public void UnPause()
        {
            IsPause = false;
            if (UnPauseDelegate != null)
            {
                UnPauseDelegate(this, CurrentTime, PauseTime);
            }
        }
        #endregion

        #region get & set
        public Clip GetClipByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (Clips == null || Clips.Count == 0) return null;
            for (int i = 0; i < Clips.Count; i++)
            {
                Clip clip = Clips[i];
                if (clip == null) continue;
                if (name.Equals(clip.Name)) return clip;
            }
            return null;
        }
        public ClipAction GetSpecificAction(string clipName, string actionName, Type type)
        {
            Clip clip = GetClipByName(clipName);
            if (clip == null)
            {
                Utility.LogError("GetSpecificAction error caused by null clip named: " + clipName);
                return null;
            }
            return clip.GetAction(actionName, type);
        }
        public ClipAction GetSpecificAction(string clipName, string actionName)
        {
            Clip clip = GetClipByName(clipName);
            if (clip == null)
            {
                Utility.LogError("GetSpecificAction error caused by null clip named: " + clipName);
                return null;
            }
            return clip.GetAction(actionName);
        }
        #endregion

        #region editor function
#if UNITY_EDITOR
        public void DisplayEditorView()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            GUILayout.EndHorizontal();
        }
#endif
        #endregion

        #endregion
    }
    #endregion
}
