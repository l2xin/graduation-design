/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTEditorComponent.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/28 13:57:16
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
    public class GTEditorComponent
    {
        public float Height { get; set; }
        public float Width { get; set; }
        public int Number { get; set; }
        public float MarginLeft { get; set; }

        public virtual void Draw(GTEditorSubView context)
        {

        }
    }

    public class GTEditorFileFieldComponent : GTEditorComponent
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
        public string[] FilterFolders { get; set; }
        public float FieldShowTypeWidth { get; set; }
        public float ObjectFieldWidth { get; set; }


        public List<string> AllCfgPath = null;
        public List<string> AllCfgName = null;
        public int AllCfgPathIndex;

        public string LastFilePath { get; set; }
        public string CurrentFilePath { get; set; }

        private UnityEngine.Object CurrentAsset;
        public GTEditorFileFieldComponent()
        {
            this.FieldShowType = ShowType.CurrentEdit;
            AllListeners = new List<FileChangeCallback>();
            FieldShowTypeWidth = 100;
            ObjectFieldWidth = 200;
        }

        public override void Draw(GTEditorSubView varView)
        {
            base.Draw(varView);
            GUILayout.BeginHorizontal(GUILayout.Width(FieldShowTypeWidth+ObjectFieldWidth+MarginLeft));
            GUILayout.Space(MarginLeft);
            if (Width <= 0) { return; }
            FieldShowType = (ShowType)EditorGUILayout.EnumPopup(FieldShowType, GUILayout.Width(FieldShowTypeWidth));
            LastFilePath = CurrentFilePath;
            switch (FieldShowType)
            {
                case ShowType.CurrentEdit:
                    EditorGUILayout.ObjectField(CurrentAsset, typeof(TextAsset), false, GUILayout.Width(ObjectFieldWidth));
                    break;
                case ShowType.FileNamePop:
                    ShowFileNamePop();
                    break;
                default:
                    break;
            }
            GUILayout.EndHorizontal();
        }
        public void SetCurrentAsset(string varPath)
        {
            if (string.IsNullOrEmpty(varPath)) return;
            CurrentAsset = AssetDatabase.LoadAssetAtPath(varPath, typeof(TextAsset)) as TextAsset;
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
                            for (int j = 0; j < FilterFolders.Length; j++)
                            {
                                string str = FilterFolders[j];
                                if (string.IsNullOrEmpty(str)) continue;
                                if (path.StartsWith(FolderPath + str))
                                {
                                    continue;
                                }
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
                AllCfgPathIndex = EditorGUILayout.Popup(AllCfgPathIndex, contents, GUILayout.Width(200));
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
}
#endif