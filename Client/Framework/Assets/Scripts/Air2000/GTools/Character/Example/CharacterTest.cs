/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: CharacterTest.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/3 19:04:27
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameLogic;

namespace Air2000.Character
{
    public class CharacterTest : MonoBehaviour
    {
        public NewGameCharacter Character;
        public static bool Paused = false;
        void Awake()
        {
            if (Character == null)
            {
                Character = GetComponent<NewGameCharacter>();
            }
            if (Character != null)
            {
                Character.Init(Color.black);
            }
        }
        void RunEnd(MotionCommander.Command cmd, Motion motion)
        {
            cmd.OnFinishDelegate -= RunEnd;
            //Debug.LogError("Run End");
        }
        void JumpEnd(MotionCommander.Command cmd, Motion motion)
        {
            cmd.OnFinishDelegate -= JumpEnd;
            //Character.pTargetPos += new Vector3(0, 0, 3);
            //Character.ExecuteCommand(CharacterCommand.CC_WalkToPoint);
        }
        void OnGUI()
        {
            //if (Character == null || Character.MotionMachine == null) return;
            //if (GUI.Button(new Rect(10, 10, 150, 30), "Run to point"))
            //{
            //    if (StandaloneDebug.Instance != null && StandaloneDebug.Instance.CurrentGrid != null)
            //    {
            //        Character.GridController.To = StandaloneDebug.Instance.CurrentGrid;
            //        Character.pTargetPos = StandaloneDebug.Instance.CurrentGrid.transform.position;
            //        Character.ExecuteCommand(CharacterCommand.CC_WalkToPoint);
            //        MotionCommander.Command command = Character.MotionCommander.TryGet(CharacterCommand.CC_WalkToPoint);
            //        if (command != null)
            //        {
            //            command.OnFinishDelegate += RunEnd;
            //        }
            //    }
            //}
            //if (GUI.Button(new Rect(10, 50, 150, 30), "Jump to point"))
            //{
            //    if (StandaloneDebug.Instance != null && StandaloneDebug.Instance.CurrentGrid != null)
            //    {
            //        Character.GridController.To = StandaloneDebug.Instance.CurrentGrid;
            //        Character.pTargetPos = StandaloneDebug.Instance.CurrentGrid.transform.position;
            //        MotionCommander.Command command = Character.MotionCommander.TryGet(CharacterCommand.CC_JumpToPoint);
            //        if (command != null)
            //        {
            //            command.OnFinishDelegate += JumpEnd;
            //        }
            //        Character.ExecuteCommand(CharacterCommand.CC_JumpToPoint);
            //    }
            //}
            //if (GUI.Button(new Rect(10, 90, 150, 30), "Fly to point"))
            //{
            //    if (StandaloneDebug.Instance != null && StandaloneDebug.Instance.CurrentGrid != null)
            //    {
            //        Character.GridController.To = StandaloneDebug.Instance.CurrentGrid;
            //        Character.pTargetPos = StandaloneDebug.Instance.CurrentGrid.transform.position;
            //        Character.ExecuteCommand(CharacterCommand.CC_FlyToPoint);
            //    }
            //}
            //if (GUI.Button(new Rect(10, 130, 150, 30), "Idle"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_Stop);
            //}
            //if (GUI.Button(new Rect(10, 170, 150, 30), "Appear"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_Appear);
            //    //Character.pMotionMachine.ExecuteMotion(RoleMotionType.RMT_Appear);
            //}
            //if (GUI.Button(new Rect(10, 210, 150, 30), "Attack"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_Attack);
            //}
            //if (GUI.Button(new Rect(10, 250, 150, 30), "BeAttack"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_BeAttack);
            //}
            //if (GUI.Button(new Rect(10, 290, 150, 30), "Reach Final Pos"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_ReachFinalPos);
            //}
            //if (GUI.Button(new Rect(10, 330, 150, 30), "Reach Final Pos2"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_ReachFinalPos2);
            //}
            //if (GUI.Button(new Rect(10, 370, 150, 30), "Victory"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_Victory);
            //}
            //if (GUI.Button(new Rect(10, 410, 150, 30), "Fail"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_Fail);
            //}
            //if (GUI.Button(new Rect(10, 450, 150, 30), "Generate Move Path"))
            //{
            //    if (Character.BehaviourPerformer == null) return;
            //    BehaviourQueue Q = new BehaviourQueue(Character);
            //    //Vector3 jumpTarget = Character.transform.TransformPoint(new Vector3(0, 0, 1.5f));
            //    //JumpBehaviour jump = new JumpBehaviour(jumpTarget, null, null);
            //    //Q.Add(jump);

            //    ////Quaternion turnTarget = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 180, 0));
            //    ////TurnBehaviour turn = new TurnBehaviour(turnTarget, null, null);
            //    ////Q.Add(turn);
            //    //WalkBehaviour walk1 = new WalkBehaviour(jumpTarget, null, null);
            //    //Q.Add(walk1);

            //    //Vector3 walkTarget = Character.transform.TransformPoint(new Vector3(0, 0, -1.5f));
            //    //WalkBehaviour walk2 = new WalkBehaviour(walkTarget, null, null);
            //    //Q.Add(walk2);

            //    GridControl startGrid = GameObject.Find("Scene/Chessboard/08.12").GetComponent<GridControl>();
            //    GridControl endGrid = GameObject.Find("Scene/Chessboard/08.08").GetComponent<GridControl>();
            //    GridControl thirdGrid = GameObject.Find("Scene/Chessboard/08.10").GetComponent<GridControl>();
            //    Q.StartGrid = startGrid.Grid;
            //    Q.EndGrid = thirdGrid.Grid;

            //    //Vector3 flyTarget = Character.transform.TransformPoint(new Vector3(0, 0, 3f));
            //    //JumpBehaviour fly = new JumpBehaviour(endGrid.transform.position, startGrid.Grid, endGrid.Grid);
            //    //Q.Add(fly);

            //    //Quaternion turnTarget = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 180, 0));
            //    //TurnBehaviour turn = new TurnBehaviour(turnTarget, null, null);
            //    //Q.Add(turn);

            //    //Vector3 jumpTarget = Character.transform.TransformPoint(new Vector3(0, 0, 1.5f));
            //    //JumpBehaviour jump = new JumpBehaviour(jumpTarget, null, null);
            //    //Q.Add(jump);
            //    Character.SetPosition(startGrid.transform.position);

            //    WalkBehaviour walk1 = new WalkBehaviour(endGrid.transform.position, startGrid.Grid, endGrid.Grid);
            //    Q.Add(walk1);

            //    WalkBehaviour walk2 = new WalkBehaviour(thirdGrid.transform.position, endGrid.Grid, thirdGrid.Grid);
            //    Q.Add(walk2);

            //    Character.BehaviourPerformer.Play(Q);
            //    Paused = false;
            //    //Character.BehaviourPerformer.ClearPathQueue();
            //    //Character.SetPosition(Vector3.zero);
            //}

            //if (GUI.Button(new Rect(10, 490, 150, 30), "Reset position"))
            //{
            //    Paused = true;
            //    Character.BehaviourPerformer.ClearPathQueue();
            //    Character.Appear(Vector3.zero, GTools.Character.CharacterCommand.CC_Stop);
            //    Character.AdjustRotation(null);
            //}
            //if (GUI.Button(new Rect(10, 530, 150, 30), "Display"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_Display);
            //}
            //if (GUI.Button(new Rect(170, 530, 150, 30), "Inactive"))
            //{
            //    Character.gameObject.SetActive(false);
            //    //Character.ExecuteCommand(CharacterCommand.CC_Display);
            //}
            //if (GUI.Button(new Rect(330, 530, 150, 30), "Active"))
            //{
            //    Character.gameObject.SetActive(true);
            //    //Character.ExecuteCommand(CharacterCommand.CC_Display);
            //}
            //if (GUI.Button(new Rect(10, 570, 150, 30), "Overlap"))
            //{
            //    Character.ExecuteCommand(CharacterCommand.CC_Overlap);
            //}
            //if (GUI.Button(new Rect(10, 610, 150, 30), "EnableStencilTest"))
            //{
            //    Character.EnableStencilTest();
            //}
            //if (GUI.Button(new Rect(10, 650, 150, 30), "DisableStencilTest"))
            //{
            //    Character.DisableStencilTest();
            //}
        }
    }
}
