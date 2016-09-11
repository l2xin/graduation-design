/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTRotateAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/13 14:40:56
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Air2000.Animator
{
    [Serializable]
    public class RotateAction : EaseAction
    {
        private Vector3 StartEulerAngles;
        public override string DisplayName
        {
            get
            {
                return "[Rotate] " + IdentifyName;
            }
        }
        public override void OnClipInited(Clip clip)
        {
            base.OnClipInited(clip);
            if (AnimateObj != null)
            {
                StartEulerAngles = AnimateObj.transform.eulerAngles;
            }
        }
        public override void OnBegin(Clip clip)
        {
            base.OnBegin(clip);
            if (AnimateObj == null) { m_Status = Status.Error; return; }
            if (Reset)
            {
                AnimateObj.transform.eulerAngles = StartEulerAngles;
            }
            if (EaseTime <= 0)
            {
                if (Target == null)
                {
                    AnimateObj.transform.eulerAngles = Vec3Target;
                }
                else
                {
                    AnimateObj.transform.eulerAngles = Target.eulerAngles;
                }
                m_Status = Status.StopOnNextFrame;
            }
            else
            {
                if (Target == null)
                {
                    iTween.RotateTo(AnimateObj, iTween.Hash("rotation", Vec3Target, "time", EaseTime, "easetype", EaseType));
                }
                else
                {
                    iTween.RotateTo(AnimateObj, iTween.Hash("rotation", Target.eulerAngles, "time", EaseTime, "easetype", EaseType));
                }
            }
            //switch (ObjStatusOperation)
            //{
            //    case GameObjectActiveOperation.ActiveOnBegin_InActiveOnEnd:
            //        if (AnimateObj.activeSelf == false)
            //        {
            //            AnimateObj.SetActive(true);
            //        }
            //        break;
            //    case GameObjectActiveOperation.ActiveOnBegin:
            //        if (AnimateObj.activeSelf == false)
            //        {
            //            AnimateObj.SetActive(true);
            //        }
            //        break;
            //    case GameObjectActiveOperation.IgnoreActiveStatus:
            //        break;
            //    default:
            //        break;
            //}
        }

        public override void OnEnd(Clip clip)
        {
            base.OnEnd(clip);
            if (AnimateObj != null)
            {
                //switch (ObjStatusOperation)
                //{
                //    case GameObjectActiveOperation.ActiveOnBegin_InActiveOnEnd:
                //        if (AnimateObj.activeSelf == true)
                //        {
                //            AnimateObj.SetActive(false);
                //        }
                //        break;
                //    case GameObjectActiveOperation.ActiveOnBegin:
                //        break;
                //    case GameObjectActiveOperation.IgnoreActiveStatus:
                //        break;
                //    default:
                //        break;
                //}
                //if (Reset)
                //{
                //    AnimateObj.transform.eulerAngles = StartEulerAngles;
                //}
            }
        }
    }
}
