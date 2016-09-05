/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Utility.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/13 17:45:21
            // Modify History:
            //
//----------------------------------------------------------------*/
//#define OPEN_GTANIMATOR_LOG_ALL
//#define OPEN_GTANIMATOR_LOG_DEBUG
//#define OPEN_GTANIMATOR_LOG_WARNING
//#define OPEN_GTANIMATOR_LOG_ERROR

using System;
using UnityEngine;

namespace GTools.Animator
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
#if OPEN_GTANIMATOR_LOG_ALL
            Debug.LogError("[GTAnimator] " + msg); return;
#elif OPEN_GTANIMATOR_LOG_ERROR
            Debug.LogError("[GTAnimator] " + msg);
#endif
        }
        public static void LogDebug(string msg)
        {
#if OPEN_GTANIMATOR_LOG_ALL
            Debug.Log("[GTAnimator] " + msg); return;
#elif OPEN_GTANIMATOR_LOG_DEBUG
            Debug.Log("[GTAnimator] " + msg);
#endif
        }
        public static void LogWarning(string msg)
        {
#if OPEN_GTANIMATOR_LOG_ALL
            Debug.LogWarning("[GTAnimator] " + msg); return;
#elif OPEN_GTANIMATOR_LOG_WARNING
            Debug.LogWarning("[GTAnimator] " + msg);
#endif
        }
    }

}
