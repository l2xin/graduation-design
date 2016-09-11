/*----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            // 
            // FileName: InspecMotionMachine.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/15 21:39:49
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Air2000.Character
{
    [CustomEditor(typeof(MotionMachine))]
    public class InspecMotionMachine : UnityEditor.Editor
    {
        public MotionMachine Instance;
        public int CurrentSelectedClipIndex;
        public List<string> AllMotionNames = new List<string>();
        public List<Type> AllPluginType = new List<Type>();
        public List<string> AllPluginTypeName = new List<string>();
        public int GonnaAddPluginIndex;
        public Type GonnaCreatePluginType;
        public Motion CurrentSelectedMotion;

        public int CurrentSelectedPluginIndex;
        public List<string> AllPluginNames = new List<string>();
        public MotionPlugin CurrentSelectedPlugin;
        public RoleMotionType GonnaCreateMotionType;

        public bool BoolDisplayAllPlugin = true;

        public bool OpenMotionHeader = true;

        public string GonnaCreateActionName;

        #region debug declaration
        public bool EnableQueue = true;
        public bool PlayAllClip = false;
        public bool PlayImmediately = true;
        public string SpecificName;
        public int CurrentSelectedDebugClipIndex;
        public Motion CurrentSelectedDebugClip;

        #endregion
        private void OnEnable()
        {
            Instance = target as MotionMachine;
        }
        public bool IsMotionNameLegal(RoleMotionType type)
        {
            if (Instance == null) return false;
            Motion motion = Instance.GetMotion(type);
            if (motion == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsPluginNameLegal(string name)
        {
            //if (string.IsNullOrEmpty(name)) return false;
            //if (CurrentSelectedClip == null) return false;
            //if (CurrentSelectedClip.GetAllAction() == null || CurrentSelectedClip.GetAllAction().Count == 0) return true;
            //for (int i = 0; i < CurrentSelectedClip.Actions.Count; i++)
            //{
            //    ClipAction action = CurrentSelectedClip.Actions[i];
            //    if (action == null) continue;
            //    if (action.IdentifyName == name) return false;
            //}
            return true;
        }
        public override void OnInspectorGUI()
        {
            try
            {
                if (Instance == null) return;
                base.OnInspectorGUI();

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
                if (CurrentSelectedMotion != null)
                {
                    OpenMotionHeader = EditorUtility.DrawHeader("Motion: " + CurrentSelectedMotion.Type.ToString() + "  ||  Clip:  " + CurrentSelectedMotion.ClipName, true);
                }
                else
                {
                    OpenMotionHeader = EditorUtility.DrawHeader("Motion", true);
                }
                if (OpenMotionHeader)
                {
                    #region animation clip operation
                    EditorUtility.BeginContents();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name: ");
                    GonnaCreateMotionType = (RoleMotionType)EditorGUILayout.EnumPopup(GonnaCreateMotionType);
                    if (GUILayout.Button("Add", GUILayout.Width(60), GUILayout.Height(15)))
                    {
                        if (Instance.Motions == null) Instance.Motions = new List<Motion>();
                        if (IsMotionNameLegal(GonnaCreateMotionType) == false)
                        {
                            Utility.LogError("motion name is illegal,please try another one"); return;
                        }
                        Instance.Motions.Add(new Motion() { Type = GonnaCreateMotionType });
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(4.0f);
                    if (GUILayout.Button("Clear", GUILayout.Width(80)))
                    {
                        if (Instance.Motions != null)
                        {
                            Instance.Motions.Clear();
                        }
                    }
                    if (GUILayout.Button("Delete", GUILayout.Width(80)))
                    {
                        if (CurrentSelectedMotion == null) return;
                        if (Instance.Motions == null) return;
                        Instance.Motions.Remove(CurrentSelectedMotion);
                    }
                    if (GUILayout.Button("Clone", GUILayout.Width(80)))
                    {
                        if (Instance.Motions == null) Instance.Motions = new List<Motion>();
                        if (IsMotionNameLegal(GonnaCreateMotionType) == false)
                        {
                            Utility.LogError("motion name is illegal,please try another one"); return;
                        }
                        Motion motion = new Motion();
                        motion.Type = GonnaCreateMotionType;
                        motion.Clone(CurrentSelectedMotion);
                        Instance.Motions.Add(motion);
                    }
                    GUILayout.EndHorizontal();
                    #endregion

                    #region animation clip pop view
                    GUILayout.Space(4.0f);
                    AllMotionNames = new List<string>();
                    if (Instance.Motions != null && Instance.Motions.Count > 0)
                    {
                        for (int i = 0; i < Instance.Motions.Count; i++)
                        {
                            Motion motion = Instance.Motions[i];
                            if (motion == null)
                            {
                                Instance.Motions.RemoveAt(i); i--; continue;
                            }
                            AllMotionNames.Add("Motion: " + motion.Type.ToString() + "  ||  Clip:  " + motion.ClipName);
                        }
                    }
                    CurrentSelectedClipIndex = EditorGUILayout.Popup(CurrentSelectedClipIndex, AllMotionNames.ToArray());
                    if (CurrentSelectedClipIndex < Instance.Motions.Count && CurrentSelectedClipIndex >= 0)
                    {
                        CurrentSelectedMotion = Instance.Motions[CurrentSelectedClipIndex];
                    }
                    else
                    {
                        CurrentSelectedMotion = null;
                    }
                    #endregion

                    #region display clip property
                    GUILayout.Space(4.0f);
                    if (CurrentSelectedMotion != null)
                    {
                        CurrentSelectedMotion.DiaplayEditorView(Instance);
                    }
                    #endregion

                    EditorUtility.EndContents();
                }
                #endregion

                #region animation clip actions view
                GUILayout.Space(5.0f);
                if (EditorUtility.DrawHeader("Plugin", true))
                {
                    if (CurrentSelectedMotion == null)
                    {
                        EditorGUILayout.HelpBox("Please select an motion", MessageType.Warning);
                    }
                    else
                    {
                        #region header
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Motion: " + CurrentSelectedMotion.Type.ToString());
                        //GUILayout.Space(2.0f);
                        //GUILayout.Label("Time from " + CurrentSelectedMotion.BeginTime + "s to " + CurrentSelectedMotion.EndTime + "s");
                        GUILayout.EndHorizontal();
                        #endregion

                        #region add action
                        EditorUtility.BeginContents();
                        GUILayout.Space(2.0f);
                        GUILayout.BeginHorizontal();
                        if (AllPluginType == null || AllPluginType.Count == 0 || AllPluginTypeName == null || AllPluginTypeName.Count == 0)
                        {
                            AllPluginType = EditorUtility.GetAllSubClass(typeof(MotionPlugin), null);
                            if (AllPluginType != null && AllPluginType.Count > 0)
                            {
                                AllPluginTypeName = new List<string>();
                                for (int i = 0; i < AllPluginType.Count; i++)
                                {
                                    Type type = AllPluginType[i];
                                    if (type == null) continue;
                                    string typeName = EditorUtility.GetTypeNameWithoutNamespcae(type.FullName);
                                    AllPluginTypeName.Add(typeName);
                                }
                            }
                        }
                        if (AllPluginTypeName != null && AllPluginTypeName.Count > 0)
                        {
                            GonnaAddPluginIndex = EditorGUILayout.Popup(GonnaAddPluginIndex, AllPluginTypeName.ToArray());
                            if (GonnaAddPluginIndex >= 0 && GonnaAddPluginIndex < AllPluginTypeName.Count)
                            {
                                GonnaCreatePluginType = AllPluginType[GonnaAddPluginIndex];
                            }
                        }
                        GUILayout.FlexibleSpace();
                        GonnaCreateActionName = EditorGUILayout.TextField(GonnaCreateActionName);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Add", GUILayout.Width(60), GUILayout.Height(15)))
                        {
                            if (CurrentSelectedMotion == null) return;
                            if (GonnaCreatePluginType == null) return;
                            if (IsPluginNameLegal(GonnaCreateActionName) == false)
                            {
                                Utility.LogError("plugin identify name is illegal,please try another one"); return;
                            }
                            MotionPlugin plugin = CurrentSelectedMotion.AddPlugin(GonnaCreatePluginType);
                            if (plugin != null)
                            {
                                plugin.IdentifyName = GonnaCreateActionName;
                            }
                        }
                        GUILayout.EndHorizontal();
                        #endregion

                        #region operation of action collection

                        GUILayout.Space(3.0f);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Clear", GUILayout.Width(60)))
                        {
                            if (CurrentSelectedMotion == null) return;
                            CurrentSelectedMotion.ClearPlugin();
                        }
                        if (GUILayout.Button("Delete", GUILayout.Width(60)))
                        {
                            if (CurrentSelectedPlugin == null) return;
                            if (CurrentSelectedMotion == null) return;
                            CurrentSelectedMotion.RemovePlugin(CurrentSelectedPlugin);
                        }
                        if (GUILayout.Button("Clone", GUILayout.Width(60))) { }
                        GUILayout.FlexibleSpace();
                        if (BoolDisplayAllPlugin)
                        {
                            if (GUILayout.Button("Display-Single"))
                            {
                                BoolDisplayAllPlugin = !BoolDisplayAllPlugin;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("    Display-All  "))
                            {
                                BoolDisplayAllPlugin = !BoolDisplayAllPlugin;
                            }
                        }
                        GUILayout.EndHorizontal();
                        EditorUtility.EndContents();
                        #endregion

                        #region display view
                        EditorUtility.BeginContents();
                        GUILayout.Space(6.0f);
                        if (BoolDisplayAllPlugin == true)
                        {
                            #region all view
                            if (CurrentSelectedMotion == null || CurrentSelectedMotion.GetAllPlugin() == null || CurrentSelectedMotion.GetAllPlugin().Count == null)
                            {
                                return;
                            }
                            for (int i = 0; i < CurrentSelectedMotion.GetAllPlugin().Count; i++)
                            {
                                MotionPlugin plugin = CurrentSelectedMotion.GetAllPlugin()[i];
                                if (plugin == null) continue;
                                EditorUtility.BeginContents();
                                GUILayout.Space(2.0f);
                                plugin.DisplayEditorView(CurrentSelectedMotion);
                                GUILayout.Space(2.0f);
                                EditorUtility.EndContents();
                            }
                            #endregion
                        }
                        else
                        {
                            #region single view
                            AllPluginNames = new List<string>();
                            if (CurrentSelectedMotion.GetAllPlugin() != null && CurrentSelectedMotion.GetAllPlugin().Count > 0)
                            {
                                for (int i = 0; i < CurrentSelectedMotion.GetAllPlugin().Count; i++)
                                {
                                    MotionPlugin plugin = CurrentSelectedMotion.GetAllPlugin()[i];
                                    if (plugin == null)
                                    {
                                        continue;
                                    }
                                    AllPluginNames.Add("From " + plugin.BeginTime + " to " + plugin.EndTime + " || " + plugin.DisplayName);
                                }
                            }
                            CurrentSelectedPluginIndex = EditorGUILayout.Popup(CurrentSelectedPluginIndex, AllPluginNames.ToArray());
                            if (CurrentSelectedPluginIndex >= 0 && CurrentSelectedPluginIndex < CurrentSelectedMotion.GetAllPlugin().Count)
                            {
                                CurrentSelectedPlugin = CurrentSelectedMotion.GetAllPlugin()[CurrentSelectedPluginIndex];
                            }
                            #endregion

                            #region detail property
                            GUILayout.Space(6.0f);
                            if (CurrentSelectedPlugin != null)
                            {
                                CurrentSelectedPlugin.DisplayEditorView(CurrentSelectedMotion);
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
    }
}
