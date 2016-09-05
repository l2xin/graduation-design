/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ScaleAction.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/13 14:41:12
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System;

namespace GTools.Animator
{
    [Serializable]
    public class ScaleAction : EaseAction
    {
        private Vector3 StartScale;
        public override string DisplayName
        {
            get
            {
                return "[Scale] " + IdentifyName;
            }
        }
        public override void OnClipInited(Clip clip)
        {
            base.OnClipInited(clip);
            if (AnimateObj != null)
            {
                StartScale = AnimateObj.transform.localScale;
            }
        }
        public override void OnBegin(Clip clip)
        {
            base.OnBegin(clip);
            if (AnimateObj == null) { m_Status = Status.Error; return; }
            if (Reset)
            {
                AnimateObj.transform.localScale = StartScale;
            }
            if (EaseTime <= 0)
            {
                if (Target == null)
                {
                    AnimateObj.transform.localScale = Vec3Target;
                }
                else
                {
                    AnimateObj.transform.localScale = Target.localScale;
                }
                m_Status = Status.StopOnNextFrame;
            }
            else
            {
                if (Target == null)
                {
                    iTween.ScaleTo(AnimateObj, iTween.Hash("scale", Vec3Target, "time", EaseTime, "easetype", EaseType));
                }
                else
                {
                    iTween.ScaleTo(AnimateObj, iTween.Hash("scale", Target.localScale, "time", EaseTime, "easetype", EaseType));
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
                iTween.Stop(AnimateObj);
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
                //    AnimateObj.transform.localScale = StartScale;
                //}
            }
        }
    }
}
