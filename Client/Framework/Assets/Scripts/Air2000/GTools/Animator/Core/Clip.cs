/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Clip.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/13 14:04:14
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Air2000.Animator
{
    public enum GameObjectActiveOperation
    {
        DontOperate,
        AlwaysChangeStatus,
        OnlyActiveOnBegin,
    }

    [Serializable]
    public class Clip
    {
        #region [delegate & event]
        public delegate void BeginDelegate(Clip clip);
        public delegate void UpdateDelegate(Clip clip);
        public delegate void EndDelegate(Clip clip);
        public event BeginDelegate OnBeginDelegate;
        public event UpdateDelegate OnUpdateDelegate;
        public event EndDelegate OnEndDelegate;
        #endregion

        #region [enum]Status
        public enum Status
        {
            UnAction,
            Action,
            Updating,
            WaittingForLastActionEnd,
            DisableOnNextFrame,
            Error,
        }
        #endregion

        [HideInInspector]
        [NonSerialized]
        public Status m_Status = Status.UnAction;
        public string Name;
        public float BeginTime;
        public float EndTime;
        public GameObjectActiveOperation ActiveObjOperation = GameObjectActiveOperation.DontOperate;
        public GameObject AnimateObj;

        #region All kinds of actions
        [HideInInspector]
        public List<RotateAction> RotateActions = new List<RotateAction>();
        [HideInInspector]
        public List<MoveAction> MoveActions = new List<MoveAction>();
        [HideInInspector]
        public List<ScaleAction> ScaleActions = new List<ScaleAction>();
        [HideInInspector]
        public List<UITweenPositionAction> UITweenPositionActions = new List<UITweenPositionAction>();
        [HideInInspector]
        public List<UITweenRotationAction> UITweenRotationActions = new List<UITweenRotationAction>();
        [HideInInspector]
        public List<UITweenScaleAction> UITweenScaleActions = new List<UITweenScaleAction>();
        [HideInInspector]
        public List<ActiveObjAction> ActiveObjActions = new List<ActiveObjAction>();
        [HideInInspector]
        public List<PlayEffectAction> PlayEffectActions = new List<PlayEffectAction>();
        [HideInInspector]
        public List<PlayAnimAction> PlayAnimActions = new List<PlayAnimAction>();
        [HideInInspector]
        public List<PlaySoundAction> PlaySoundActions = new List<PlaySoundAction>();
        #endregion

        [NonSerialized]
        public List<ClipAction> Actions = new List<ClipAction>();
        [HideInInspector]
        [NonSerialized]
        public List<ClipAction> WaittingProcessActions = new List<ClipAction>();
        [HideInInspector]
        [NonSerialized]
        public List<ClipAction> ActiveActions = new List<ClipAction>();
        public virtual void OnAnimatorInited(GTAnimator animator)
        {
            GetAllAction();
            if (Actions != null && Actions.Count > 0)
            {
                for (int i = 0; i < Actions.Count; i++)
                {
                    ClipAction action = Actions[i];
                    if (action == null) continue;
                    action.OnClipInited(this);
                }
            }
        }
        public virtual void OnAnimatorBegin(GTAnimator animator)
        {
        }
        public virtual void OnBegin(GTAnimator animator)
        {
            GetAllAction();
            WaittingProcessActions = new List<ClipAction>(Actions);
            switch (ActiveObjOperation)
            {
                case GameObjectActiveOperation.AlwaysChangeStatus:
                    if (AnimateObj.activeSelf == false)
                    {
                        AnimateObj.SetActive(true);
                    }
                    break;
                case GameObjectActiveOperation.OnlyActiveOnBegin:
                    if (AnimateObj.activeSelf == false)
                    {
                        AnimateObj.SetActive(true);
                    }
                    break;
                case GameObjectActiveOperation.DontOperate:
                    break;
                default:
                    break;
            }
            if (OnBeginDelegate != null)
            {
                OnBeginDelegate(this);
            }
        }
        public virtual void OnUpdate(GTAnimator animator)
        {
            if (animator == null) return;
            if (OnUpdateDelegate != null)
            {
                OnUpdateDelegate(this);
            }
            if (ActiveActions != null && ActiveActions.Count > 0)
            {
                for (int i = 0; i < ActiveActions.Count; i++)
                {
                    ClipAction action = ActiveActions[i];
                    if (action == null)
                    {
                        ActiveActions.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (action.EndTime <= animator.CurrentTime)
                    {
                        action.OnEnd(this);
                        ActiveActions.Remove(action);
                        i--;
                    }
                    else if (action.m_Status == ClipAction.Status.Error || action.m_Status == ClipAction.Status.StopOnNextFrame)
                    {
                        action.OnEnd(this);
                        ActiveActions.Remove(action);
                        i--;
                    }
                    else
                    {
                        action.OnUpdate(this);
                    }
                }
            }
            if (WaittingProcessActions != null && WaittingProcessActions.Count > 0)
            {
                for (int i = 0; i < WaittingProcessActions.Count; i++)
                {
                    ClipAction action = WaittingProcessActions[i];
                    if (action == null)
                    {
                        WaittingProcessActions.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (action.BeginTime <= animator.CurrentTime && action.m_Status == ClipAction.Status.UnAction)
                    {
                        action.OnBegin(this);
                        ActiveActions.Add(action);
                        WaittingProcessActions.Remove(action);
                        i--;
                    }
                }
            }
        }
        public virtual void OnEnd(GTAnimator animator)
        {
            if (ActiveActions != null || ActiveActions.Count > 0)
            {
                for (int i = 0; i < ActiveActions.Count; i++)
                {
                    ClipAction action = ActiveActions[i];
                    if (action == null) continue;
                    action.OnEnd(this);
                }
            }
            if (AnimateObj != null)
            {
                switch (ActiveObjOperation)
                {
                    case GameObjectActiveOperation.AlwaysChangeStatus:
                        if (AnimateObj.activeSelf == true)
                        {
                            AnimateObj.SetActive(false);
                        }
                        break;
                    case GameObjectActiveOperation.OnlyActiveOnBegin:
                        break;
                    case GameObjectActiveOperation.DontOperate:
                        break;
                    default:
                        break;
                }
            }
            if (OnEndDelegate != null)
            {
                OnEndDelegate(this);
            }
            m_Status = Status.UnAction;
        }
        public virtual void OnAnimatorEnd(GTAnimator animator) { }
        public ClipAction AddAction(Type type)
        {
            if (type == null) return null;
            ClipAction action = null;
            if (type == typeof(MoveAction))
            {
                action = new MoveAction();
                MoveActions.Add(action as MoveAction);
            }
            else if (type == typeof(RotateAction))
            {
                action = new RotateAction();
                RotateActions.Add(action as RotateAction);
            }
            else if (type == typeof(ScaleAction))
            {
                action = new ScaleAction();
                ScaleActions.Add(action as ScaleAction);
            }
            else if (type == typeof(UITweenPositionAction))
            {
                action = new UITweenPositionAction();
                UITweenPositionActions.Add(action as UITweenPositionAction);
            }
            else if (type == typeof(UITweenRotationAction))
            {
                action = new UITweenRotationAction();
                UITweenRotationActions.Add(action as UITweenRotationAction);
            }
            else if (type == typeof(UITweenScaleAction))
            {
                action = new UITweenScaleAction();
                UITweenScaleActions.Add(action as UITweenScaleAction);
            }

            else if (type == typeof(ActiveObjAction))
            {
                action = new ActiveObjAction();
                ActiveObjActions.Add(action as ActiveObjAction);
            }
            else if (type == typeof(PlayEffectAction))
            {
                action = new PlayEffectAction();
                PlayEffectActions.Add(action as PlayEffectAction);
            }
            else if (type == typeof(PlayAnimAction))
            {
                action = new PlayAnimAction();
                PlayAnimActions.Add(action as PlayAnimAction);
            }
            else if (type == typeof(PlaySoundAction))
            {
                action = new PlaySoundAction();
                PlaySoundActions.Add(action as PlaySoundAction);
            }
            return action;
        }
        public ClipAction AddAction(ClipAction action)
        {
            if (action == null) return null;
            if (action.GetType() == typeof(MoveAction))
            {
                MoveActions.Add(action as MoveAction);
            }
            else if (action.GetType() == typeof(RotateAction))
            {
                RotateActions.Add(action as RotateAction);
            }
            else if (action.GetType() == typeof(ScaleAction))
            {
                ScaleActions.Add(action as ScaleAction);
            }
            else if (action.GetType() == typeof(UITweenPositionAction))
            {
                UITweenPositionActions.Add(action as UITweenPositionAction);
            }
            else if (action.GetType() == typeof(UITweenRotationAction))
            {
                UITweenRotationActions.Add(action as UITweenRotationAction);
            }
            else if (action.GetType() == typeof(UITweenPositionAction))
            {
                UITweenPositionActions.Add(action as UITweenPositionAction);
            }
            else if (action.GetType() == typeof(ActiveObjAction))
            {
                ActiveObjActions.Add(action as ActiveObjAction);
            }
            else if (action.GetType() == typeof(PlayEffectAction))
            {
                PlayEffectActions.Add(action as PlayEffectAction);
            }
            else if (action.GetType() == typeof(PlayAnimAction))
            {
                PlayAnimActions.Add(action as PlayAnimAction);
            }
            else if (action.GetType() == typeof(PlaySoundAction))
            {
                PlaySoundActions.Add(action as PlaySoundAction);
            }
            return action;
        }
        public void RemoveAction(ClipAction action)
        {
            if (action == null) return;
            if (action.GetType() == typeof(MoveAction))
            {
                MoveActions.Remove(action as MoveAction);
            }
            else if (action.GetType() == typeof(RotateAction))
            {
                RotateActions.Remove(action as RotateAction);
            }
            else if (action.GetType() == typeof(ScaleAction))
            {
                ScaleActions.Remove(action as ScaleAction);
            }
            else if (action.GetType() == typeof(UITweenPositionAction))
            {
                UITweenPositionActions.Remove(action as UITweenPositionAction);
            }
            else if (action.GetType() == typeof(UITweenRotationAction))
            {
                UITweenRotationActions.Remove(action as UITweenRotationAction);
            }
            else if (action.GetType() == typeof(UITweenScaleAction))
            {
                UITweenScaleActions.Remove(action as UITweenScaleAction);
            }
            else if (action.GetType() == typeof(ActiveObjAction))
            {
                ActiveObjActions.Remove(action as ActiveObjAction);
            }
            else if (action.GetType() == typeof(PlayEffectAction))
            {
                PlayEffectActions.Remove(action as PlayEffectAction);
            }
            else if (action.GetType() == typeof(PlayAnimAction))
            {
                PlayAnimActions.Remove(action as PlayAnimAction);
            }
            else if (action.GetType() == typeof(PlaySoundAction))
            {
                PlaySoundActions.Remove(action as PlaySoundAction);
            }
        }
        public void ClearAction()
        {
            if (RotateActions != null) RotateActions.Clear();
            if (MoveActions != null) MoveActions.Clear();
            if (ScaleActions != null) ScaleActions.Clear();
            if (UITweenPositionActions != null) UITweenPositionActions.Clear();
            if (UITweenRotationActions != null) UITweenRotationActions.Clear();
            if (UITweenScaleActions != null) UITweenScaleActions.Clear();
            if (ActiveObjActions != null) ActiveActions.Clear();
            if (PlayEffectActions != null) PlayEffectActions.Clear();
            if (PlayAnimActions != null) PlayAnimActions.Clear();
            if (PlaySoundActions != null) PlaySoundActions.Clear();

        }
        public List<ClipAction> GetAllAction()
        {
            if (Actions == null) Actions = new List<ClipAction>();
            Actions.Clear();
            if (RotateActions != null && RotateActions.Count > 0)
            {
                for (int i = 0; i < RotateActions.Count; i++)
                {
                    ClipAction action = RotateActions[i];
                    if (action == null)
                    {
                        RotateActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            if (MoveActions != null && MoveActions.Count > 0)
            {
                for (int i = 0; i < MoveActions.Count; i++)
                {
                    ClipAction action = MoveActions[i];
                    if (action == null)
                    {
                        MoveActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            if (ScaleActions != null && ScaleActions.Count > 0)
            {
                for (int i = 0; i < ScaleActions.Count; i++)
                {
                    ClipAction action = ScaleActions[i];
                    if (action == null)
                    {
                        ScaleActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            if (UITweenPositionActions != null && UITweenPositionActions.Count > 0)
            {
                for (int i = 0; i < UITweenPositionActions.Count; i++)
                {
                    ClipAction action = UITweenPositionActions[i];
                    if (action == null)
                    {
                        UITweenPositionActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            if (UITweenRotationActions != null && UITweenRotationActions.Count > 0)
            {
                for (int i = 0; i < UITweenRotationActions.Count; i++)
                {
                    ClipAction action = UITweenRotationActions[i];
                    if (action == null)
                    {
                        UITweenRotationActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            if (UITweenScaleActions != null && UITweenScaleActions.Count > 0)
            {
                for (int i = 0; i < UITweenScaleActions.Count; i++)
                {
                    ClipAction action = UITweenScaleActions[i];
                    if (action == null)
                    {
                        UITweenScaleActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            if (ActiveObjActions != null && ActiveObjActions.Count > 0)
            {
                for (int i = 0; i < ActiveObjActions.Count; i++)
                {
                    ActiveObjAction action = ActiveObjActions[i];
                    if (action == null)
                    {
                        ActiveObjActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            if (PlayEffectActions != null && PlayEffectActions.Count > 0)
            {
                for (int i = 0; i < PlayEffectActions.Count; i++)
                {
                    PlayEffectAction action = PlayEffectActions[i];
                    if (action == null)
                    {
                        PlayEffectActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            if (PlayAnimActions != null && PlayAnimActions.Count > 0)
            {
                for (int i = 0; i < PlayAnimActions.Count; i++)
                {
                    PlayAnimAction action = PlayAnimActions[i];
                    if (action == null)
                    {
                        PlayAnimActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            if (PlaySoundActions != null && PlaySoundActions.Count > 0)
            {
                for (int i = 0; i < PlaySoundActions.Count; i++)
                {
                    PlaySoundAction action = PlaySoundActions[i];
                    if (action == null)
                    {
                        PlaySoundActions.RemoveAt(i); i--; continue;
                    }
                    Actions.Add(action);
                }
            }
            return Actions;
        }
        public ClipAction GetAction(string actionName, Type type)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                Utility.LogError("GetAction error caused by null action name"); return null;
            }
            if (type == typeof(MoveAction))
            {
                for (int i = 0; i < MoveActions.Count; i++)
                {
                    ClipAction action = MoveActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            else if (type == typeof(RotateAction))
            {
                for (int i = 0; i < RotateActions.Count; i++)
                {
                    ClipAction action = RotateActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            else if (type == typeof(ScaleAction))
            {
                for (int i = 0; i < ScaleActions.Count; i++)
                {
                    ClipAction action = ScaleActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            else if (type == typeof(UITweenPositionAction))
            {
                for (int i = 0; i < UITweenPositionActions.Count; i++)
                {
                    ClipAction action = UITweenPositionActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            else if (type == typeof(UITweenRotationAction))
            {
                for (int i = 0; i < UITweenRotationActions.Count; i++)
                {
                    ClipAction action = UITweenRotationActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            else if (type == typeof(UITweenScaleAction))
            {
                for (int i = 0; i < UITweenScaleActions.Count; i++)
                {
                    ClipAction action = UITweenScaleActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            else if (type == typeof(ActiveObjAction))
            {
                for (int i = 0; i < ActiveObjActions.Count; i++)
                {
                    ActiveObjAction action = ActiveObjActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            else if (type == typeof(PlayEffectAction))
            {
                for (int i = 0; i < PlayEffectActions.Count; i++)
                {
                    PlayEffectAction action = PlayEffectActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            else if (type == typeof(PlayAnimAction))
            {
                for (int i = 0; i < PlayAnimActions.Count; i++)
                {
                    PlayAnimAction action = PlayAnimActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            else if (type == typeof(PlaySoundAction))
            {
                for (int i = 0; i < PlaySoundActions.Count; i++)
                {
                    PlaySoundAction action = PlaySoundActions[i];
                    if (action == null)
                    {
                        continue;
                    }
                    if (action.IdentifyName == actionName) return action;
                }
            }
            return null;
        }
        public ClipAction GetAction(string actionName)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                Utility.LogError("GetAction error caused by null action name"); return null;
            }
            for (int i = 0; i < MoveActions.Count; i++)
            {
                ClipAction action = MoveActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            for (int i = 0; i < RotateActions.Count; i++)
            {
                ClipAction action = RotateActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            for (int i = 0; i < ScaleActions.Count; i++)
            {
                ClipAction action = ScaleActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            for (int i = 0; i < UITweenPositionActions.Count; i++)
            {
                ClipAction action = UITweenPositionActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            for (int i = 0; i < UITweenRotationActions.Count; i++)
            {
                ClipAction action = UITweenRotationActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            for (int i = 0; i < UITweenScaleActions.Count; i++)
            {
                ClipAction action = UITweenScaleActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            for (int i = 0; i < ActiveObjActions.Count; i++)
            {
                ActiveObjAction action = ActiveObjActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            for (int i = 0; i < PlayEffectActions.Count; i++)
            {
                PlayEffectAction action = PlayEffectActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            for (int i = 0; i < PlayAnimActions.Count; i++)
            {
                PlayAnimAction action = PlayAnimActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            for (int i = 0; i < PlaySoundActions.Count; i++)
            {
                PlaySoundAction action = PlaySoundActions[i];
                if (action == null)
                {
                    continue;
                }
                if (action.IdentifyName == actionName) return action;
            }
            return null;
        }
        public void Clone(Clip obj)
        {
            if (obj == null) return;

            BeginTime = obj.BeginTime;
            EndTime = obj.EndTime;
            AnimateObj = obj.AnimateObj;
            ActiveObjOperation = obj.ActiveObjOperation;

            obj.GetAllAction();
            if (obj.Actions != null && obj.Actions.Count > 0)
            {
                for (int i = 0; i < obj.Actions.Count; i++)
                {
                    ClipAction action = obj.Actions[i];
                    if (action == null) continue;
                    ClipAction newAction = AddAction(action.GetType());
                    if (newAction != null)
                    {
                        newAction.Clone(action);
                    }
                }
            }

        }
        #region other functions
        public void ClampTime()
        {
            if (GetMinActionBeginTime() != -1) { BeginTime = GetMinActionBeginTime(); }
            if (GetMaxActionEndTime() != -1) { EndTime = GetMaxActionEndTime(); }
        }
        public float GetMinActionBeginTime()
        {
            GetAllAction();
            if (Actions == null || Actions.Count == 0) return -1;
            float minTime = Mathf.Infinity;
            for (int i = 0; i < Actions.Count; i++)
            {
                ClipAction action = Actions[i];
                if (action == null) continue;
                if (action.BeginTime < minTime)
                {
                    minTime = action.BeginTime;
                }
            }
            if (minTime < 0) minTime = 0;
            return minTime;
        }
        public float GetMaxActionEndTime()
        {
            GetAllAction();
            if (Actions == null || Actions.Count == 0) return -1;
            float maxTime = -Mathf.Infinity;
            for (int i = 0; i < Actions.Count; i++)
            {
                ClipAction action = Actions[i];
                if (action == null) continue;
                if (action.EndTime > maxTime)
                {
                    maxTime = action.EndTime;
                }
            }
            if (maxTime < 0) maxTime = 0;
            return maxTime;
        }
        #endregion

        #region editor function
#if UNITY_EDITOR
        public void DiaplayEditorView()
        {

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ClipName", GUILayout.Width(100));
            Name = EditorGUILayout.TextField(Name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("BeginTime", GUILayout.Width(100));
            BeginTime = EditorGUILayout.FloatField(BeginTime);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EndTime", GUILayout.Width(100));
            EndTime = EditorGUILayout.FloatField(EndTime);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AnimateObj", GUILayout.Width(100));
            AnimateObj = (GameObject)EditorGUILayout.ObjectField(AnimateObj, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ActiveObjOperation", GUILayout.Width(100));
            ActiveObjOperation = (GameObjectActiveOperation)EditorGUILayout.EnumPopup(ActiveObjOperation);
            GUILayout.EndHorizontal();
        }
#endif
        #endregion
    }
}
