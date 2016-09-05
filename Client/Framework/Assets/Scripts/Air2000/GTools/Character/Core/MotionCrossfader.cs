/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: MotionCrossfader.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/5 15:11:47
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

namespace GTools.Character
{
    #region [class]InspecCrossfader
#if UNITY_EDITOR
    [CustomEditor(typeof(MotionCrossfader))]
    public class InspecMotionCrossfader : Editor
    {
        public MotionCrossfader Instance;
        void OnEnable()
        {
            Instance = target as MotionCrossfader;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Instance == null) return;
            if (GUILayout.Button("Auto Generate"))
            {
                Instance.GetMotionMachine();
                if (Instance.Machine == null) return;
                if (Instance.Machine.Motions == null || Instance.Machine.Motions.Count == 0) return;
                if (Instance.Roots == null) Instance.Roots = new List<MotionCrossfader.From>();

                for (int i = 0; i < Instance.Machine.Motions.Count; i++)
                {
                    Motion motion = Instance.Machine.Motions[i];
                    if (motion == null) continue;
                    MotionCrossfader.From root = Instance.TryGetRoot(motion.Type);
                    if (root == null)
                    {
                        root = new MotionCrossfader.From();
                        root.Motion = motion.Type;
                        root.DisplayName = motion.Type.ToString();
                        Instance.AddRoot(root);
                    }
                }

                for (int i = 0; i < Instance.Roots.Count; i++)
                {
                    MotionCrossfader.From root = Instance.Roots[i];
                    if (root == null) continue;
                    for (int j = 0; j < Instance.Roots.Count; j++)
                    {
                        MotionCrossfader.From target = Instance.Roots[j];
                        if (target == null) continue;
                        if (root.HasTarget(target.Motion) == false)
                        {
                            if (root.Targets == null) root.Targets = new List<MotionCrossfader.To>();
                            if (root.Motion == target.Motion)
                            {
                                root.Targets.Add(new MotionCrossfader.To() { Motion = target.Motion, DisplayName = target.Motion.ToString(), FadeValue = 0f });
                            }
                            else
                            {
                                root.Targets.Add(new MotionCrossfader.To() { Motion = target.Motion, DisplayName = target.Motion.ToString(), FadeValue = 0.2f });
                            }
                        }
                    }
                }
            }
        }
    }
#endif
    #endregion

    #region [class]MotionCrossfader
    public class MotionCrossfader : MonoBehaviour
    {
        #region [class]From
        [Serializable]
        public class From
        {
            public string DisplayName;
            public RoleMotionType Motion;
            public List<To> Targets = new List<To>();
            public bool HasTarget(RoleMotionType type)
            {
                if (Targets == null || Targets.Count == 0) return false;
                for (int i = 0; i < Targets.Count; i++)
                {
                    To target = Targets[i];
                    if (target.Motion == type) return true;
                }
                return false;
            }
            public To GetTarget(RoleMotionType type)
            {
                if (Targets == null || Targets.Count == 0) return null;
                for (int i = 0; i < Targets.Count; i++)
                {
                    To target = Targets[i];
                    if (target == null) continue;
                    if (target.Motion == type) return target;
                }
                return null;
            }
        }
        #endregion

        #region [class]To
        [Serializable]
        public class To
        {
            public string DisplayName;
            public RoleMotionType Motion;
            public float FadeValue;
        }
        #endregion

        #region [Fields]
        public MotionMachine Machine;
        public List<From> Roots = new List<From>();
        private Motion CurrentMotion;
        private Motion LastMotion;
        private bool ForcePlayAnimation;
        #endregion

        #region [Functions]

        #region  monobehaviour
        void Awake() { Machine = GetMotionMachine(); }
        void OnEnable() { ForcePlayAnimation = true; }
        #endregion

        #region set & get
        public void AddRoot(From root)
        {
            if (root == null) return;
            if (Roots == null) Roots = new List<From>();
            for (int i = 0; i < Roots.Count; i++)
            {
                From tempRoot = Roots[i];
                if (tempRoot == null) continue;
                if (tempRoot.Motion == root.Motion) return;
            }
            Roots.Add(root);
        }
        public From TryGetRoot(RoleMotionType type)
        {
            if (Roots == null || Roots.Count == 0) return null;
            for (int i = 0; i < Roots.Count; i++)
            {
                From tempTree = Roots[i];
                if (tempTree == null) continue;
                if (tempTree.Motion == type) return tempTree;
            }
            return null;
        }
        public To GetTarget(RoleMotionType type, RoleMotionType targetType)
        {
            From from = TryGetRoot(type);
            if (from == null) return null;
            return from.GetTarget(targetType);
        }
        public MotionMachine GetMotionMachine()
        {
            if (Machine == null)
            {
                MotionMachine[] machines = GetComponentsInParent<MotionMachine>();
                if (machines != null && machines.Length > 0)
                {
                    Machine = machines[0];
                }
            }
            return Machine;
        }
        #endregion

        #region play control
        public void PlayIdle()
        {
            if (Machine == null) return;
            Motion idleMotion = Machine.GetMotion(RoleMotionType.RMT_Idle);
            if (idleMotion == null) return;
            if (Machine.Animation != null)
            {
                Machine.Animation.Play(idleMotion.ClipName);
            }
        }
        public void Play(Motion motion)
        {
            if (motion == null) return;
            if (Machine == null || Machine.Animation == null) return;
            if (CurrentMotion == null)
            {
                Machine.Animation.Play(motion.ClipName);
            }
            else if (ForcePlayAnimation)
            {
                ForcePlayAnimation = false;
                Machine.Animation.Play(motion.ClipName);
            }
            else if (motion == CurrentMotion)
            {
                if (motion.WrapMode != WrapMode.Loop)
                {
                    AnimationClip clip = Machine.Animation.GetClip(motion.ClipName);

                    Machine.Animation.Stop();
                    Machine.Animation.Play(motion.ClipName);
                }
            }
            else
            {
                To target = GetTarget(CurrentMotion.Type, motion.Type);
                float value = 0.2f;
                if (target != null)
                {
                    value = target.FadeValue;
                }
                Machine.Animation.CrossFade(motion.ClipName, value);
            }
            LastMotion = CurrentMotion;
            CurrentMotion = motion;
        }
        #endregion

        #endregion
    }
    #endregion
}

