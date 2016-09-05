/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: PlaySoundAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/7/11 14:10:48
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GTools.Animator
{
    public enum SoundType
    {
        UI,
        Role,
        BGM,
    }
    [Serializable]
    public class PlaySoundAction : ClipAction
    {
        public Air2000.UISoundEnum UISound;
        public Air2000.RoleSoundEnum RoleSound;
        public SoundType SoundType = SoundType.UI;
        public override string DisplayName
        {
            get
            {
                return "[PlaySound] " + IdentifyName;
            }
        }
        public override void OnBegin(Clip clip)
        {
            base.OnBegin(clip);
            switch (SoundType)
            {
                case SoundType.UI:
                    Air2000.AudioAdapter.Instance.PlayUISound(UISound);
                    break;
                case SoundType.Role:
                    Air2000.AudioAdapter.Instance.PlayRoleSound(RoleSound);
                    break;
                case SoundType.BGM:
                    break;
                default:
                    break;
            }
            m_Status = Status.StopOnNextFrame;
        }
#if UNITY_EDITOR
        public override void DisplayEditorView()
        {
            base.DisplayEditorView();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("SoundType", GUILayout.Width(100));
            SoundType = (SoundType)EditorGUILayout.EnumPopup(SoundType);
            GUILayout.EndHorizontal();

            switch (SoundType)
            {
                case SoundType.UI:
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Sound", GUILayout.Width(100));
                    UISound = (Air2000.UISoundEnum)EditorGUILayout.EnumPopup(UISound);
                    GUILayout.EndHorizontal();
                    break;
                case SoundType.Role:
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Sound", GUILayout.Width(100));
                    RoleSound = (Air2000.RoleSoundEnum)EditorGUILayout.EnumPopup(RoleSound);
                    GUILayout.EndHorizontal();
                    break;
                case SoundType.BGM:
                    break;
                default:
                    break;
            }



        }
#endif
    }
}
