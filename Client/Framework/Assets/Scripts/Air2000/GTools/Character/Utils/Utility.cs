/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Utility.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/5 13:48:07
            // Modify History:
            //
//----------------------------------------------------------------*/
//#define OPEN_MOTIONANIMATOR_LOG_ALL
//#define OPEN_MOTIONANIMATOR_LOG_DEBUG
//#define OPEN_MOTIONANIMATOR_LOG_WARNING
//#define OPEN_MOTIONANIMATOR_LOG_ERROR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GTools.Character
{
    public class Utility
    {
        public static string GetTypeNameWithoutNamespcae(string varTypeName)
        {
            if (string.IsNullOrEmpty(varTypeName))
            {
                return string.Empty;
            }
            string[] pathArray = varTypeName.Split(new char[] { '.' });
            if (pathArray == null || pathArray.Length == 0)
            {
                return string.Empty;
            }
            return pathArray[pathArray.Length - 1];
        }

        public static string GenerateID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToString(buffer, 0);
        }

        public static void LogError(string msg)
        {
#if OPEN_MOTIONANIMATOR_LOG_ALL
            Debug.LogError("[GTAnimator] " + msg); return;
#elif OPEN_MOTIONANIMATOR_LOG_ERROR
            Debug.LogError("[GTAnimator] " + msg);
#endif
        }
        public static void LogDebug(string msg)
        {
#if OPEN_MOTIONANIMATOR_LOG_ALL
            Debug.Log("[GTAnimator] " + msg); return;
#elif OPEN_MOTIONANIMATOR_LOG_DEBUG
            Debug.Log("[GTAnimator] " + msg);
#endif
        }
        public static void LogWarning(string msg)
        {
#if OPEN_MOTIONANIMATOR_LOG_ALL
            Debug.LogWarning("[GTAnimator] " + msg); return;
#elif OPEN_MOTIONANIMATOR_LOG_WARNING
            Debug.LogWarning("[GTAnimator] " + msg);
#endif
        }

        public static RoleMotionType CC_RMT(CharacterCommand command)
        {
            if (command == CharacterCommand.CC_None)
            {
                return RoleMotionType.RMT_Idle;
            }
            string[] strArray = command.ToString().Split(new char[] { '_' });
            if (strArray == null || strArray.Length < 2)
            {
                return RoleMotionType.RMT_Idle;
            }
            string rmtStr = "RMT_" + strArray[1];
            try
            {
                RoleMotionType rmt = (RoleMotionType)System.Enum.Parse(typeof(RoleMotionType), rmtStr);
                return rmt;
            }
            catch
            {
                return RoleMotionType.RMT_Idle;
            }
        }
        public static CharacterCommand RMT_CC(RoleMotionType type)
        {
            if (type == RoleMotionType.RMT_Idle)
            {
                return CharacterCommand.CC_Stop;
            }
            else if (type == RoleMotionType.RMT_Run)
            {
                return CharacterCommand.CC_WalkToPoint;
            }
            else if (type == RoleMotionType.RMT_Jump)
            {
                return CharacterCommand.CC_JumpToPoint;
            }
            else if (type == RoleMotionType.RMT_Fly)
            {
                return CharacterCommand.CC_FlyToPoint;
            }
            string[] strArray = type.ToString().Split(new char[] { '_' });
            if (strArray == null || strArray.Length < 2)
            {
                return CharacterCommand.CC_Stop;
            }
            string rmtStr = "CC_" + strArray[1];
            try
            {
                CharacterCommand cc = (CharacterCommand)System.Enum.Parse(typeof(CharacterCommand), rmtStr);
                return cc;
            }
            catch
            {
                return CharacterCommand.CC_Stop;
            }
        }
        public static string GenerateRootPath(Transform targetTransform, Transform currentTransform)
        {
            string path = string.Empty;
            path = InternalGenerateRootPath(targetTransform, currentTransform, path);
            return path;
        }
        private static string InternalGenerateRootPath(Transform targetTransform, Transform currentTransform, string path)
        {
            if (targetTransform == null || currentTransform == null)
            {
                return string.Empty;
            }
            if (targetTransform != currentTransform)
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = currentTransform.name;
                }
                else
                {
                    path = currentTransform.name + "/" + path;
                }
                return InternalGenerateRootPath(targetTransform, currentTransform.parent, path);
            }
            else
            {
                return path;
            }
        }
    }
}
