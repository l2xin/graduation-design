/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Example.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/5 15:18:20
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Air2000.Character
{
    public class Example : MonoBehaviour
    {
        public MotionMachine Animator;

        void Awake()
        {
            if (Animator == null)
            {
                Animator = GetComponent<MotionMachine>();
            }
        }
        void OnGUI()
        {
            if (Animator == null) return;
            if (GUI.Button(new Rect(10, 10, 150, 30), "Idle"))
            {
                Animator.ExecuteMotion(RoleMotionType.RMT_Idle);
            }
            if (GUI.Button(new Rect(10, 50, 150, 30), " Run"))
            {
                Animator.ExecuteMotion(RoleMotionType.RMT_Run);
            }
            if (GUI.Button(new Rect(10, 90, 150, 30), "Appear"))
            {
                Animator.ExecuteMotion(RoleMotionType.RMT_Appear);
            }
        }
    }
}
