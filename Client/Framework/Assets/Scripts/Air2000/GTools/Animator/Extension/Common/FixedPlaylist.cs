/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: FixedPlaylist.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/17 21:06:43
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Air2000;
using GameLogic;
using System.Reflection;

namespace GTools.Animator
{
    public class FixedPlaylist : MonoBehaviour
    {
        #region listen
        public delegate void EndDelegate(FixedPlaylist list);
        public event EndDelegate ListenEnd;
        #endregion

        [Serializable]
        public class Description
        {
            public float Time;
            public GTAnimator Animator;
            public string IdentifyName;
            public GTAnimator RelyOn;
            public string RelyOnAnimatorName;
            public UnityEngine.Object AdapterType;
            public GameObject AdapterObj;
            public Component Adapter;
            public bool CallbackOnEnd = false;
            public bool DriveOnStart = false;
            //public bool PlayAllClip = true;
            //public string SpecificClip;
        }
        public UnityEngine.Object AdapterType;
        public GameObject AdapterObj;
        [NonSerialized]
        [HideInInspector]
        public Component Adapter;
        public List<Description> Playlist = new List<Description>();

        [HideInInspector]
        [NonSerialized]
        public bool IsPlaying;

        [HideInInspector]
        [NonSerialized]
        public GTAnimator LastAnimator;
        [HideInInspector]
        [NonSerialized]
        public GTAnimator CurrentAnimator;

        [NonSerialized]
        public List<Description> Animators = new List<Description>();
        [HideInInspector]
        [NonSerialized]
        public List<Description> WaittingProcessAnimators = new List<Description>();
        [HideInInspector]
        [NonSerialized]
        public List<Description> ActiveAnimators = new List<Description>();

        [HideInInspector]
        [NonSerialized]
        public float CurrentTime;

        private void Awake()
        {
            if (Playlist != null && Playlist.Count > 0)
            {
                for (int i = 0; i < Playlist.Count; i++)
                {
                    Description des = Playlist[i];
                    if (des == null) continue;
                    if (string.IsNullOrEmpty(des.IdentifyName) == false)
                    {
                        des.Animator = GTAnimator.Get(des.IdentifyName);
                    }
                    if (string.IsNullOrEmpty(des.RelyOnAnimatorName) == false)
                    {
                        des.RelyOn = GTAnimator.Get(des.RelyOnAnimatorName);
                    }
                    if (des.AdapterType != null && des.Animator != null)
                    {
                        Component com = null;
                        if (des.AdapterObj != null)
                        {
                            com = des.AdapterObj.GetComponent(des.AdapterType.name);
                        }
                        else
                        {
                            com = des.Animator.GetComponent(des.AdapterType.name);
                        }
                        if (des.DriveOnStart == false)
                        {
                            if (com != null)
                            {
                                MethodInfo info = com.GetType().GetMethod("Drive");
                                if (info != null)
                                {
                                    com.GetType().InvokeMember("Drive", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, com, new object[] { });
                                }
                                else
                                {
                                    Utility.LogError("FixedPlaylist: can not drive your adapter caused by null method named 'Drive(void)'");
                                }
                            }
                        }
                        des.Adapter = com;
                    }
                }
            }
            if (AdapterType != null)
            {
                if (AdapterObj != null)
                {
                    Adapter = AdapterObj.GetComponent(AdapterType.name);
                }
                else
                {
                    Adapter = GetComponent(AdapterType.name);
                }
                if (Adapter != null)
                {
                    MethodInfo info = Adapter.GetType().GetMethod("Drive");
                    if (info != null)
                    {
                        Adapter.GetType().InvokeMember("Drive", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, Adapter, new object[] { });
                    }
                    else
                    {
                        //Utility.LogError("FixedPlaylist: can not drive your adapter caused by null method named 'Drive(void)'");
                    }
                }
            }
        }
        void Start()
        {
            if (Playlist != null && Playlist.Count > 0)
            {
                for (int i = 0; i < Playlist.Count; i++)
                {
                    Description des = Playlist[i];
                    if (des == null) continue;
                    if (des.Adapter != null && des.DriveOnStart == true)
                    {
                        MethodInfo info = des.Adapter.GetType().GetMethod("Drive");
                        if (info != null)
                        {
                            des.Adapter.GetType().InvokeMember("Drive", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, des.Adapter, new object[] { });
                        }
                        else
                        {
                            Utility.LogError("FixedPlaylist: can not drive your adapter caused by null method named 'Drive(void)'");
                        }
                    }
                }
            }
        }
        private void OnDisable()
        {
            IsPlaying = false;
        }
        private void OnDestroy()
        {
            Playlist = null;
            ListenEnd = null;
            LastAnimator = null;
            CurrentAnimator = null;
        }
        public void Play(bool forceFinish = false, EndDelegate callback = null)
        {
            if (forceFinish == true)
            {
                Stop();
            }
            if (IsPlaying)
            {
                if (callback != null)
                {
                    callback(this);
                }
                return;
            }
            WaittingProcessAnimators = new List<Description>(Playlist);
            ActiveAnimators = new List<Description>();
            IsPlaying = true;
            CurrentTime = 0.0f;
            ListenEnd += callback;
        }
        void Update()
        {
            if (IsPlaying)
            {
                CurrentTime += Time.deltaTime;
                if (WaittingProcessAnimators != null && WaittingProcessAnimators.Count > 0)
                {
                    for (int i = 0; i < WaittingProcessAnimators.Count; i++)
                    {
                        Description des = WaittingProcessAnimators[i];
                        if (des == null)
                        {
                            continue;
                        }
                        if (des.Time == -1)
                        {
                            continue;
                        }
                        if (des.Animator == null)
                        {
                            continue;
                        }
                        if (CurrentTime >= des.Time)
                        {
                            //if (name == "MyTurnAndRoll")
                            //{
                            //    Debug.LogError("Fixedlist: wanna play " + des.Animator.name);
                            //}
                            //if (des.Animator.m_Status == GTAnimator.Status.UnAction)
                            //{
                            GTAnimator.PlayTask task = new GTAnimator.PlayTask();
                            task.PlayAllClip = true;
                            task.PlayMode = GTAnimator.PlayMode.StopPrevious;

                            des.Animator.PreBeginDelegate += OnOneAnimatorPreStart;
                            des.Animator.BeginDelegate += OnOneAnimatorStart;
                            des.Animator.UpdateDelegate += OnOneAnimatorUpdate;
                            des.Animator.EndDelegate += OnOneAnimatorEnd;
                            des.Animator.Play(task);
                            WaittingProcessAnimators.Remove(des);
                            ActiveAnimators.Add(des);
                            //}
                            //else
                            //{
                            //    Debug.LogError("Fixedlist: play error " + des.Animator.name + ", status is " + des.Animator.m_Status.ToString());
                            //}
                        }
                    }
                }
                if (WaittingProcessAnimators.Count == 0 && ActiveAnimators.Count == 0)
                {
                    PlayEnd();
                }
            }
        }
        private void OnOneAnimatorPreStart(GTAnimator animator, GTAnimator.PlayTask task)
        {
            if (animator == null) return;
            if (Adapter != null)
            {
                MethodInfo info = Adapter.GetType().GetMethod("OnAnimatorPreStart");
                if (info != null)
                {
                    Adapter.GetType().InvokeMember("OnAnimatorPreStart", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, Adapter, new object[] { animator, task });
                }
                //else
                //{
                //  //  Utility.LogError("FixedPlaylist: can not callback your adapter caused by null method named 'OnAnimatorStart(void)'");
                //}
            }
            Description des = GetDescription(animator);
            if (des == null || des.Animator == null) return;
            if (des.Adapter != null)
            {
                MethodInfo info = des.Adapter.GetType().GetMethod("OnAnimatorPreStart");
                if (info != null)
                {
                    des.Adapter.GetType().InvokeMember("OnAnimatorPreStart", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, des.Adapter, new object[] { animator, task });
                }
                else
                {
                    Utility.LogError("FixedPlaylist: can not callback your adapter caused by null method named 'OnAnimatorPreStart(void)'");
                }
            }
        }
        private void OnOneAnimatorStart(GTAnimator animator)
        {
            if (animator == null) return;
            if (Adapter != null)
            {
                MethodInfo info = Adapter.GetType().GetMethod("OnAnimatorStart");
                if (info != null)
                {
                    Adapter.GetType().InvokeMember("OnAnimatorStart", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, Adapter, new object[] { animator });
                }
                //else
                //{
                //  //  Utility.LogError("FixedPlaylist: can not callback your adapter caused by null method named 'OnAnimatorStart(void)'");
                //}
            }
            Description des = GetDescription(animator);
            if (des == null || des.Animator == null) return;
            if (des.Adapter != null)
            {
                MethodInfo info = des.Adapter.GetType().GetMethod("OnAnimatorStart");
                if (info != null)
                {
                    des.Adapter.GetType().InvokeMember("OnAnimatorStart", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, des.Adapter, new object[] { animator });
                }
                else
                {
                    Utility.LogError("FixedPlaylist: can not callback your adapter caused by null method named 'OnAnimatorStart(void)'");
                }
            }
        }
        private void OnOneAnimatorUpdate(GTAnimator animator)
        {
            if (animator == null) return;
            if (Adapter != null)
            {
                MethodInfo info = Adapter.GetType().GetMethod("OnAnimatorUpdate");
                if (info != null)
                {
                    Adapter.GetType().InvokeMember("OnAnimatorUpdate", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, Adapter, new object[] { animator });
                }
                else
                {
                    //Utility.LogError("FixedPlaylist: can not callback your adapter caused by null method named 'OnAnimatorUpdate(void)'");
                }
            }
            Description des = GetDescription(animator);
            if (des == null || des.Animator == null) return;
            if (des.Adapter != null)
            {
                MethodInfo info = des.Adapter.GetType().GetMethod("OnAnimatorUpdate");
                if (info != null)
                {
                    des.Adapter.GetType().InvokeMember("OnAnimatorUpdate", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, des.Adapter, new object[] { animator });
                }
                else
                {
                    Utility.LogError("FixedPlaylist: can not callback your adapter caused by null method named 'OnAnimatorUpdate(void)'");
                }
            }
        }
        private void OnOneAnimatorEnd(GTAnimator animator)
        {
            if (animator != null)
            {
                animator.PreBeginDelegate -= OnOneAnimatorPreStart;
                animator.BeginDelegate -= OnOneAnimatorStart;
                animator.UpdateDelegate -= OnOneAnimatorUpdate;
                animator.EndDelegate -= OnOneAnimatorEnd;
            }
            if (Adapter != null)
            {
                MethodInfo info = Adapter.GetType().GetMethod("OnAnimatorEnd");
                if (info != null)
                {
                    Adapter.GetType().InvokeMember("OnAnimatorEnd", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, Adapter, new object[] { animator });
                }
                else
                {
                    Utility.LogError("FixedPlaylist: can not callback your adapter caused by null method named 'OnAnimatorEnd(void)'");
                }
            }
            Description des = GetDescription(animator);
            if (des == null)
            {
                return;
            }
            ActiveAnimators.Remove(des);
            if (des.Adapter != null)
            {
                MethodInfo info = des.Adapter.GetType().GetMethod("OnAnimatorEnd");
                if (info != null)
                {
                    des.Adapter.GetType().InvokeMember("OnAnimatorEnd", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, des.Adapter, new object[] { animator });
                }
                else
                {
                    Utility.LogError("FixedPlaylist: can not callback your adapter caused by null method named 'OnAnimatorEnd(void)'");
                }
                if (des.CallbackOnEnd)
                {
                    if (ListenEnd != null)
                    {
                        ListenEnd(this);
                    }
                }
            }
            if (WaittingProcessAnimators != null && WaittingProcessAnimators.Count > 0)
            {
                for (int i = 0; i < WaittingProcessAnimators.Count; i++)
                {
                    Description tempDes = WaittingProcessAnimators[i];
                    if (tempDes == null || tempDes.Animator == null) continue;
                    if (tempDes.RelyOn == animator && tempDes.Time == -1 && tempDes.Animator.m_Status == GTAnimator.Status.UnAction)
                    {
                        GTAnimator.PlayTask task = new GTAnimator.PlayTask();
                        task.PlayAllClip = true;
                        task.PlayMode = GTAnimator.PlayMode.DontPlayWhenBusying;

                        tempDes.Animator.PreBeginDelegate += OnOneAnimatorPreStart;
                        tempDes.Animator.BeginDelegate += OnOneAnimatorStart;
                        tempDes.Animator.UpdateDelegate += OnOneAnimatorUpdate;
                        tempDes.Animator.EndDelegate += OnOneAnimatorEnd;
                        tempDes.Animator.Play(task);
                        WaittingProcessAnimators.Remove(tempDes);
                        ActiveAnimators.Add(tempDes);
                    }
                }
            }
        }
        public Description GetDescription(GTAnimator animator)
        {
            if (animator == null) return null;
            if (Playlist == null || Playlist.Count == 0) return null;
            for (int i = 0; i < Playlist.Count; i++)
            {
                Description des = Playlist[i];
                if (des == null) continue;
                if (des.Animator == animator) return des;
            }
            return null;
        }
        private void PlayEnd()
        {
            CurrentTime = 0.0f;
            IsPlaying = false;
            if (ListenEnd != null)
            {
                ListenEnd(this);
            }
        }
        public void Stop()
        {
            if (WaittingProcessAnimators.Count > 0)
            {
                for (int i = 0; i < WaittingProcessAnimators.Count; i++)
                {
                    Description des = WaittingProcessAnimators[i];
                    if (des == null || des.Animator == null) continue;
                    des.Animator.Stop();
                }
                WaittingProcessAnimators.Clear();
            }
            if (ActiveAnimators.Count > 0)
            {
                for (int i = 0; i < ActiveAnimators.Count; i++)
                {
                    Description des = ActiveAnimators[i];
                    if (des == null || des.Animator == null) continue;
                    des.Animator.Stop();
                }
                ActiveAnimators.Clear();
            }
            PlayEnd();
        }
    }
}
