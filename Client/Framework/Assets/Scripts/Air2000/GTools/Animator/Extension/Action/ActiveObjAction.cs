/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ActiveObjAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/15 16:54:44
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace GTools.Animator
{
    [Serializable]
    public class ActiveObjAction : ClipAction
    {
        public GameObjectActiveOperation ActiveObjOperation = GameObjectActiveOperation.OnlyActiveOnBegin;
        public GameObject OperationObj;
        public override void OnBegin(Clip clip)
        {
            base.OnBegin(clip);
            if (OperationObj == null)
            {
                m_Status = Status.Error;
                return;
            }
            switch (ActiveObjOperation)
            {
                case GameObjectActiveOperation.DontOperate:
                    break;
                case GameObjectActiveOperation.AlwaysChangeStatus:
                    if (OperationObj.activeSelf == false)
                    {
                        OperationObj.SetActive(true);
                    }
                    break;
                case GameObjectActiveOperation.OnlyActiveOnBegin:
                    if (OperationObj.activeSelf == false)
                    {
                        OperationObj.SetActive(true);
                    }
                    break;
                default:
                    break;
            }
        }
        public override void OnEnd(Clip clip)
        {
            base.OnEnd(clip);
            if (OperationObj != null)
            {
                switch (ActiveObjOperation)
                {
                    case GameObjectActiveOperation.DontOperate:
                        break;
                    case GameObjectActiveOperation.AlwaysChangeStatus:
                        if (OperationObj.activeSelf == true)
                        {
                            OperationObj.SetActive(false);
                        }
                        break;
                    case GameObjectActiveOperation.OnlyActiveOnBegin: break;
                    default:
                        break;
                }
            }
        }

        public override void Clone(ClipAction obj)
        {
            base.Clone(obj);
            ActiveObjAction action = obj as ActiveObjAction;
            if (action != null)
            {
                ActiveObjOperation = action.ActiveObjOperation;
                OperationObj = action.OperationObj;
            }
        }

#if UNITY_EDITOR
        public override void DisplayEditorView()
        {
            base.DisplayEditorView();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("OperationObj", GUILayout.Width(100));
            OperationObj = (GameObject)EditorGUILayout.ObjectField(OperationObj, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Operation", GUILayout.Width(100));
            ActiveObjOperation = (GameObjectActiveOperation)EditorGUILayout.EnumPopup(ActiveObjOperation);
            GUILayout.EndHorizontal();
        }
#endif
    }
}
