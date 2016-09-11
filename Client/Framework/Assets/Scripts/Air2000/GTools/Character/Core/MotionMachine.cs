/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: MotionMachine.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/5 12:50:55
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Air2000.Character
{
    public class MotionMachine : MonoBehaviour
    {
        #region [enum]Method type
        public enum MethodType
        {
            OnAwake,
            OnStart,
            OnEnable,
            OnDisable,
            OnDestroy,
        }
        #endregion

        #region [delegate & event]
        //public delegate void AnimatorCannotPlayDelegate(MotionMachine animator);
        //public delegate void AnimatorPreBeginDelegate(MotionMachine animator, PlayTask task);
        //public delegate void AnimatorBeginDelegate(MotionMachine animator);
        //public delegate void AnimatorUpdateDelegate(MotionMachine animator);
        //public delegate void AnimatorUpdatePercentageDelegate(MotionMachine animator, float currentPlayTime, float currentPecentage);
        //public delegate void AnimatorEndDelegate(MotionMachine animator);
        //public delegate void AnimatorPauseDelegate(MotionMachine animator, float currentTime);
        //public delegate void AnimatorUnPauseDelegate(MotionMachine animator, float currentTime, float pauseTime);
        //public delegate void AnimatorStopDelegate(MotionMachine animator);

        //public event AnimatorCannotPlayDelegate CannotPlayDelegate;
        //public event AnimatorPreBeginDelegate PreBeginDelegate;
        //public event AnimatorBeginDelegate BeginDelegate;
        //public event AnimatorUpdateDelegate UpdateDelegate;
        //public event AnimatorUpdatePercentageDelegate UpdatePercentageDelegate;
        //public event AnimatorEndDelegate EndDelegate;
        //public event AnimatorPauseDelegate PauseDelegate;
        //public event AnimatorUnPauseDelegate UnPauseDelegate;
        //public event AnimatorStopDelegate StopDelegate;
        #endregion

        #region [Fields]

        [HideInInspector]
        [SerializeField]
        public List<Motion> Motions = new List<Motion>();
        [NonSerialized]
        public bool IsPause = false;
        [NonSerialized]
        public float PauseTime;
        public Character Character;
        public Animation Animation;
        public MotionCrossfader Crossfader;
        public MotionCommander Commander;

        [NonSerialized]
        public Motion LastMotion = null;
        [NonSerialized]
        public Motion CurrentMotion = null;
        [NonSerialized]
        public Motion NextMotion = null;

        #endregion

        #region [Function]

        #region monobehaviour
        void Awake()
        {
            GetCharacter();
            GetAnimation();
            GetCrossfader();
            GetCommander();
            NotifyAllMotion(MethodType.OnAwake);
        }
        void Start() { NotifyAllMotion(MethodType.OnStart); }
        void OnEnable() { ExecuteMotion(RoleMotionType.RMT_Idle); NotifyAllMotion(MethodType.OnEnable); }
        void OnDisable() { NotifyAllMotion(MethodType.OnDisable); }
        void Update()
        {
            if (IsPause)
            {
                PauseTime += Time.deltaTime;
                return;
            }
            if (CurrentMotion != null)
            {
                CurrentMotion.OnUpdate(this);
            }
            if (NextMotion != null)
            {
                if (CurrentMotion != null)
                {
                    CurrentMotion.OnEnd(this);
                }
                CurrentMotion = NextMotion;
                NextMotion = null;
                CurrentMotion.OnBegin(this);
                CurrentMotion.OnUpdate(this);
            }
        }
        void OnDestroy()
        {
            LastMotion = null;
            CurrentMotion = null;
            NextMotion = null;
            NotifyAllMotion(MethodType.OnDestroy);
            if (Motions != null)
            {
                Motions.Clear();
            }
        }
        void NotifyAllMotion(MethodType type)
        {
            if (Motions != null && Motions.Count > 0)
            {
                for (int i = 0; i < Motions.Count; i++)
                {
                    Motion motion = Motions[i];
                    if (motion == null) continue;
                    switch (type)
                    {
                        case MethodType.OnAwake:
                            motion.OnMachineAwake(this);
                            break;
                        case MethodType.OnStart:
                            motion.OnMachineStart(this);
                            break;
                        case MethodType.OnEnable:
                            motion.OnMachineEnable(this);
                            break;
                        case MethodType.OnDisable:
                            motion.OnMachineDisable(this);
                            break;
                        case MethodType.OnDestroy:
                            motion.OnMachineDestroy(this);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region play control
        public RoleMotionType LastPlayMotion = RoleMotionType.RMT_Attack;
        public bool ExecuteMotion(RoleMotionType type)
        {
            //if (CurrentMotion != null)
            //{
            //    if (CurrentMotion.Type == type)
            //    {
            //        if (CurrentMotion.WrapMode == WrapMode.Loop)
            //        {
            //            return false;
            //        }
            //    }
            //}
            //if (LastPlayMotion == RoleMotionType.RMT_Jump)
            //{
            //    int a = 1;
            //}
            //LastPlayMotion = type;

            Motion nextMotion = GetMotion(type);
            if (nextMotion == null) return false;
            NextMotion = nextMotion;
            return true;
        }
        public bool ExecuteMotionImmediately(RoleMotionType type)
        {
            if (CurrentMotion != null)
            {
                if (CurrentMotion.Type == type) return false;
            }
            Motion nextMotion = GetMotion(type);
            if (nextMotion == null) return false;
            if (CurrentMotion != null) CurrentMotion.OnEnd(this);
            CurrentMotion = nextMotion;
            CurrentMotion.OnBegin(this);
            return true;
        }
        #endregion

        #region getting & setting
        public Motion GetMotion(RoleMotionType type)
        {
            if (Motions == null || Motions.Count == 0) return null;
            for (int i = 0; i < Motions.Count; i++)
            {
                Motion motion = Motions[i];
                if (motion == null) continue;
                if (type.Equals(motion.Type)) return motion;
            }
            return null;
        }
        public MotionPlugin GetSpecificPlugin(RoleMotionType type, string pluginName, Type pluginType)
        {
            Motion motion = GetMotion(type);
            if (motion == null)
            {
                Utility.LogError("GetSpecificPlugin error caused by null motion named: " + type.ToString());
                return null;
            }
            return motion.GetPlugin(pluginName, pluginType);
        }
        public MotionPlugin GetSpecificPlugin(RoleMotionType type, string pluginName)
        {
            Motion motion = GetMotion(type);
            if (motion == null)
            {
                Utility.LogError("GetSpecificPlugin error caused by null motion named: " + type.ToString());
                return null;
            }
            return motion.GetPlugin(pluginName);
        }
        public Animation GetAnimation()
        {
            if (Animation == null)
            {
                Animation[] animations = GetComponentsInChildren<Animation>(true);
                if (animations != null && animations.Length > 0)
                {
                    Animation = animations[0];
                }
            }
            return Animation;
        }
        public Character GetCharacter()
        {
            if (Character == null)
            {
                Character[] chs = GetComponentsInParent<Character>(true);
                if (chs != null && chs.Length > 0)
                {
                    Character = chs[0];
                }
            }
            return Character;
        }
        public MotionCrossfader GetCrossfader()
        {
            if (Crossfader == null)
            {
                MotionCrossfader[] faders = GetComponentsInChildren<MotionCrossfader>(true);
                if (faders != null && faders.Length > 0)
                {
                    Crossfader = faders[0];
                }
            }
            return Crossfader;
        }
        public MotionCommander GetCommander()
        {
            if (Commander == null)
            {
                MotionCommander[] commanders = GetComponentsInChildren<MotionCommander>(true);
                if (commanders != null && commanders.Length > 0)
                {
                    Commander = commanders[0];
                }
            }
            return Commander;
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
        public void OnPreReplaceAnimation()
        {
            if (Motions != null && Motions.Count > 0)
            {
                for (int i = 0; i < Motions.Count; i++)
                {
                    Motion motion = Motions[i];
                    if (motion == null) continue;
                    motion.OnPreReplaceAnimation();
                }
            }
        }
        public void OnReplacedAnimation()
        {
            if (Motions != null && Motions.Count > 0)
            {
                for (int i = 0; i < Motions.Count; i++)
                {
                    Motion motion = Motions[i];
                    if (motion == null) continue;
                    motion.OnReplacedAnimation();
                }
            }
        }
        public void UpdateDependency()
        {
            if (GetCharacter() != null)
            {
                Animation = GetCharacter().GetAnimation();
            }
            GetCrossfader();
            GetCommander();
            if (Motions != null && Motions.Count > 0)
            {
                for (int i = 0; i < Motions.Count; i++)
                {
                    Motion motion = Motions[i];
                    if (motion == null) continue;
                    motion.UpdateDependency();
                }
            }
        }
#endif
        #endregion

        #endregion
    }
}
