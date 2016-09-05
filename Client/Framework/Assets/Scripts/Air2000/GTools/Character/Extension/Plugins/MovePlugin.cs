/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: MovePlugin.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/6 9:44:24
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

namespace GTools.Character
{
    [Serializable]
    public class MovePlugin : MotionPlugin
    {
        public enum MoveType
        {
            Update,
            SpecificPoint,
            BeatBack,
            FlashMove,
        }
        public override string DisplayName
        {
            get
            {
                return "[Move] ";
            }
        }
        public float MoveSpeed;
        public float CutClipLength;
        public MoveType Type = MoveType.SpecificPoint;
        public iTween.EaseType EaseType = iTween.EaseType.linear;
        public override void OnBegin(Motion motion)
        {
            base.OnBegin(motion);
            if (Motion == null || Machine == null || Machine.Character == null)
            {
                CurrentStatus = Status.StopOnNextFrame;
                return;
            }
            if (Type == MoveType.SpecificPoint)
            {
                FaceToTarget();
                MoveToTarget();
            }
        }
        public override void OnUpdate(Motion motion)
        {
            base.OnUpdate(motion);
            if (Type == MoveType.Update || Type == MoveType.BeatBack || Type == MoveType.FlashMove)
            {
                Vector3 newPos = Vector3.zero;
                switch (Type)
                {
                    case MoveType.Update:
                        newPos = Machine.Character.pTargetPos;
                        break;
                    case MoveType.BeatBack:
                        newPos = Machine.Character.transform.TransformPoint(Vector3.back * Time.deltaTime * MoveSpeed);
                        break;
                    case MoveType.FlashMove:
                        newPos = Machine.Character.transform.TransformPoint(Vector3.forward * Time.deltaTime * MoveSpeed);
                        break;
                    default:
                        break;
                }
                iTween.MoveUpdate(Machine.Character.gameObject, newPos, Time.deltaTime);
            }
        }
        public override void OnEnd(Motion motion)
        {
            base.OnEnd(motion);
            if (Machine != null && Machine.Character != null)
            {
                iTween.Stop(Machine.Character.gameObject);
            }
        }
        private void MoveToTarget()
        {
            float travelTime = 2f;
            if (Motion.WrapMode == WrapMode.Loop)
            {
                travelTime = Vector3.Distance(Machine.Character.transform.position, Machine.Character.pTargetPos) / MoveSpeed;
            }
            else
            {
                AnimationClip clip = Motion.GetAnimationClip();
                if (clip != null)
                {
                    travelTime = clip.length - CutClipLength;
                }
            }
            iTween.MoveTo(Machine.Character.gameObject, iTween.Hash("easetype", EaseType, "time", travelTime, "position", Machine.Character.pTargetPos, "oncomplete", "OnTweenEnd", "oncompleteparams", this));
        }
        private void FaceToTarget()
        {
            float distance = Vector3.Distance(Machine.Character.pTargetPos, Machine.Character.transform.position);
            if (distance < 0.3f)
            {
                return;
            }
            Machine.Character.TurnToTargetDirection();
            //Vector3 dir = Machine.Character.pTargetPos - Machine.Character.transform.position;
            //dir.Normalize();
            //Quaternion rotation = Quaternion.LookRotation(dir);
            //Machine.Character.SetRotation(rotation);
            //Machine.Character.SetBodyTilt(Quaternion.identity);
        }
        public void OnTweenEnd()
        {
            //CurrentStatus = Status.StopOnNextFrame;
            if (Motion != null)
            {
                Motion.FinishMotion();
            }
        }
#if UNITY_EDITOR
        public override void DisplayEditorView(Motion motion)
        {
            base.DisplayEditorView(motion);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type", GUILayout.Width(100));
            Type = (MoveType)EditorGUILayout.EnumPopup(Type);
            GUILayout.EndHorizontal();

            if (motion != null)
            {
                if (motion.WrapMode == WrapMode.Loop)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("MoveSpeed", GUILayout.Width(140));
                    MoveSpeed = EditorGUILayout.FloatField(MoveSpeed);
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("CutClipLength", GUILayout.Width(140));
                    CutClipLength = EditorGUILayout.FloatField(CutClipLength);
                    GUILayout.EndHorizontal();

                    if (Type == MoveType.BeatBack)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("BeatBackTime", GUILayout.Width(140));
                        float tempTime = EditorGUILayout.FloatField(EndTime);
                        EndTime = tempTime + BeginTime;
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("BeatBackSpeed", GUILayout.Width(140));
                        MoveSpeed = EditorGUILayout.FloatField(MoveSpeed);
                        GUILayout.EndHorizontal();
                    }
                    else if (Type == MoveType.FlashMove)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("FlashMoveTime", GUILayout.Width(140));
                        float tempTime = EditorGUILayout.FloatField(EndTime);
                        EndTime = tempTime + BeginTime;
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("FlashMoveSpeed", GUILayout.Width(140));
                        MoveSpeed = EditorGUILayout.FloatField(MoveSpeed);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("MoveSpeed", GUILayout.Width(100));
                MoveSpeed = EditorGUILayout.FloatField(MoveSpeed);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("CutClipLength", GUILayout.Width(100));
                CutClipLength = EditorGUILayout.FloatField(CutClipLength);
                GUILayout.EndHorizontal();
            }


            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EaseType", GUILayout.Width(100));
            EaseType = (iTween.EaseType)EditorGUILayout.EnumPopup(EaseType);
            GUILayout.EndHorizontal();
        }
#endif
    }
}
