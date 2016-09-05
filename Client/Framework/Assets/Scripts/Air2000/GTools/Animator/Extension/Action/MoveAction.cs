/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: MoveAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/13 14:23:01
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System;

namespace GTools.Animator
{
    [Serializable]
    public class MoveAction : EaseAction
    {
        [HideInInspector]
        [NonSerialized]
        public Vector3 StartPosition;
        public override string DisplayName
        {
            get
            {
                return "[Move] " + IdentifyName;
            }
        }
        public override void OnClipInited(Clip clip)
        {
            base.OnClipInited(clip);
            if (AnimateObj != null)
            {
                StartPosition = AnimateObj.transform.position;
            }
        }
        public override void OnClipBegin(Clip clip)
        {
            base.OnClipBegin(clip);
        }
        public override void OnBegin(Clip clip)
        {
            base.OnBegin(clip);
            if (AnimateObj == null) { m_Status = Status.Error; return; }
            if (Reset)
            {
                AnimateObj.transform.position = StartPosition;
            }
            StartPosition = AnimateObj.transform.position;
            if (EaseTime <= 0)
            {
                if (Target == null)
                {
                    AnimateObj.transform.position = Vec3Target;
                }
                else
                {
                    AnimateObj.transform.position = Target.position;
                }
                m_Status = Status.StopOnNextFrame;
            }
            else
            {
                if (Target == null)
                {
                    iTween.MoveTo(AnimateObj, iTween.Hash("position", Vec3Target, "time", EaseTime, "easetype", EaseType));
                }
                else
                {
                    iTween.MoveTo(AnimateObj, iTween.Hash("position", Target.position, "time", EaseTime, "easetype", EaseType));
                }
            }
            //switch (ObjStatusOperation)
            //{
            //    case GameObjectStatusOperation.ActiveOnBegin_InActiveOnEnd:
            //        if (AnimateObj.activeSelf == false)
            //        {
            //            AnimateObj.SetActive(true);
            //        }
            //        break;
            //    case GameObjectStatusOperation.ActiveOnBegin:
            //        if (AnimateObj.activeSelf == false)
            //        {
            //            AnimateObj.SetActive(true);
            //        }
            //        break;
            //    case GameObjectStatusOperation.IgnoreActiveStatus:
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
                //    case GameObjectStatusOperation.ActiveOnBegin_InActiveOnEnd:
                //        if (AnimateObj.activeSelf == true)
                //        {
                //            AnimateObj.SetActive(false);
                //        }
                //        break;
                //    case GameObjectStatusOperation.ActiveOnBegin:
                //        break;
                //    case GameObjectStatusOperation.IgnoreActiveStatus:
                //        break;
                //    default:
                //        break;
                //}
                //if (Reset)
                //{
                //    AnimateObj.transform.position = StartPosition;
                //}
            }
        }
    }
}
