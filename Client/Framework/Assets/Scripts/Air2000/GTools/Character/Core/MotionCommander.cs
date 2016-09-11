/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: MotionCommander.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/10 12:23:00
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
    #region [class]InspecMotionCommander
#if UNITY_EDITOR
    [CustomEditor(typeof(MotionCommander))]
    public class InspecMotionCommander : Editor
    {
        public MotionCommander Instance;
        void OnEnable()
        {
            Instance = target as MotionCommander;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Instance == null) return;
            if (GUILayout.Button("Auto Generate"))
            {
                MotionMachine machine = Instance.GetMotionMachine();
                if (machine == null || machine.Motions == null || machine.Motions.Count == 0) return;
                if (Instance.Commands == null) Instance.Commands = new List<MotionCommander.Command>();
                List<Motion> motions = machine.Motions;
                for (int i = 0; i < motions.Count; i++)
                {
                    Motion motion = motions[i];
                    if (motion == null) continue;
                    string[] strArray = motion.Type.ToString().Split(new char[] { '_' });
                    if (strArray == null || strArray.Length < 2)
                    {
                        continue;
                    }
                    string tempStr = strArray[1];
                    if (tempStr.StartsWith("Pre") || tempStr.EndsWith("End"))
                    {
                        continue;
                    }
                    CharacterCommand commandType = Utility.RMT_CC(motion.Type);
                    MotionCommander.Command command = Instance.TryGet(commandType);
                    if (command == null)
                    {
                        command = new MotionCommander.Command();
                        command.Type = commandType;
                        command.DisplayName = commandType.ToString();
                        command.SequentialMotions.Add(motion.Type);
                        Instance.Add(command);
                    }
                }
            }
        }
    }
#endif
    #endregion
    #region [enum]CharacterCommand
    [Serializable]
    public enum CharacterCommand
    {
        CC_None,
        CC_Stop,
        CC_Appear,
        CC_Disappear,
        CC_ReachFinalPos,
        CC_Fail,
        CC_RunToPoint,
        CC_WalkToPoint,
        CC_JumpToPoint,
        CC_FlyToPoint,
        CC_Attack,
        CC_BeAttack,
        CC_Victory,
        CC_ReachFinalPos2,
        CC_Overlap=20,
        CC_Display = 21,
    }
    #endregion
    public class MotionCommander : MonoBehaviour
    {
        #region [class]Command
        [Serializable]
        public class Command
        {
            #region [delegate & event]
            public delegate void ActiveDelegate(Command currentCommand, Command lastCommand);
            public delegate void InActiveDelegate(Command currentCommand, Command nextComand);
            public delegate void FinishDelegate(Command cmd, Motion finishMotion);
            public event ActiveDelegate OnActiveDelegate;
            public event InActiveDelegate OnInActiveDelegate;
            public event FinishDelegate OnFinishDelegate;
            #endregion

            #region [Fields]
            public string DisplayName;
            public CharacterCommand Type;
            public List<RoleMotionType> SequentialMotions = new List<RoleMotionType>();
            [NonSerialized]
            public List<Motion> Motions = new List<Motion>();
            [NonSerialized]
            public MotionMachine Machine;
            [NonSerialized]
            public MotionCommander Commander;
            [NonSerialized]
            public bool ActiveStatus = false;
            #endregion

            #region [Functions]
            private void OnLastMotionEnd(Motion motion)
            {
                if (motion != null)
                {
                    motion.OnPreEndDelegate -= OnLastMotionEnd;
                }
                Motion nextMotion = null;
                if (Machine == null || Commander == null) return;
                if (Motions == null || Motions.Count == 0) return;
                for (int i = 0; i < Motions.Count; i++)
                {
                    Motion tempMotion = Motions[i];
                    if (tempMotion == null) continue;
                    if (tempMotion == motion)
                    {
                        if (i < Motions.Count - 1)
                        {
                            nextMotion = Motions[i + 1];
                            break;
                        }
                    }
                }
                if (nextMotion == null)
                {
                    FinishCommand(motion); return;
                }
                nextMotion.OnPreEndDelegate += OnLastMotionEnd;
                Machine.ExecuteMotion(nextMotion.Type);
            }
            private void FinishCommand(Motion motion)
            {
                if (Commander) Commander.ExecuteCommand(CharacterCommand.CC_Stop);
                if (OnFinishDelegate != null)
                {
                    OnFinishDelegate(this, motion);
                }
            }
            public void OnAwake(MotionMachine machine, MotionCommander commander)
            {
                Machine = machine;
                Commander = commander;
                if (machine != null)
                {
                    if (machine.Motions != null && machine.Motions.Count > 0 && SequentialMotions != null && SequentialMotions.Count > 0)
                    {
                        for (int i = 0; i < SequentialMotions.Count; i++)
                        {
                            RoleMotionType motionType = SequentialMotions[i];
                            Motion motion = machine.GetMotion(motionType);
                            if (motion == null) continue;
                            if (Motions == null) Motions = new List<Motion>();
                            Motions.Add(motion);
                        }
                    }
                }
            }
            public void OnDestroy(MotionMachine machine, MotionCommander commander)
            {
                if (Motions != null)
                {
                    Motions.Clear();
                }
                Machine = null;
                Commander = null;
            }
            public void Active(MotionMachine machine, MotionCommander commander, Command lastCommand)
            {
                //Debug.LogError(Machine.Character.name+" --active "+ Type.ToString());
                if (machine == null || commander == null) return;
                if (Motions == null || Motions.Count == 0) return;
                Motion motion = Motions[0];
                if (motion == null) return;
                ActiveStatus = true;
                //motion.OnPreEndDelegate -= OnLastMotionEnd;
                motion.OnPreEndDelegate += OnLastMotionEnd;
                if (OnActiveDelegate != null)
                {
                    OnActiveDelegate(this, lastCommand);
                }
                machine.ExecuteMotion(motion.Type);
                return;
            }
            public void InActive(MotionMachine machine, MotionCommander commander, Command nextCommand)
            {
                ActiveStatus = false;
                if (OnInActiveDelegate != null)
                {
                    OnInActiveDelegate(this, nextCommand);
                }
                if (Motions != null && Motions.Count > 0)
                {
                    for (int i = 0; i < Motions.Count; i++)
                    {
                        Motion tempMotion = Motions[i];
                        if (tempMotion == null) continue;
                        tempMotion.OnPreEndDelegate -= OnLastMotionEnd;
                    }
                }
            }
            public Motion TryGetMotion(RoleMotionType type)
            {
                if (Motions == null || Motions.Count == 0) return null;
                for (int i = 0; i < Motions.Count; i++)
                {
                    Motion tempMotion = Motions[i];
                    if (tempMotion == null) continue;
                    if (tempMotion.Type == type)
                    {
                        return tempMotion;
                    }
                }
                return null;
            }
            #endregion
        }
        #endregion

        #region [delegate & event]
        public delegate void EnableDelegate(MotionCommander commander);
        public delegate void DisableDelegate(MotionCommander commander);
        public delegate void ChangeCommandDelegate(Command command, Command lastCommand);
        public event EnableDelegate OnEnableDelegate;
        public event DisableDelegate OnDisableDelegate;
        public event ChangeCommandDelegate OnChangeCommandDelegate;
        #endregion

        #region [Fields]
        public List<Command> Commands = new List<Command>();
        [NonSerialized]
        [HideInInspector]
        public MotionMachine Machine;
        [NonSerialized]
        public Command LastCommand;
        [NonSerialized]
        public Command CurrentCommand;
        #endregion

        #region [Functions]
        public bool ExecuteCommand(CharacterCommand type)
        {
            if (type == CharacterCommand.CC_None) return false;
            Command cmd = TryGet(type);
            if (cmd == null) return false;
            LastCommand = CurrentCommand;
            CurrentCommand = cmd;
            if (LastCommand != null && LastCommand.ActiveStatus == true)
            {
                LastCommand.InActive(Machine, this, cmd);
            }
            CurrentCommand.Active(Machine, this, LastCommand);
            if (OnChangeCommandDelegate != null)
            {
                OnChangeCommandDelegate(CurrentCommand, LastCommand);
            }
            return true;
        }
        public Command TryGet(CharacterCommand command)
        {
            if (Commands == null || Commands.Count == 0) return null;
            for (int i = 0; i < Commands.Count; i++)
            {
                Command cmd = Commands[i];
                if (cmd == null) continue;
                if (cmd.Type == command) return cmd;
            }
            return null;
        }
        public bool Add(Command cmd)
        {
            if (cmd == null) return false;
            if (TryGet(cmd.Type) != null) return false;
            if (Commands == null) Commands = new List<Command>();
            Commands.Add(cmd); return true;
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
        #region monobehaviour
        void Awake()
        {
            Machine = GetMotionMachine();
            if (Commands != null && Commands.Count > 0)
            {
                for (int i = 0; i < Commands.Count; i++)
                {
                    Command cmd = Commands[i];
                    if (cmd == null) continue;
                    cmd.OnAwake(Machine, this);
                }
            }
        }
        void OnEnable()
        {
            if (OnEnableDelegate != null) OnEnableDelegate(this);
        }
        void OnDisable()
        {
            if (OnDisableDelegate != null) OnDisableDelegate(this);
        }
        void OnDestroy()
        {
            if (Commands != null && Commands.Count > 0)
            {
                for (int i = 0; i < Commands.Count; i++)
                {
                    Command cmd = Commands[i];
                    if (cmd == null) continue;
                    cmd.OnDestroy(Machine, this);
                }
            }
            OnChangeCommandDelegate = null;
        }
        #endregion
        #endregion
    }
}
