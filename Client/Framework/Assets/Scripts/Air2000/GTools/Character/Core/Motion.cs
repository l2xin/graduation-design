/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Motion.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/5 13:16:19
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

#region custom namespace
using GameLogic;
#endregion

namespace Air2000.Character
{
    #region [enum]RoleMotionType
    [Serializable]
    public enum RoleMotionType
    {
        RMT_None,
        RMT_Idle,
        RMT_SpecIdle,
        RMT_Appear,
        RMT_ReachFinalPos,

        RMT_PreRun,
        RMT_Run,
        RMT_RunEnd,

        RMT_PreJump,
        RMT_Jump,
        RMT_JumpEnd,

        RMT_PreTinyJump,
        RMT_TinyJump,
        RMT_TinyJumpEnd,

        RMT_PreFly,
        RMT_Fly,
        RMT_FlyEnd,

        RMT_PreAttack,
        RMT_Attack,
        RMT_AttackEnd,

        RMT_PreBeAttack,
        RMT_BeAttack,
        RMT_BeAttackEnd,

        RMT_PreVictory,
        RMT_Victory,
        RMT_VictoryEnd,

        RMT_PreFail,
        RMT_Fail,
        RMT_FailEnd,

        RMT_ReachFinalPos2,


        RMT_Overlap = 41,

        RMT_Display = 43,

    }
    #endregion

    [Serializable]
    public class Motion
    {
        #region [enum]Method type
        public enum MethodType
        {
            OnBegin,
            OnEnd,
        }
        #endregion

        #region [enum]MotionFinishMode
        [Serializable]
        public enum FinishMode
        {
            Automatic,
            SpecificTime,
            WhenAnimationEnd,
            WhenAllPluginEnd,
        }
        #endregion

        #region [delegate & event]
        public delegate void BeginDelegate(Motion motion);
        public delegate void UpdateDelegate(Motion motion);
        public delegate void PreEndDelegate(Motion motion);
        public delegate void EndDelegate(Motion motion);
        public event BeginDelegate OnBeginDelegate;
        public event UpdateDelegate OnUpdateDelegate;
        public event PreEndDelegate OnPreEndDelegate;
        public event EndDelegate OnEndDelegate;
        #endregion

        #region [Fields]

        #region SerializeField

        public RoleMotionType Type;
        public string ClipName;
        public WrapMode WrapMode;
        public float Speed = 1.0f;
        public FinishMode HowToFinish = FinishMode.WhenAllPluginEnd;
        public MotionMachine Machine;

        #region define for FinishMode=SpecificTime
        public float BeginTime;
        public float EndTime;
        public bool IsPlayAnimation = false;
        #endregion

        #region All kinds of plugins
        [HideInInspector]
        public List<TimeScalePlugin> TimeScalePlugins = new List<TimeScalePlugin>();
        [HideInInspector]
        public List<CameraShakePlugin> CameraShakePlugins = new List<CameraShakePlugin>();
        [HideInInspector]
        public List<HitJudgePlugin> HitJudgePlugins = new List<HitJudgePlugin>();
        [HideInInspector]
        public List<MotionSpeedPlugin> MotionSpeedPlugins = new List<MotionSpeedPlugin>();
        [HideInInspector]
        public List<EffectPlugin> EffectPlugins = new List<EffectPlugin>();
        [HideInInspector]
        public List<MovePlugin> MovePlugins = new List<MovePlugin>();

        //#region custom plugins
        //[HideInInspector]
        //public List<CameraFollowPlugin> CameraFollowPlugins = new List<CameraFollowPlugin>();
        //[HideInInspector]
        //public List<TriggerAttackPlugin> TriggerAttackPlugins = new List<TriggerAttackPlugin>();
        //[HideInInspector]
        //public List<AttackPlugin> AttackPlugins = new List<AttackPlugin>();
        //[HideInInspector]
        //public List<CheckIfGotTargetPlugin> CheckIfGotTargetPlugins = new List<CheckIfGotTargetPlugin>();
        //[HideInInspector]
        //public List<HitGridPlugin> HitGridPlugins = new List<HitGridPlugin>();
        //[HideInInspector]
        //public List<FocusPlugin> FocusPlugins = new List<FocusPlugin>();
        //[HideInInspector]
        //public List<SoundPlugin> SoundPlugins = new List<SoundPlugin>();
        //#endregion

        #endregion

        #endregion

        #region runtime
        [NonSerialized]
        public float CurrentTime;
        [NonSerialized]
        public List<MotionPlugin> Plugins = new List<MotionPlugin>();
        [HideInInspector]
        [NonSerialized]
        public List<MotionPlugin> WaittingUpdatePlugins = new List<MotionPlugin>();
        [HideInInspector]
        [NonSerialized]
        public List<MotionPlugin> UpdatingPlugins = new List<MotionPlugin>();
        #endregion

        #endregion

        #region [Functions]

        #region motionmachine & motion behaviour function
        public virtual void OnMachineAwake(MotionMachine machine) { Machine = machine; NotifyAllPlugin(MotionMachine.MethodType.OnAwake); }
        public virtual void OnMachineStart(MotionMachine machine) { NotifyAllPlugin(MotionMachine.MethodType.OnStart); }
        public virtual void OnMachineEnable(MotionMachine machine) { NotifyAllPlugin(MotionMachine.MethodType.OnEnable); }
        public virtual void OnMachineDisable(MotionMachine machine) { NotifyAllPlugin(MotionMachine.MethodType.OnDisable); }
        public virtual void OnMachineDestroy(MotionMachine machine) { NotifyAllPlugin(MotionMachine.MethodType.OnDestroy); }
        private void NotifyAllPlugin(MotionMachine.MethodType type)
        {
            GetAllPlugin();
            if (Plugins != null && Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MotionPlugin plugin = Plugins[i];
                    if (plugin == null) continue;
                    switch (type)
                    {
                        case MotionMachine.MethodType.OnAwake:
                            plugin.OnMachineAwake(Machine, this);
                            break;
                        case MotionMachine.MethodType.OnStart:
                            plugin.OnMachineStart(Machine, this);
                            break;
                        case MotionMachine.MethodType.OnEnable:
                            plugin.OnMachineEnable(Machine, this);
                            break;
                        case MotionMachine.MethodType.OnDisable:
                            plugin.OnMachineDisable(Machine, this);
                            break;
                        case MotionMachine.MethodType.OnDestroy:
                            plugin.OnMachineDestroy(Machine, this);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void NotifyMotionBehaviourMethod(MethodType type)
        {
            GetAllPlugin();
            if (Plugins != null && Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MotionPlugin plugin = Plugins[i];
                    if (plugin == null) continue;
                    switch (type)
                    {
                        case MethodType.OnBegin:
                            plugin.OnMotionBegin(this);
                            break;
                        case MethodType.OnEnd:
                            plugin.OnMotionEnd(this);
                            break;
                        default:
                            break;
                    }
                }
            }


        }
        public virtual void OnBegin(MotionMachine machine)
        {
            if (machine == null || machine.Animation == null || machine.Crossfader == null) return;
            GetAllPlugin();
            if (WaittingUpdatePlugins == null) WaittingUpdatePlugins = new List<MotionPlugin>();
            if (WaittingUpdatePlugins.Count > 0) WaittingUpdatePlugins.Clear();
            if (UpdatingPlugins == null) UpdatingPlugins = new List<MotionPlugin>();
            if (UpdatingPlugins.Count > 0) UpdatingPlugins.Clear();
            if (Plugins != null && Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MotionPlugin plugin = Plugins[i];
                    if (plugin == null || plugin.PlayOnMotionEnd) continue;
                    WaittingUpdatePlugins.Add(plugin);
                }
            }
            machine.Animation.wrapMode = WrapMode;
            //AnimationClip clip = machine.Animation.GetClip(ClipName);
            //if (clip != null)
            //{
            //    clip.wrapMode = WrapMode;
            //}
            AnimationState state = machine.Animation[ClipName];
            if (state != null)
            {
                if (Speed <= 0)
                {
                    Speed = 1;
                }
                state.speed = Speed;
            }
            CurrentTime = 0f;
            //animator.Animation.Play(Name);
            if (HowToFinish != FinishMode.SpecificTime)
            {
                machine.Crossfader.Play(this);
            }
            else
            {
                IsPlayAnimation = false;
            }
            NotifyMotionBehaviourMethod(MethodType.OnBegin);
        }
        public virtual void OnUpdate(MotionMachine machine)
        {
            if (machine == null) return;
            CurrentTime += Time.deltaTime;
            if (OnUpdateDelegate != null)
            {
                OnUpdateDelegate(this);
            }
            if (UpdatingPlugins != null && UpdatingPlugins.Count > 0)
            {
                for (int i = 0; i < UpdatingPlugins.Count; i++)
                {
                    MotionPlugin plugin = UpdatingPlugins[i];
                    if (plugin == null)
                    {
                        UpdatingPlugins.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (plugin.EndTime <= CurrentTime)
                    {
                        plugin.OnEnd(this);
                        UpdatingPlugins.Remove(plugin);
                        i--;
                    }
                    else if (plugin.CurrentStatus == MotionPlugin.Status.Error || plugin.CurrentStatus == MotionPlugin.Status.StopOnNextFrame)
                    {
                        plugin.OnEnd(this);
                        UpdatingPlugins.Remove(plugin);
                        i--;
                    }
                    else
                    {
                        plugin.OnUpdate(this);
                    }
                }
            }
            if (WaittingUpdatePlugins != null && WaittingUpdatePlugins.Count > 0)
            {
                for (int i = 0; i < WaittingUpdatePlugins.Count; i++)
                {
                    MotionPlugin plugin = WaittingUpdatePlugins[i];
                    if (plugin == null)
                    {
                        WaittingUpdatePlugins.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (plugin.BeginTime <= CurrentTime && plugin.CurrentStatus == MotionPlugin.Status.UnAction)
                    {
                        plugin.OnBegin(this);
                        UpdatingPlugins.Add(plugin);
                        WaittingUpdatePlugins.Remove(plugin);
                        i--;
                    }
                }
            }

            switch (HowToFinish)
            {
                case FinishMode.Automatic:
                case FinishMode.WhenAllPluginEnd:
                    if (Type == RoleMotionType.RMT_Idle)
                    {
                        if (Machine.Animation.isPlaying == false)
                        {
                            Machine.Crossfader.PlayIdle();
                        }
                    }
                    else
                    {
                        if (UpdatingPlugins.Count == 0 && WaittingUpdatePlugins.Count == 0)
                        {
                            FinishMotion();
                        }
                        if (Machine.Animation.isPlaying == false)
                        {
                            Machine.Crossfader.PlayIdle();
                        }
                    }
                    break;
                case FinishMode.SpecificTime:
                    if (CurrentTime >= EndTime && IsPlayAnimation == true)
                    {
                        FinishMotion();
                    }
                    else if (CurrentTime >= BeginTime && IsPlayAnimation == false)
                    {
                        IsPlayAnimation = true;
                        if (Machine != null && Machine.Crossfader != null)
                        {
                            Machine.Crossfader.Play(this);
                        }
                    }
                    else if (Machine.Animation.isPlaying == false)
                    {
                        Machine.Crossfader.PlayIdle();
                    }
                    break;
                case FinishMode.WhenAnimationEnd:
                    if (Machine.Animation.isPlaying == false)
                    {
                        FinishMotion();
                    }
                    break;

                default:
                    break;
            }
        }
        public virtual void OnEnd(MotionMachine machine)
        {
            if (UpdatingPlugins != null || UpdatingPlugins.Count > 0)
            {
                for (int i = 0; i < UpdatingPlugins.Count; i++)
                {
                    MotionPlugin plugin = UpdatingPlugins[i];
                    if (plugin == null || plugin.DontStopOnMotionEnd) continue;
                    plugin.OnEnd(this);
                }
            }
            if (OnEndDelegate != null)
            {
                OnEndDelegate(this);
            }
        }
        public virtual void FinishMotion()
        {
            NotifyMotionBehaviourMethod(MethodType.OnEnd);
            if (OnPreEndDelegate != null)
            {
                OnPreEndDelegate(this);
            }
            //if (Machine.NextMotion == null)
            //{
            //    Machine.Character.ExecuteCommand(CharacterCommand.CC_Stop);
            //}
        }
        public void Clone(Motion obj)
        {
            if (obj == null) return;

            BeginTime = obj.BeginTime;
            EndTime = obj.EndTime;

            WrapMode = obj.WrapMode;
            HowToFinish = obj.HowToFinish;
            Speed = obj.Speed;

            obj.GetAllPlugin();
            if (obj.Plugins != null && obj.Plugins.Count > 0)
            {
                for (int i = 0; i < obj.Plugins.Count; i++)
                {
                    MotionPlugin plugin = obj.Plugins[i];
                    if (plugin == null) continue;
                    MotionPlugin newPlugin = AddPlugin(plugin.GetType());
                    if (newPlugin != null)
                    {
                        newPlugin.Clone(plugin);
                    }
                }
            }

        }
        #endregion

        #region all kinds of plugins's getting | setting methods
        public MotionPlugin AddPlugin(System.Type type)
        {
            if (type == null) return null;
            MotionPlugin plugin = null;
            if (type == typeof(MovePlugin))
            {
                plugin = new MovePlugin();
                MovePlugins.Add(plugin as MovePlugin);
            }
            else if (type == typeof(EffectPlugin))
            {
                plugin = new EffectPlugin();
                EffectPlugins.Add(plugin as EffectPlugin);
            }
            else if (type == typeof(MotionSpeedPlugin))
            {
                plugin = new MotionSpeedPlugin();
                MotionSpeedPlugins.Add(plugin as MotionSpeedPlugin);
            }
            else if (type == typeof(HitJudgePlugin))
            {
                plugin = new HitJudgePlugin();
                HitJudgePlugins.Add(plugin as HitJudgePlugin);
            }
            else if (type == typeof(CameraShakePlugin))
            {
                plugin = new CameraShakePlugin();
                CameraShakePlugins.Add(plugin as CameraShakePlugin);
            }
            else if (type == typeof(TimeScalePlugin))
            {
                plugin = new TimeScalePlugin();
                TimeScalePlugins.Add(plugin as TimeScalePlugin);
            }
            //else if (type == typeof(CameraFollowPlugin))
            //{
            //    plugin = new CameraFollowPlugin();
            //    CameraFollowPlugins.Add(plugin as CameraFollowPlugin);
            //}
            //else if (type == typeof(TriggerAttackPlugin))
            //{
            //    plugin = new TriggerAttackPlugin();
            //    TriggerAttackPlugins.Add(plugin as TriggerAttackPlugin);
            //}
            //else if (type == typeof(AttackPlugin))
            //{
            //    plugin = new AttackPlugin();
            //    AttackPlugins.Add(plugin as AttackPlugin);
            //}
            //else if (type == typeof(CheckIfGotTargetPlugin))
            //{
            //    plugin = new CheckIfGotTargetPlugin();
            //    CheckIfGotTargetPlugins.Add(plugin as CheckIfGotTargetPlugin);
            //}
            //else if (type == typeof(HitGridPlugin))
            //{
            //    plugin = new HitGridPlugin();
            //    HitGridPlugins.Add(plugin as HitGridPlugin);
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    plugin = new FocusPlugin();
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (type == typeof(SoundPlugin))
            //{
            //    plugin = new SoundPlugin();
            //    SoundPlugins.Add(plugin as SoundPlugin);
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    plugin = new FocusPlugin();
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    plugin = new FocusPlugin();
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    plugin = new FocusPlugin();
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    plugin = new FocusPlugin();
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    plugin = new FocusPlugin();
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            return plugin;
        }
        public MotionPlugin AddPlugin(MotionPlugin plugin)
        {
            if (plugin == null) return null;
            if (plugin.GetType() == typeof(MovePlugin))
            {
                MovePlugins.Add(plugin as MovePlugin);
            }
            else if (plugin.GetType() == typeof(EffectPlugin))
            {
                EffectPlugins.Add(plugin as EffectPlugin);
            }
            else if (plugin.GetType() == typeof(MotionSpeedPlugin))
            {
                MotionSpeedPlugins.Add(plugin as MotionSpeedPlugin);
            }
            else if (plugin.GetType() == typeof(HitJudgePlugin))
            {
                HitJudgePlugins.Add(plugin as HitJudgePlugin);
            }
            else if (plugin.GetType() == typeof(CameraShakePlugin))
            {
                CameraShakePlugins.Add(plugin as CameraShakePlugin);
            }
            else if (plugin.GetType() == typeof(TimeScalePlugin))
            {
                TimeScalePlugins.Add(plugin as TimeScalePlugin);
            }
            //else if (plugin.GetType() == typeof(CameraFollowPlugin))
            //{
            //    CameraFollowPlugins.Add(plugin as CameraFollowPlugin);
            //}
            //else if (plugin.GetType() == typeof(TriggerAttackPlugin))
            //{
            //    TriggerAttackPlugins.Add(plugin as TriggerAttackPlugin);
            //}
            //else if (plugin.GetType() == typeof(AttackPlugin))
            //{
            //    AttackPlugins.Add(plugin as AttackPlugin);
            //}
            //else if (plugin.GetType() == typeof(CheckIfGotTargetPlugin))
            //{
            //    CheckIfGotTargetPlugins.Add(plugin as CheckIfGotTargetPlugin);
            //}
            //else if (plugin.GetType() == typeof(HitGridPlugin))
            //{
            //    HitGridPlugins.Add(plugin as HitGridPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(SoundPlugin))
            //{
            //    SoundPlugins.Add(plugin as SoundPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Add(plugin as FocusPlugin);
            //}
            return plugin;
        }
        public void RemovePlugin(MotionPlugin plugin)
        {
            if (plugin == null) return;
            if (plugin.GetType() == typeof(MovePlugin))
            {
                MovePlugins.Remove(plugin as MovePlugin);
            }
            else if (plugin.GetType() == typeof(EffectPlugin))
            {
                EffectPlugins.Remove(plugin as EffectPlugin);
            }
            else if (plugin.GetType() == typeof(MotionSpeedPlugin))
            {
                MotionSpeedPlugins.Remove(plugin as MotionSpeedPlugin);
            }
            else if (plugin.GetType() == typeof(HitJudgePlugin))
            {
                HitJudgePlugins.Remove(plugin as HitJudgePlugin);
            }
            else if (plugin.GetType() == typeof(CameraShakePlugin))
            {
                CameraShakePlugins.Remove(plugin as CameraShakePlugin);
            }
            else if (plugin.GetType() == typeof(TimeScalePlugin))
            {
                TimeScalePlugins.Remove(plugin as TimeScalePlugin);
            }
            //else if (plugin.GetType() == typeof(CameraFollowPlugin))
            //{
            //    CameraFollowPlugins.Remove(plugin as CameraFollowPlugin);
            //}
            //else if (plugin.GetType() == typeof(TriggerAttackPlugin))
            //{
            //    TriggerAttackPlugins.Remove(plugin as TriggerAttackPlugin);
            //}
            //else if (plugin.GetType() == typeof(AttackPlugin))
            //{
            //    AttackPlugins.Remove(plugin as AttackPlugin);
            //}
            //else if (plugin.GetType() == typeof(CheckIfGotTargetPlugin))
            //{
            //    CheckIfGotTargetPlugins.Remove(plugin as CheckIfGotTargetPlugin);
            //}
            //else if (plugin.GetType() == typeof(HitGridPlugin))
            //{
            //    HitGridPlugins.Remove(plugin as HitGridPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Remove(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(SoundPlugin))
            //{
            //    SoundPlugins.Remove(plugin as SoundPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Remove(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Remove(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Remove(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Remove(plugin as FocusPlugin);
            //}
            //else if (plugin.GetType() == typeof(FocusPlugin))
            //{
            //    FocusPlugins.Remove(plugin as FocusPlugin);
            //}
        }
        public void ClearPlugin()
        {
            if (MotionSpeedPlugins != null) MotionSpeedPlugins.Clear();
            if (MovePlugins != null) MovePlugins.Clear();
            if (EffectPlugins != null) EffectPlugins.Clear();
            if (HitJudgePlugins != null) HitJudgePlugins.Clear();
            if (CameraShakePlugins != null) CameraShakePlugins.Clear();
            if (TimeScalePlugins != null) TimeScalePlugins.Clear();
            //if (CameraFollowPlugins != null) CameraFollowPlugins.Clear();
            //if (TriggerAttackPlugins != null) TriggerAttackPlugins.Clear();
            //if (AttackPlugins != null) AttackPlugins.Clear();
            //if (CheckIfGotTargetPlugins != null) CheckIfGotTargetPlugins.Clear();
            //if (HitGridPlugins != null) HitGridPlugins.Clear();
            //if (FocusPlugins != null) FocusPlugins.Clear();
            //if (SoundPlugins != null) SoundPlugins.Clear();
            //if (FocusPlugins != null) FocusPlugins.Clear();
            //if (FocusPlugins != null) FocusPlugins.Clear();
            //if (FocusPlugins != null) FocusPlugins.Clear();
            //if (FocusPlugins != null) FocusPlugins.Clear();
            //if (FocusPlugins != null) FocusPlugins.Clear();

        }
        public List<MotionPlugin> GetAllPlugin()
        {
            if (Plugins == null) Plugins = new List<MotionPlugin>();
            Plugins.Clear();
            if (MotionSpeedPlugins != null && MotionSpeedPlugins.Count > 0)
            {
                for (int i = 0; i < MotionSpeedPlugins.Count; i++)
                {
                    MotionSpeedPlugin plugin = MotionSpeedPlugins[i];
                    if (plugin == null)
                    {
                        MotionSpeedPlugins.RemoveAt(i); i--; continue;
                    }
                    Plugins.Add(plugin);
                }
            }
            if (MovePlugins != null && MovePlugins.Count > 0)
            {
                for (int i = 0; i < MovePlugins.Count; i++)
                {
                    MotionPlugin plugin = MovePlugins[i];
                    if (plugin == null)
                    {
                        MovePlugins.RemoveAt(i); i--; continue;
                    }
                    Plugins.Add(plugin);
                }
            }
            if (EffectPlugins != null && EffectPlugins.Count > 0)
            {
                for (int i = 0; i < EffectPlugins.Count; i++)
                {
                    EffectPlugin plugin = EffectPlugins[i];
                    if (plugin == null)
                    {
                        EffectPlugins.RemoveAt(i); i--; continue;
                    }
                    Plugins.Add(plugin);
                }
            }
            if (HitJudgePlugins != null && HitJudgePlugins.Count > 0)
            {
                for (int i = 0; i < HitJudgePlugins.Count; i++)
                {
                    HitJudgePlugin plugin = HitJudgePlugins[i];
                    if (plugin == null)
                    {
                        HitJudgePlugins.RemoveAt(i); i--; continue;
                    }
                    Plugins.Add(plugin);
                }
            }
            if (CameraShakePlugins != null && CameraShakePlugins.Count > 0)
            {
                for (int i = 0; i < CameraShakePlugins.Count; i++)
                {
                    CameraShakePlugin plugin = CameraShakePlugins[i];
                    if (plugin == null)
                    {
                        CameraShakePlugins.RemoveAt(i); i--; continue;
                    }
                    Plugins.Add(plugin);
                }
            }
            if (TimeScalePlugins != null && TimeScalePlugins.Count > 0)
            {
                for (int i = 0; i < TimeScalePlugins.Count; i++)
                {
                    TimeScalePlugin plugin = TimeScalePlugins[i];
                    if (plugin == null)
                    {
                        TimeScalePlugins.RemoveAt(i); i--; continue;
                    }
                    Plugins.Add(plugin);
                }
            }
            //if (CameraFollowPlugins != null && CameraFollowPlugins.Count > 0)
            //{
            //    for (int i = 0; i < CameraFollowPlugins.Count; i++)
            //    {
            //        CameraFollowPlugin plugin = CameraFollowPlugins[i];
            //        if (plugin == null)
            //        {
            //            CameraFollowPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (TriggerAttackPlugins != null && TriggerAttackPlugins.Count > 0)
            //{
            //    for (int i = 0; i < TriggerAttackPlugins.Count; i++)
            //    {
            //        TriggerAttackPlugin plugin = TriggerAttackPlugins[i];
            //        if (plugin == null)
            //        {
            //            TriggerAttackPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (AttackPlugins != null && AttackPlugins.Count > 0)
            //{
            //    for (int i = 0; i < AttackPlugins.Count; i++)
            //    {
            //        AttackPlugin plugin = AttackPlugins[i];
            //        if (plugin == null)
            //        {
            //            AttackPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (CheckIfGotTargetPlugins != null && CheckIfGotTargetPlugins.Count > 0)
            //{
            //    for (int i = 0; i < CheckIfGotTargetPlugins.Count; i++)
            //    {
            //        CheckIfGotTargetPlugin plugin = CheckIfGotTargetPlugins[i];
            //        if (plugin == null)
            //        {
            //            CheckIfGotTargetPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (HitGridPlugins != null && HitGridPlugins.Count > 0)
            //{
            //    for (int i = 0; i < HitGridPlugins.Count; i++)
            //    {
            //        HitGridPlugin plugin = HitGridPlugins[i];
            //        if (plugin == null)
            //        {
            //            HitGridPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (FocusPlugins != null && FocusPlugins.Count > 0)
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            AttackPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (SoundPlugins != null && SoundPlugins.Count > 0)
            //{
            //    for (int i = 0; i < SoundPlugins.Count; i++)
            //    {
            //        SoundPlugin plugin = SoundPlugins[i];
            //        if (plugin == null)
            //        {
            //            SoundPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (FocusPlugins != null && FocusPlugins.Count > 0)
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            AttackPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (FocusPlugins != null && FocusPlugins.Count > 0)
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            AttackPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (FocusPlugins != null && FocusPlugins.Count > 0)
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            AttackPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (FocusPlugins != null && FocusPlugins.Count > 0)
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            AttackPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            //if (FocusPlugins != null && FocusPlugins.Count > 0)
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            AttackPlugins.RemoveAt(i); i--; continue;
            //        }
            //        Plugins.Add(plugin);
            //    }
            //}
            return Plugins;
        }
        public MotionPlugin GetPlugin(string pluginName, System.Type type)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                Utility.LogError("GetAction error caused by null action name"); return null;
            }
            if (type == typeof(MovePlugin))
            {
                for (int i = 0; i < MovePlugins.Count; i++)
                {
                    MovePlugin plugin = MovePlugins[i];
                    if (plugin == null)
                    {
                        continue;
                    }
                    if (plugin.IdentifyName == pluginName) return plugin;
                }
            }
            else if (type == typeof(EffectPlugin))
            {
                for (int i = 0; i < EffectPlugins.Count; i++)
                {
                    EffectPlugin plugin = EffectPlugins[i];
                    if (plugin == null)
                    {
                        continue;
                    }
                    if (plugin.IdentifyName == pluginName) return plugin;
                }
            }
            else if (type == typeof(MotionSpeedPlugin))
            {
                for (int i = 0; i < MotionSpeedPlugins.Count; i++)
                {
                    MotionSpeedPlugin plugin = MotionSpeedPlugins[i];
                    if (plugin == null)
                    {
                        continue;
                    }
                    if (plugin.IdentifyName == pluginName) return plugin;
                }
            }
            else if (type == typeof(HitJudgePlugin))
            {
                for (int i = 0; i < HitJudgePlugins.Count; i++)
                {
                    HitJudgePlugin plugin = HitJudgePlugins[i];
                    if (plugin == null)
                    {
                        continue;
                    }
                    if (plugin.IdentifyName == pluginName) return plugin;
                }
            }
            else if (type == typeof(CameraShakePlugin))
            {
                for (int i = 0; i < CameraShakePlugins.Count; i++)
                {
                    CameraShakePlugin plugin = CameraShakePlugins[i];
                    if (plugin == null)
                    {
                        continue;
                    }
                    if (plugin.IdentifyName == pluginName) return plugin;
                }
            }
            else if (type == typeof(TimeScalePlugin))
            {
                for (int i = 0; i < TimeScalePlugins.Count; i++)
                {
                    TimeScalePlugin plugin = TimeScalePlugins[i];
                    if (plugin == null)
                    {
                        continue;
                    }
                    if (plugin.IdentifyName == pluginName) return plugin;
                }
            }
            //else if (type == typeof(CameraFollowPlugin))
            //{
            //    for (int i = 0; i < CameraFollowPlugins.Count; i++)
            //    {
            //        CameraFollowPlugin plugin = CameraFollowPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(TriggerAttackPlugin))
            //{
            //    for (int i = 0; i < TriggerAttackPlugins.Count; i++)
            //    {
            //        TriggerAttackPlugin plugin = TriggerAttackPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(AttackPlugin))
            //{
            //    for (int i = 0; i < AttackPlugins.Count; i++)
            //    {
            //        AttackPlugin plugin = AttackPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(CheckIfGotTargetPlugin))
            //{
            //    for (int i = 0; i < CheckIfGotTargetPlugins.Count; i++)
            //    {
            //        CheckIfGotTargetPlugin plugin = CheckIfGotTargetPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(HitGridPlugin))
            //{
            //    for (int i = 0; i < HitGridPlugins.Count; i++)
            //    {
            //        HitGridPlugin plugin = HitGridPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(SoundPlugin))
            //{
            //    for (int i = 0; i < SoundPlugins.Count; i++)
            //    {
            //        SoundPlugin plugin = SoundPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            //else if (type == typeof(FocusPlugin))
            //{
            //    for (int i = 0; i < FocusPlugins.Count; i++)
            //    {
            //        FocusPlugin plugin = FocusPlugins[i];
            //        if (plugin == null)
            //        {
            //            continue;
            //        }
            //        if (plugin.IdentifyName == pluginName) return plugin;
            //    }
            //}
            return null;
        }
        public MotionPlugin GetPlugin(string pluginName)
        {
            if (string.IsNullOrEmpty(pluginName))
            {
                // Utility.LogError("GetPlugin error caused by null action name"); return null;
            }
            for (int i = 0; i < MovePlugins.Count; i++)
            {
                MovePlugin plugin = MovePlugins[i];
                if (plugin == null)
                {
                    continue;
                }
                if (plugin.IdentifyName == pluginName) return plugin;
            }
            for (int i = 0; i < EffectPlugins.Count; i++)
            {
                EffectPlugin plugin = EffectPlugins[i];
                if (plugin == null)
                {
                    continue;
                }
                if (plugin.IdentifyName == pluginName) return plugin;
            }
            for (int i = 0; i < MotionSpeedPlugins.Count; i++)
            {
                MotionSpeedPlugin plugin = MotionSpeedPlugins[i];
                if (plugin == null)
                {
                    continue;
                }
                if (plugin.IdentifyName == pluginName) return plugin;
            }
            for (int i = 0; i < HitJudgePlugins.Count; i++)
            {
                HitJudgePlugin plugin = HitJudgePlugins[i];
                if (plugin == null)
                {
                    continue;
                }
                if (plugin.IdentifyName == pluginName) return plugin;
            }
            for (int i = 0; i < CameraShakePlugins.Count; i++)
            {
                CameraShakePlugin plugin = CameraShakePlugins[i];
                if (plugin == null)
                {
                    continue;
                }
                if (plugin.IdentifyName == pluginName) return plugin;
            }
            for (int i = 0; i < TimeScalePlugins.Count; i++)
            {
                TimeScalePlugin plugin = TimeScalePlugins[i];
                if (plugin == null)
                {
                    continue;
                }
                if (plugin.IdentifyName == pluginName) return plugin;
            }
            //for (int i = 0; i < CameraFollowPlugins.Count; i++)
            //{
            //    CameraFollowPlugin plugin = CameraFollowPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < TriggerAttackPlugins.Count; i++)
            //{
            //    TriggerAttackPlugin plugin = TriggerAttackPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < AttackPlugins.Count; i++)
            //{
            //    AttackPlugin plugin = AttackPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < CheckIfGotTargetPlugins.Count; i++)
            //{
            //    CheckIfGotTargetPlugin plugin = CheckIfGotTargetPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < HitGridPlugins.Count; i++)
            //{
            //    HitGridPlugin plugin = HitGridPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < FocusPlugins.Count; i++)
            //{
            //    FocusPlugin plugin = FocusPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < SoundPlugins.Count; i++)
            //{
            //    SoundPlugin plugin = SoundPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < FocusPlugins.Count; i++)
            //{
            //    FocusPlugin plugin = FocusPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < FocusPlugins.Count; i++)
            //{
            //    FocusPlugin plugin = FocusPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < FocusPlugins.Count; i++)
            //{
            //    FocusPlugin plugin = FocusPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < FocusPlugins.Count; i++)
            //{
            //    FocusPlugin plugin = FocusPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            //for (int i = 0; i < FocusPlugins.Count; i++)
            //{
            //    FocusPlugin plugin = FocusPlugins[i];
            //    if (plugin == null)
            //    {
            //        continue;
            //    }
            //    if (plugin.IdentifyName == pluginName) return plugin;
            //}
            return null;
        }
        #endregion

        #region getting & setting
        public AnimationClip GetAnimationClip()
        {
            if (string.IsNullOrEmpty(ClipName)) return null;
            if (Machine == null || Machine.Animation == null) return null;
            return Machine.Animation.GetClip(ClipName);
        }
        public AnimationState GetAnimationState()
        {
            if (string.IsNullOrEmpty(ClipName)) return null;
            if (Machine == null || Machine.Animation == null) return null;
            return Machine.Animation[ClipName];
        }
        public MotionMachine GetMotionMachine()
        {
            return Machine;
        }
        public Character GetCharacter()
        {
            if (Machine != null)
            {
                return Machine.GetCharacter();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region mark as obsolete at 2016/7/19 16:30
        public void ClampTime()
        {
            // dont clamp here 20160614
            //if (GetMinActionBeginTime() != -1) { BeginTime = GetMinActionBeginTime(); }
            //if (GetMaxActionEndTime() != -1) { EndTime = GetMaxActionEndTime(); }
        }
        public float GetMinActionBeginTime()
        {
            GetAllPlugin();
            if (Plugins == null || Plugins.Count == 0) return -1;
            float minTime = Mathf.Infinity;
            for (int i = 0; i < Plugins.Count; i++)
            {
                MotionPlugin plugin = Plugins[i];
                if (plugin == null) continue;
                if (plugin.BeginTime < minTime)
                {
                    minTime = plugin.BeginTime;
                }
            }
            if (minTime < 0) minTime = 0;
            return minTime;
        }
        public float GetMaxActionEndTime()
        {
            GetAllPlugin();
            if (Plugins == null || Plugins.Count == 0) return -1;
            float maxTime = -Mathf.Infinity;
            for (int i = 0; i < Plugins.Count; i++)
            {
                MotionPlugin plugin = Plugins[i];
                if (plugin == null) continue;
                if (plugin.EndTime > maxTime)
                {
                    maxTime = plugin.EndTime;
                }
            }
            if (maxTime < 0) maxTime = 0;
            return maxTime;
        }
        #endregion

        #region editor function
#if UNITY_EDITOR
        public void DiaplayEditorView(MotionMachine machine)
        {
            Machine = machine;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type", GUILayout.Width(100));
            Type = (RoleMotionType)EditorGUILayout.EnumPopup(Type);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ClipName", GUILayout.Width(100));
            ClipName = EditorGUILayout.TextField(ClipName);
            GUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(ClipName))
            {
                ClipName = Type.ToString();
            }

            AnimationClip clip = GetAnimationClip();
            if (clip != null)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ClipLength", GUILayout.Width(100));
                EditorGUILayout.FloatField(clip.length);
                GUILayout.EndHorizontal();
            }


            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Speed", GUILayout.Width(100));
            Speed = EditorGUILayout.FloatField(Speed);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("WrapMode", GUILayout.Width(100));
            WrapMode = (WrapMode)EditorGUILayout.EnumPopup(WrapMode);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("FinishMode", GUILayout.Width(100));
            HowToFinish = (FinishMode)EditorGUILayout.EnumPopup(HowToFinish);
            GUILayout.EndHorizontal();

            if (HowToFinish == FinishMode.SpecificTime)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("BeginTime", GUILayout.Width(100));
                BeginTime = EditorGUILayout.FloatField(BeginTime);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("EndTime", GUILayout.Width(100));
                EndTime = EditorGUILayout.FloatField(EndTime);
                GUILayout.EndHorizontal();
            }
        }
        public void OnPreReplaceAnimation()
        {
            GetAllPlugin();
            if (Plugins != null && Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MotionPlugin plugin = Plugins[i];
                    if (plugin == null) continue;
                    plugin.OnPreReplaceAnimation(this);
                }
            }
        }
        public void OnReplacedAnimation()
        {
            GetAllPlugin();
            if (Plugins != null && Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MotionPlugin plugin = Plugins[i];
                    if (plugin == null) continue;
                    plugin.OnReplacedAnimation(this);
                }
            }
        }
        public void UpdateDependency()
        {
            GetAllPlugin();
            if (Plugins != null && Plugins.Count > 0)
            {
                for (int i = 0; i < Plugins.Count; i++)
                {
                    MotionPlugin plugin = Plugins[i];
                    if (plugin == null) continue;
                    plugin.UpdateDependency(this);
                }
            }
        }
#endif
        #endregion

        #endregion
    }
}
