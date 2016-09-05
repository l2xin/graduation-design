/*----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            // 
            // FileName: InspecGTAnimator.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/13 15:11:25
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GTools.Animator
{
    [CustomEditor(typeof(GTAnimator))]
    public class InspecGTAnimator : UnityEditor.Editor
    {
        public GTAnimator Instance;

        public int CurrentSelectedClipIndex;
        public List<string> AllClipNames = new List<string>();
        public List<Type> AllActionType = new List<Type>();
        public List<string> AllActionTypeName = new List<string>();
        public int GonnaAddActionIndex;
        public Type GonnaCreateActionType;
        public Clip CurrentSelectedClip;

        public int CurrentSelectedActionIndex;
        public List<string> AllActionNames = new List<string>();
        public ClipAction CurrentSelectedAction;
        public string GonnaCreateClipName;

        public bool BoolDisplayAllAction = true;

        public bool OpenClipHeader = true;

        public string GonnaCreateActionName;

        #region debug declaration
        public bool EnableQueue = true;
        public bool PlayAllClip = false;
        public bool PlayImmediately = true;
        public string SpecificName;
        public int CurrentSelectedDebugClipIndex;
        public Clip CurrentSelectedDebugClip;

        #endregion
        private void OnEnable()
        {
            Instance = target as GTAnimator;
        }
        public bool IsClipNameLegal(string name)
        {
            if (Instance == null) return false;
            if (string.IsNullOrEmpty(name)) return false;
            if (Instance.Clips == null || Instance.Clips.Count == 0) return true;
            for (int i = 0; i < Instance.Clips.Count; i++)
            {
                Clip clip = Instance.Clips[i];
                if (clip == null) continue;
                if (clip.Name == name) return false;
            }
            return true;
        }
        public bool IsActionNameLegal(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            if (CurrentSelectedClip == null) return false;
            if (CurrentSelectedClip.GetAllAction() == null || CurrentSelectedClip.GetAllAction().Count == 0) return true;
            for (int i = 0; i < CurrentSelectedClip.Actions.Count; i++)
            {
                ClipAction action = CurrentSelectedClip.Actions[i];
                if (action == null) continue;
                if (action.IdentifyName == name) return false;
            }
            return true;
        }
        public override void OnInspectorGUI()
        {
            try
            {
                if (Instance == null) return;
                base.OnInspectorGUI();

                #region Debug view
                if (Application.isPlaying)
                {
                    if (EditorUtility.DrawHeader("Debug", true))
                    {
                        EditorUtility.BeginContents();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("EnableQueue", GUILayout.Width(100));
                        EnableQueue = EditorGUILayout.Toggle(EnableQueue);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("PlayAllClip", GUILayout.Width(100));
                        PlayAllClip = EditorGUILayout.Toggle(PlayAllClip);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("PlayImmediately", GUILayout.Width(100));
                        PlayImmediately = EditorGUILayout.Toggle(PlayImmediately);
                        GUILayout.EndHorizontal();

                        #region animation clip pop view
                        GUILayout.Space(4.0f);
                        AllClipNames = new List<string>();
                        if (Instance.Clips != null && Instance.Clips.Count > 0)
                        {
                            for (int i = 0; i < Instance.Clips.Count; i++)
                            {
                                Clip clip = Instance.Clips[i];
                                if (clip == null)
                                {
                                    Instance.Clips.RemoveAt(i); i--; continue;
                                }
                                AllClipNames.Add("From " + clip.BeginTime + "s to " + clip.EndTime + "s || Name: " + clip.Name);
                            }
                        }
                        CurrentSelectedDebugClipIndex = EditorGUILayout.Popup(CurrentSelectedDebugClipIndex, AllClipNames.ToArray());
                        if (CurrentSelectedDebugClipIndex < Instance.Clips.Count && CurrentSelectedClipIndex >= 0)
                        {
                            CurrentSelectedDebugClip = Instance.Clips[CurrentSelectedDebugClipIndex];
                        }
                        else
                        {
                            CurrentSelectedDebugClip = null;
                        }
                        #endregion

                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button("Play"))
                        {
                            if (Application.isPlaying == false) return;
                            if (CurrentSelectedDebugClip == null) return;

                            GTAnimator.PlayTask task = new GTAnimator.PlayTask();
                            task.PlayAllClip = PlayAllClip;
                            if (PlayAllClip == false)
                            {
                                task.WannaPlayClips = new string[] { CurrentSelectedDebugClip.Name };
                            }
                            if (EnableQueue)
                            {
                                task.PlayMode = GTAnimator.PlayMode.Waitting;
                            }
                            else
                            {
                                task.PlayMode = GTAnimator.PlayMode.StopPrevious;
                            }
                            GTAnimator.Handler handler = new GTAnimator.Handler();
                            handler.Callback = HandlerCallback;
                            task.Handler = handler;
                            Instance.Play(task);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Pause"))
                        {
                            if (Application.isPlaying == false) return;
                            Instance.Pause();
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("UnPause"))
                        {
                            if (Application.isPlaying == false) return;
                            Instance.UnPause();
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Stop"))
                        {
                            if (Application.isPlaying == false) return;
                            Instance.Stop(true);
                        }
                        GUILayout.EndHorizontal();
                        EditorUtility.EndContents();
                    }
                }
                #endregion

                #region animator property view
                //GUILayout.Space(5.0f);
                //if (GTAnimatorEditorUtility.DrawHeader("Animator Property", true))
                //{
                //    GTAnimatorEditorUtility.BeginContents();

                //    GUILayout.BeginHorizontal();
                //    if (GUILayout.Button("Preview", GUILayout.Width(100))) { }
                //    if (GUILayout.Button("Clear", GUILayout.Width(100))) { }
                //    GUILayout.EndHorizontal();

                //    GTAnimatorEditorUtility.EndContents();
                //}
                #endregion

                #region animation clip view
                GUILayout.Space(5.0f);
                if (CurrentSelectedClip != null)
                {
                    OpenClipHeader = EditorUtility.DrawHeader("Clip - " + CurrentSelectedClip.Name + " || From " + CurrentSelectedClip.BeginTime + "s to " + CurrentSelectedClip.EndTime + "s", true);
                }
                else
                {
                    OpenClipHeader = EditorUtility.DrawHeader("Clip", true);
                }
                if (OpenClipHeader)
                {
                    #region animation clip operation
                    EditorUtility.BeginContents();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name: ");
                    GonnaCreateClipName = EditorGUILayout.TextField(GonnaCreateClipName);
                    if (GUILayout.Button("Add", GUILayout.Width(60), GUILayout.Height(15)))
                    {
                        if (Instance.Clips == null) Instance.Clips = new List<Clip>();
                        if (IsClipNameLegal(GonnaCreateClipName) == false)
                        {
                            Utility.LogError("clip name is illegal,please try another one"); return;
                        }
                        Instance.Clips.Add(new Clip() { Name = GonnaCreateClipName });
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(4.0f);
                    if (GUILayout.Button("Clear", GUILayout.Width(80)))
                    {
                        if (Instance.Clips != null)
                        {
                            Instance.Clips.Clear();
                        }
                    }
                    if (GUILayout.Button("Delete", GUILayout.Width(80)))
                    {
                        if (CurrentSelectedClip == null) return;
                        if (Instance.Clips == null) return;
                        Instance.Clips.Remove(CurrentSelectedClip);
                    }
                    if (GUILayout.Button("Clone", GUILayout.Width(80)))
                    {
                        if (Instance.Clips == null) Instance.Clips = new List<Clip>();
                        if (IsClipNameLegal(GonnaCreateClipName) == false)
                        {
                            Utility.LogError("clip name is illegal,please try another one"); return;
                        }
                        Clip clip = new Clip();
                        clip.Name = GonnaCreateClipName;
                        clip.Clone(CurrentSelectedClip);
                        Instance.Clips.Add(clip);
                    }
                    GUILayout.EndHorizontal();
                    #endregion

                    #region animation clip pop view
                    GUILayout.Space(4.0f);
                    AllClipNames = new List<string>();
                    if (Instance.Clips != null && Instance.Clips.Count > 0)
                    {
                        for (int i = 0; i < Instance.Clips.Count; i++)
                        {
                            Clip clip = Instance.Clips[i];
                            if (clip == null)
                            {
                                Instance.Clips.RemoveAt(i); i--; continue;
                            }
                            clip.ClampTime();
                            AllClipNames.Add("From " + clip.BeginTime + "s to " + clip.EndTime + "s || Name: " + clip.Name);
                        }
                    }
                    CurrentSelectedClipIndex = EditorGUILayout.Popup(CurrentSelectedClipIndex, AllClipNames.ToArray());
                    if (CurrentSelectedClipIndex < Instance.Clips.Count && CurrentSelectedClipIndex >= 0)
                    {
                        CurrentSelectedClip = Instance.Clips[CurrentSelectedClipIndex];
                    }
                    else
                    {
                        CurrentSelectedClip = null;
                    }
                    #endregion

                    #region display clip property
                    GUILayout.Space(4.0f);
                    if (CurrentSelectedClip != null)
                    {
                        CurrentSelectedClip.DiaplayEditorView();
                    }
                    #endregion

                    EditorUtility.EndContents();
                }
                #endregion

                #region animation clip actions view
                GUILayout.Space(5.0f);
                if (EditorUtility.DrawHeader("Action", true))
                {
                    if (CurrentSelectedClip == null)
                    {
                        EditorGUILayout.HelpBox("Please select an animation clip", MessageType.Warning);
                    }
                    else
                    {
                        #region header
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Clip: " + CurrentSelectedClip.Name);
                        GUILayout.Space(2.0f);
                        GUILayout.Label("Time from " + CurrentSelectedClip.BeginTime + "s to " + CurrentSelectedClip.EndTime + "s");
                        GUILayout.EndHorizontal();
                        #endregion

                        #region add action
                        EditorUtility.BeginContents();
                        GUILayout.Space(2.0f);
                        GUILayout.BeginHorizontal();
                        if (AllActionType == null || AllActionType.Count == 0 || AllActionTypeName == null || AllActionTypeName.Count == 0)
                        {
                            AllActionType = EditorUtility.GetAllSubClass(typeof(ClipAction), null);
                            if (AllActionType != null && AllActionType.Count > 0)
                            {
                                AllActionTypeName = new List<string>();
                                for (int i = 0; i < AllActionType.Count; i++)
                                {
                                    Type type = AllActionType[i];
                                    if (type == null) continue;
                                    string typeName = EditorUtility.GetTypeNameWithoutNamespcae(type.FullName);
                                    AllActionTypeName.Add(typeName);
                                }
                            }
                        }
                        if (AllActionTypeName != null && AllActionTypeName.Count > 0)
                        {
                            GonnaAddActionIndex = EditorGUILayout.Popup(GonnaAddActionIndex, AllActionTypeName.ToArray());
                            if (GonnaAddActionIndex >= 0 && GonnaAddActionIndex < AllActionTypeName.Count)
                            {
                                GonnaCreateActionType = AllActionType[GonnaAddActionIndex];
                            }
                        }
                        GUILayout.FlexibleSpace();
                        GonnaCreateActionName = EditorGUILayout.TextField(GonnaCreateActionName);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Add", GUILayout.Width(60), GUILayout.Height(15)))
                        {
                            if (CurrentSelectedClip == null) return;
                            if (GonnaCreateActionType == null) return;
                            if (IsActionNameLegal(GonnaCreateActionName) == false)
                            {
                                Utility.LogError("action identify name is illegal,please try another one"); return;
                            }
                            ClipAction action = CurrentSelectedClip.AddAction(GonnaCreateActionType);
                            if (action != null)
                            {
                                action.IdentifyName = GonnaCreateActionName;
                            }
                        }
                        GUILayout.EndHorizontal();
                        #endregion

                        #region operation of action collection

                        GUILayout.Space(3.0f);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Clear", GUILayout.Width(60)))
                        {
                            if (CurrentSelectedClip == null) return;
                            CurrentSelectedClip.ClearAction();
                        }
                        if (GUILayout.Button("Delete", GUILayout.Width(60)))
                        {
                            if (CurrentSelectedAction == null) return;
                            if (CurrentSelectedClip == null) return;
                            CurrentSelectedClip.RemoveAction(CurrentSelectedAction);
                        }
                        if (GUILayout.Button("Clone", GUILayout.Width(60))) { }
                        GUILayout.FlexibleSpace();
                        if (BoolDisplayAllAction)
                        {
                            if (GUILayout.Button("Display-Single"))
                            {
                                BoolDisplayAllAction = !BoolDisplayAllAction;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("    Display-All  "))
                            {
                                BoolDisplayAllAction = !BoolDisplayAllAction;
                            }
                        }
                        GUILayout.EndHorizontal();
                        EditorUtility.EndContents();
                        #endregion

                        #region display view
                        EditorUtility.BeginContents();
                        GUILayout.Space(6.0f);
                        if (BoolDisplayAllAction == true)
                        {
                            #region all view
                            if (CurrentSelectedClip == null || CurrentSelectedClip.GetAllAction() == null || CurrentSelectedClip.GetAllAction().Count == null)
                            {
                                return;
                            }
                            for (int i = 0; i < CurrentSelectedClip.GetAllAction().Count; i++)
                            {
                                ClipAction action = CurrentSelectedClip.GetAllAction()[i];
                                if (action == null) continue;
                                EditorUtility.BeginContents();
                                GUILayout.Space(2.0f);
                                action.DisplayEditorView();
                                GUILayout.Space(2.0f);
                                EditorUtility.EndContents();
                            }
                            #endregion
                        }
                        else
                        {
                            #region single view
                            AllActionNames = new List<string>();
                            if (CurrentSelectedClip.GetAllAction() != null && CurrentSelectedClip.GetAllAction().Count > 0)
                            {
                                for (int i = 0; i < CurrentSelectedClip.GetAllAction().Count; i++)
                                {
                                    ClipAction action = CurrentSelectedClip.GetAllAction()[i];
                                    if (action == null)
                                    {
                                        continue;
                                    }
                                    AllActionNames.Add("From " + action.BeginTime + " to " + action.EndTime + " || " + action.DisplayName);
                                }
                            }
                            CurrentSelectedActionIndex = EditorGUILayout.Popup(CurrentSelectedActionIndex, AllActionNames.ToArray());
                            if (CurrentSelectedActionIndex >= 0 && CurrentSelectedActionIndex < CurrentSelectedClip.GetAllAction().Count)
                            {
                                CurrentSelectedAction = CurrentSelectedClip.GetAllAction()[CurrentSelectedActionIndex];
                            }
                            #endregion

                            #region detail property
                            GUILayout.Space(6.0f);
                            if (CurrentSelectedAction != null)
                            {
                                CurrentSelectedAction.DisplayEditorView();
                            }
                            #endregion
                        }
                        EditorUtility.EndContents();
                        #endregion
                    }
                }
                #endregion
                GUILayout.Space(5.0f);
            }
            catch { }
        }

        private void HandlerCallback(GTAnimator.Handler.MsgType type, object[] args)
        {
            switch (type)
            {
                case GTAnimator.Handler.MsgType.OnAnimatorCannotPlay:
                    //Debug.LogError("OnAnimatorCannotPlay");
                    break;
                case GTAnimator.Handler.MsgType.OnAnimatorPreBegin:
                    //Debug.LogError("OnAnimatorPreBegin");
                    break;
                case GTAnimator.Handler.MsgType.OnAnimatorBegin:
                    //Debug.LogError("OnAnimatorBegin");
                    break;
                case GTAnimator.Handler.MsgType.OnAnimatorUpdate:
                    //Debug.LogError("OnAnimatorUpdate");
                    break;
                case GTAnimator.Handler.MsgType.OnAnimatorUpdatePercentage:
                    //Debug.LogError("OnAnimatorUpdatePercentage  " + args[2].ToString());
                    break;
                case GTAnimator.Handler.MsgType.OnAnimatorEnd:
                    //Debug.LogError("OnAnimatorEnd");
                    break;
                case GTAnimator.Handler.MsgType.OnAnimatorPause:
                    //Debug.LogError("OnAnimatorPause current time: " + args[1]);
                    break;
                case GTAnimator.Handler.MsgType.OnAnimatorUnPause:
                    //Debug.LogError("OnAnimatorUnPause curretn time: " + args[1] + " ,paused time: " + args[2]);
                    break;
                case GTAnimator.Handler.MsgType.OnAnimatorStop:
                    //Debug.LogError("OnAnimatorStop");
                    break;
                default:
                    break;
            }
        }
    }
}
