/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTEditorToolbar.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/26 21:20:02
            // Modify History:
            //
//----------------------------------------------------------------*/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Air2000
{
    public enum GTEditorToolbarButtonType
    {
        AutoGen,
        UnLoad,
        Save,
        SaveAs,
        ChangeFileReviewMode,
        ApplyTo,
    }
    public enum GTEditorToolbarFileFieldCreateType
    {
        OnlyObjectField,
        OnlyFileNamePop,
        All,
    }
    public class GTEditorToolbarViewComponent
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float MarginLeft { get; set; }
        public int Number { get; set; }

        public virtual void Draw(GTEditorToolbar varView) { }
    }

    public class GTEditorToolbarLable : GTEditorToolbarViewComponent
    {
        public string Text { get; set; }
        public override void Draw(GTEditorToolbar varView)
        {
            base.Draw(varView);
            if (string.IsNullOrEmpty(Text) || Width <= 0) { return; }
            GUILayout.Space(MarginLeft);
            EditorGUILayout.LabelField(Text, GUILayout.Width(Width));
        }
    }
    //public class GTEditorToolbarInputField<T> : GTEditorToolbarViewComponent
    //{
    //    public T value { get; set; }
    //    public override void Draw(GTEditorToolbar varView)
    //    {
    //        base.Draw(varView);
    //        if (string.IsNullOrEmpty(Text) || Width <= 0) { return; }
    //        GUILayout.Space(MarginLeft);
    //        EditorGUILayout.LabelField(Text, GUILayout.Width(Width));
    //    }
    //}

    public class GTEditorToolbarObjectField<T> : GTEditorToolbarViewComponent
    {
        public delegate void ToolbarObjectChangeCallback(UnityEngine.Object obj);
        private List<ToolbarObjectChangeCallback> AllListeners;
        public string Text { get; set; }
        public float TextStrWidth { get; set; }
        public UnityEngine.Object CurrentObj { get; set; }
        public UnityEngine.Object LastObj { get; set; }

        public bool ReadOnly { get; set; }
        public UnityEngine.Object DefaultObj { get; set; }
        public override void Draw(GTEditorToolbar varView)
        {
            base.Draw(varView);
            GUILayout.Space(MarginLeft);
            if (string.IsNullOrEmpty(Text) == false && TextStrWidth > 0)
            {
                EditorGUILayout.LabelField(Text, GUILayout.Width(TextStrWidth));
            }
            if (Width <= 0) return;
            LastObj = CurrentObj;
            if (Height > 0)
            {
                if (ReadOnly)
                {
                    EditorGUILayout.ObjectField(DefaultObj, typeof(T), GUILayout.Width(Width), GUILayout.Height(Height));
                    NotifyListener(DefaultObj); return;
                }
                CurrentObj = EditorGUILayout.ObjectField(CurrentObj, typeof(T), GUILayout.Width(Width), GUILayout.Height(Height));
                if (LastObj != CurrentObj)
                {
                    NotifyListener(CurrentObj);
                }
            }
            else
            {
                if (ReadOnly)
                {
                    EditorGUILayout.ObjectField(DefaultObj, typeof(T), GUILayout.Width(Width));
                    NotifyListener(DefaultObj); return;
                }
                CurrentObj = EditorGUILayout.ObjectField(CurrentObj, typeof(T), GUILayout.Width(Width));
                if (LastObj != CurrentObj)
                {
                    NotifyListener(CurrentObj);
                }
            }
        }

        public void SetOnObjectChangeListen(ToolbarObjectChangeCallback varFunc)
        {
            if (varFunc == null) { return; }
            if (AllListeners == null)
            {

                AllListeners = new List<ToolbarObjectChangeCallback>();
            }
            if (AllListeners.Contains(varFunc))
            {
                return;
            }
            AllListeners.Add(varFunc);
        }
        public void RemoveFileChangeListen(ToolbarObjectChangeCallback varFunc)
        {
            if (varFunc == null) { return; }
            if (AllListeners == null || AllListeners.Count == 0) return;
            AllListeners.Remove(varFunc);
        }
        public void RemoveAllFileChangeListen()
        {
            if (AllListeners == null || AllListeners.Count == 0) return;
            AllListeners.Clear();
        }
        public void NotifyListener(UnityEngine.Object obj)
        {
            if (AllListeners == null || AllListeners.Count == 0) { return; }
            for (int i = 0; i < AllListeners.Count; i++)
            {
                ToolbarObjectChangeCallback callback = AllListeners[i];
                if (callback == null)
                {
                    continue;
                }
                callback(obj);
            }
        }
    }
    public class GTEditorToolbarFileField : GTEditorToolbarViewComponent
    {
        public enum ShowType
        {
            CurrentEdit,
            FileNamePop,
        }
        public delegate void FileChangeCallback(string filePath);
        private List<FileChangeCallback> AllListeners;
        public string FolderPath { get; set; }
        public Type Type { get; set; }
        public ShowType FieldShowType { get; set; }


        public List<string> AllCfgPath = null;
        public List<string> AllCfgName = null;
        public int AllCfgPathIndex;
        public string[] FilterFolders { get; set; }
        public string LastFilePath { get; set; }
        public string CurrentFilePath { get; set; }
        public string CompareFilePath { get; set; }
        private UnityEngine.Object CurrentAsset;
        public GTEditorToolbarFileField()
        {
            this.FieldShowType = ShowType.CurrentEdit;
            AllListeners = new List<FileChangeCallback>();
        }

        public void SetCurrentAsset(string varPath)
        {
            if (string.IsNullOrEmpty(varPath)) return;
            CurrentAsset = AssetDatabase.LoadAssetAtPath(varPath, typeof(TextAsset)) as TextAsset;
        }

        public override void Draw(GTEditorToolbar varView)
        {
            base.Draw(varView);
            GUILayout.Space(MarginLeft);
            if (Width <= 0) { return; }
            FieldShowType = (ShowType)EditorGUILayout.EnumPopup(FieldShowType, GUILayout.Width(100));
            LastFilePath = CurrentFilePath;
            switch (FieldShowType)
            {
                case ShowType.CurrentEdit:
                    //if (string.IsNullOrEmpty(CompareFilePath) == false)
                    //{
                    //    CurrentAsset = AssetDatabase.LoadAssetAtPath(CompareFilePath, typeof(TextAsset)) as TextAsset;
                    //}
                    EditorGUILayout.ObjectField(CurrentAsset, typeof(TextAsset), false, GUILayout.Width(200));
                    //if (CurrentAsset != null)
                    //{
                    //    CurrentFilePath = AssetDatabase.GetAssetPath(CurrentAsset.GetInstanceID());
                    //    if (LastFilePath != CurrentFilePath)
                    //    {
                    //        NotifyListener(CurrentFilePath);
                    //    }
                    //}
                    break;
                case ShowType.FileNamePop:
                    ShowFileNamePop();
                    break;
                default:
                    break;
            }
        }

        public void ShowFileNamePop()
        {
            if (string.IsNullOrEmpty(FolderPath))
            {
                FieldShowType = ShowType.CurrentEdit;
                return;
            }
            if (AllCfgPath == null)
            {
                AllCfgPath = new List<string>();
            }
            if (AllCfgName == null)
            {
                AllCfgName = new List<string>();
            }
            AllCfgPath.Clear();
            AllCfgName.Clear();
            string[] allAssetPath = AssetDatabase.GetAllAssetPaths();
            if (allAssetPath != null && allAssetPath.Length > 0)
            {
                for (int i = 0; i < allAssetPath.Length; i++)
                {
                    string path = allAssetPath[i];
                    if (string.IsNullOrEmpty(path))
                    {
                        continue;
                    }
                    if (path.StartsWith(FolderPath) && path.EndsWith(".bytes"))
                    {
                        if (FilterFolders != null && FilterFolders.Length > 0)
                        {
                            bool add = true;
                            for (int j = 0; j < FilterFolders.Length; j++)
                            {
                                string str = FilterFolders[j];
                                if (string.IsNullOrEmpty(str)) continue;
                                if (path.StartsWith(FolderPath + str))
                                {
                                    add = false;
                                    break;
                                }
                            }
                            if (add)
                            {
                                AllCfgPath.Add(path);
                                AllCfgName.Add(Helper.GetFileNameFromFullPath(path));
                            }
                        }
                        else
                        {
                            AllCfgPath.Add(path);
                            AllCfgName.Add(Helper.GetFileNameFromFullPath(path));
                        }
                    }
                }
            }
            if (AllCfgName != null && AllCfgName.Count > 0)
            {
                string[] contents = new string[AllCfgName.Count];
                for (int i = 0; i < AllCfgName.Count; i++)
                {
                    contents[i] = AllCfgName[i];
                }
                //int compareIndex = -1;
                //if (string.IsNullOrEmpty(CompareFilePath) == false)
                //{
                //    for (int i = 0; i < AllCfgPath.Count; i++)
                //    {
                //        string str = AllCfgPath[i];
                //        if (string.IsNullOrEmpty(str)) continue;
                //        if (str == CompareFilePath)
                //        {
                //            compareIndex = i;
                //            break;
                //        }
                //    }
                //}
                AllCfgPathIndex = EditorGUILayout.Popup(AllCfgPathIndex, contents, GUILayout.Width(200));
                //if (compareIndex >= 0 && compareIndex < AllCfgName.Count)
                //{
                //    AllCfgPathIndex = compareIndex;
                //}
                if (AllCfgPathIndex <= AllCfgPath.Count)
                {
                    CurrentFilePath = AllCfgPath[AllCfgPathIndex];
                    if (CurrentFilePath != LastFilePath)
                    {
                        NotifyListener(CurrentFilePath);
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("No file", "There no config in " + FolderPath), GUILayout.Width(200));
            }
        }
        public void SetFileChangeListen(FileChangeCallback varFunc)
        {
            if (varFunc == null) { return; }
            if (AllListeners == null)
            {

                AllListeners = new List<FileChangeCallback>();
            }
            if (AllListeners.Contains(varFunc))
            {
                return;
            }
            AllListeners.Add(varFunc);
        }
        public void RemoveFileChangeListen(FileChangeCallback varFunc)
        {
            if (varFunc == null) { return; }
            if (AllListeners == null || AllListeners.Count == 0) return;
            AllListeners.Remove(varFunc);
        }
        public void RemoveAllFileChangeListen()
        {
            if (AllListeners == null || AllListeners.Count == 0) return;
            AllListeners.Clear();
        }
        public void NotifyListener(string filePath)
        {
            if (AllListeners == null || AllListeners.Count == 0) { return; }
            for (int i = 0; i < AllListeners.Count; i++)
            {
                FileChangeCallback callback = AllListeners[i];
                if (callback == null)
                {
                    continue;
                }
                callback(filePath);
            }
        }
    }
    public class GTEditorToolbarButton : GTEditorToolbarViewComponent
    {
        public delegate void ToolbarButtonClickCallback();
        private List<ToolbarButtonClickCallback> AllListeners;
        public string Text { get; set; }
        public string Tips { get; set; }
        public GTEditorToolbarButtonType ButtonType { get; set; }
        public override void Draw(GTEditorToolbar varView)
        {
            base.Draw(varView);
            GUILayout.Space(MarginLeft);
            if (string.IsNullOrEmpty(Text) || Width <= 0) { return; }
            if (Height > 0)
            {
                if (GUILayout.Button(new GUIContent(Text, Tips), GUILayout.Width(Width), GUILayout.Height(Height)))
                {
                    NotifyListener();
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent(Text, Tips), GUILayout.Width(Width)))
                {
                    NotifyListener();
                }
            }
        }

        public void SetOnClickListen(ToolbarButtonClickCallback varFunc)
        {
            if (varFunc == null) { return; }
            if (AllListeners == null)
            {

                AllListeners = new List<ToolbarButtonClickCallback>();
            }
            if (AllListeners.Contains(varFunc))
            {
                return;
            }
            AllListeners.Add(varFunc);
        }
        public void RemoveFileChangeListen(ToolbarButtonClickCallback varFunc)
        {
            if (varFunc == null) { return; }
            if (AllListeners == null || AllListeners.Count == 0) return;
            AllListeners.Remove(varFunc);
        }
        public void RemoveAllFileChangeListen()
        {
            if (AllListeners == null || AllListeners.Count == 0) return;
            AllListeners.Clear();
        }
        public void NotifyListener()
        {
            if (AllListeners == null || AllListeners.Count == 0) { return; }
            for (int i = 0; i < AllListeners.Count; i++)
            {
                ToolbarButtonClickCallback callback = AllListeners[i];
                if (callback == null)
                {
                    continue;
                }
                callback();
            }
        }
    }
    public class GTEditorToolbar
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float MarginTop { get; set; }
        public float MarginLeft { get; set; }
        public string Name { get; set; }
        public float NameStrWidth { get; set; }
        public List<GTEditorToolbarViewComponent> Components { get; set; }
        public GTEditorToolbar()
        {
            Components = new List<GTEditorToolbarViewComponent>();
        }
        public void DrawView(GTEditorSubView context)
        {
            if (Components == null || Components.Count == 0)
            {
                return;
            }
            if (Width <= 0)
            {
                return;
            }
            if (Height <= 0)
            {
                GUILayout.BeginHorizontal("box", GUILayout.Width(Width));
            }
            else
            {
                GUILayout.BeginHorizontal("box", GUILayout.Width(Width), GUILayout.Height(Height));
            }
            for (int i = 0; i < Components.Count; i++)
            {
                GTEditorToolbarViewComponent com = Components[i];
                if (com == null)
                {
                    continue;
                }
                com.Draw(this);
            }
            GUILayout.EndHorizontal();
        }

        public GTEditorToolbarViewComponent GetComponentByNumber<T>(int number)
        {
            if (Components == null || Components.Count == 0) return null;
            for (int i = 0; i < Components.Count; i++)
            {
                GTEditorToolbarViewComponent com = Components[i];
                if (com == null) return null;
                if (com.GetType() == typeof(T) && com.Number == number)
                {
                    return com;
                }
            }
            return null;
        }
    }
}
#endif