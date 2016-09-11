/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: HitJudgePlugin.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/6/8 9:35:41
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
    [Serializable]
    public class HitJudgePlugin : MotionPlugin
    {
        #region hit
        [NonSerialized]
        public List<HitInfo> HitInfos = new List<HitInfo>();
        public class HitInfo
        {
            public Character Character;
            public float CurrentHitCount;
            public float LastHitTime = -1f;
            public void Update(float interval, int totalHitCount, Character attacker)
            {
                if (Character == null || attacker == null) return;
                if (CurrentHitCount > totalHitCount)
                {
                    return;
                }
                if (LastHitTime == -1f)
                {
                    //first hit
                    LastHitTime = Time.realtimeSinceStartup;
                    CurrentHitCount++;
                    attacker.Attack(Character);
                }
                else
                {
                    float deltaHitTime = Time.realtimeSinceStartup - LastHitTime;
                    if (deltaHitTime >= interval)
                    {
                        LastHitTime = Time.realtimeSinceStartup;
                        CurrentHitCount++;
                        attacker.Attack(Character);
                    }
                }
            }
        }
        public HitInfo TryGetHitInfo(Character ch)
        {
            if (HitInfos == null || HitInfos.Count == 0) return null;
            for (int i = 0; i < HitInfos.Count; i++)
            {
                HitInfo info = HitInfos[i];
                if (info == null) continue;
                if (info.Character = ch) return info;
            }
            return null;
        }
        public void AddHitInfo(Character ch)
        {
            if (HitInfos == null) HitInfos = new List<HitInfo>();
            HitInfo info = new HitInfo() { Character = ch };
        }
        public HitInfo GetHitInfo(Character ch)
        {
            if (HitInfos == null || HitInfos.Count == 0) return null;
            for (int i = 0; i < HitInfos.Count; i++)
            {
                HitInfo info = HitInfos[i];
                if (info == null) continue;
                if (info.Character = ch) return info;
            }
            return null;
        }
        #endregion
        public override string DisplayName
        {
            get
            {
                return "[HitJudge] ";
            }
        }
        public GameObject Root;
        public float Radius;
        public float Angles;
        public float HitInterval;
        public int SingleCharacterHitCount;
        public override void OnBegin(Motion motion)
        {
            base.OnBegin(motion);
            if (HitInfos == null) HitInfos = new List<HitInfo>();
            HitInfos.Clear();
            if (Root == null || Motion == null || Machine == null || Machine.Character == null)
            {
                CurrentStatus = Status.StopOnNextFrame;
                return;
            }
        }
        public override void OnUpdate(Motion motion)
        {
            base.OnUpdate(motion);
            List<Character> Characters = Character.GetAllCharacter();
            if (Characters != null && Characters.Count > 0)
            {
                for (int i = 0; i < Characters.Count; i++)
                {
                    Character ch = Characters[i];
                    if (ch == null || ch == Machine.Character)
                    {
                        continue;
                    }
                    float dis = Vector3.Distance(Root.transform.position, ch.transform.position);
                    float realDis = dis - ch.Radius;
                    if (realDis <= Radius)
                    {
                        if (Angles != 0 && Angles != 360)
                        {
                            Vector3 dir = ch.transform.position - Machine.Character.transform.position;
                            dir.Normalize();
                            float dot = Vector3.Dot(Machine.Character.transform.forward, dir);//get dot value
                            float cosValue = Mathf.Cos((Mathf.PI / 180) * (Angles / 2));
                            if (dot < cosValue)
                            {
                                return;
                            }
                        }
                        HitInfo info = TryGetHitInfo(ch);
                        if (info == null)
                        {
                            info = new HitInfo();
                            info.Character = ch;
                            if (HitInfos == null) HitInfos = new List<HitInfo>();
                            HitInfos.Add(info);
                        }
                        info.Update(HitInterval, SingleCharacterHitCount, Machine.Character);
                    }
                }
            }
        }
        public override void OnEnd(Motion motion)
        {
            base.OnEnd(motion);
        }
        public override void Clone(MotionPlugin obj)
        {
            base.Clone(obj);
        }
        public override void Destroy()
        {
            base.Destroy();
        }
#if UNITY_EDITOR
        public override void DisplayEditorView(Motion motion)
        {
            base.DisplayEditorView(motion);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Root", GUILayout.Width(140));
            Root = (GameObject)EditorGUILayout.ObjectField(Root, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Radius", GUILayout.Width(140));
            Radius = EditorGUILayout.FloatField(Radius);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Angles", GUILayout.Width(140));
            Angles = EditorGUILayout.FloatField(Angles);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("HitInterval", GUILayout.Width(140));
            HitInterval = EditorGUILayout.FloatField(HitInterval);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("SingleCharacterHitCount", GUILayout.Width(140));
            SingleCharacterHitCount = EditorGUILayout.IntField(SingleCharacterHitCount);
            GUILayout.EndHorizontal();
        }
#endif
    }
}
