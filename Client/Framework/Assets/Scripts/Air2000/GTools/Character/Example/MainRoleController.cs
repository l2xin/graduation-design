/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: MainRoleController.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/8 10:35:40
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GTools.Character
{
    public class MainRoleController : MonoBehaviour
    {
        public Character Character;
        void Update()
        {
            if (Character == null)
            {
                return;
            }
            if (Input.GetKey(KeyCode.A))
            {
                float turn = -0.1f;
                float x = Mathf.Sin(turn);
                float z = Mathf.Cos(turn);

                Vector3 newdir = new Vector3(x, 0.0f, z);
                newdir = Character.gameObject.transform.TransformDirection(newdir);
                Quaternion rotation = Quaternion.LookRotation(newdir);
                Character.transform.rotation = rotation;

            }
            else if (Input.GetKey(KeyCode.D))
            {
                float turn = 0.1f;
                float x = Mathf.Sin(turn);
                float z = Mathf.Cos(turn);

                Vector3 newdir = new Vector3(x, 0.0f, z);
                newdir = Character.gameObject.transform.TransformDirection(newdir);
                Quaternion rotation = Quaternion.LookRotation(newdir);
                Character.transform.rotation = rotation;
            }
            else if (Input.GetKey(KeyCode.S))
            {
            }
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 newTarget = Character.transform.TransformPoint(Vector3.forward);
                Character.pTargetPos = newTarget;
                Character.MotionMachine.ExecuteMotion(RoleMotionType.RMT_Run);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                Character.MotionMachine.ExecuteMotion(RoleMotionType.RMT_Idle);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                Character.MotionMachine.ExecuteMotion(RoleMotionType.RMT_Attack);
            }
        }
    }
}
