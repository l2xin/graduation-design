/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: EffectPlugin.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/6 20:40:51
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Air2000.Character
{
    [Serializable]
    public class EffectPlugin : MotionPlugin
    {
        public override string DisplayName
        {
            get
            {
                return "[Effect] ";
            }
        }
        public EffectRoot Root;
        public string RootPath;
        public override void OnBegin(Motion motion)
        {
            base.OnBegin(motion);
            if (Root == null)
            {
                CurrentStatus = Status.StopOnNextFrame; return;
            }
            Root.PlayEffect();
        }
        public override void OnUpdate(Motion motion)
        {
            base.OnUpdate(motion);
        }
        public override void OnEnd(Motion motion)
        {
            base.OnEnd(motion);
            if (Root != null)
            {
                Root.StopEffect();
            }
        }
        public override void OnTimerBegin()
        {
            base.OnTimerBegin();
            if (Root != null)
            {
                Root.PlayEffect();
            }
        }

        public override void OnTimerEnd()
        {
            base.OnTimerEnd();
            if (Root != null)
            {
                Root.StopEffect();
            }
        }
        public override void OnMachineDisable(MotionMachine machine, Motion motion)
        {
            base.OnMachineDisable(machine, motion);
            if (Root != null)
            {
                //Root.StopEffect();
            }
        }

#if UNITY_EDITOR
        public override void DisplayEditorView(Motion motion)
        {
            base.DisplayEditorView(motion);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Root", GUILayout.Width(140));
            Root = (EffectRoot)EditorGUILayout.ObjectField(Root, typeof(EffectRoot), true);
            GUILayout.EndHorizontal();
        }
        public override void OnPreReplaceAnimation(Motion motion)
        {
            base.OnPreReplaceAnimation(motion);
            if (Root != null && motion != null && motion.GetCharacter() != null && motion.GetCharacter().GetBodyTransform() != null)
            {
                RootPath = Utility.GenerateRootPath(motion.GetCharacter().GetBodyTransform(), Root.transform);
            }
        }
        public override void OnReplacedAnimation(Motion motion)
        {
            base.OnReplacedAnimation(motion);
            if (motion != null && motion.GetCharacter() != null && motion.GetCharacter().GetBodyTransform() != null && string.IsNullOrEmpty(RootPath) == false)
            {
                Transform effectRootTran = motion.GetCharacter().GetBodyTransform().Find(RootPath);
                if (effectRootTran != null)
                {
                    Air2000.Character.EffectRoot com = effectRootTran.GetComponent<Air2000.Character.EffectRoot>();
                    if (com)
                    {
                        Root = com;
                    }
                    else
                    {
                        Debug.LogError("EffectPlugin::OnReplacedAnimation: error cuased by null EffectRoot component attach to " + effectRootTran.name + " (GameObject)");
                    }
                }
                else
                {
                    Debug.LogError("EffectPlugin::OnReplacedAnimation: error cuased by null transform at " + RootPath);
                }
            }
        }
        public override void UpdateDependency(Motion motion)
        {
            base.UpdateDependency(motion);
            if (motion != null)
            {

            }
        }
#endif
    }
}
