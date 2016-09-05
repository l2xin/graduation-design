/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTEditorView.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/23 16:18:59
            // Modify History:
            //
//----------------------------------------------------------------*/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Threading;

namespace Air2000
{
    public class GTEditorView : EditorWindow
    {
        #region define emum & clazz
        public enum EaseKeyEvent
        {
            IntervalSelection,
            Copy,
            Paste,
            Withdrawal,
            AntiWithdrawal,
            SaveFile,
        }
        public enum EditorWindowEvtType
        {
            OnGUI,
            Update,
            OnInspectorUpdate,
            OnSelectionChange,
            OnFocus,
            OnLostFocus,
            OnHierarchyChange,
            OnProjectChange,
            OnEnable,
            OnDisable,
            OnDestroy,

            EasyKey_IntervalSelection,
            EasyKey_Copy,
            EasyKey_Paste,
            EasyKey_Withdrawal,
            EasyKey_AntiWithdrawal,
            EasyKey_SaveFile,
        }
        public class EditorEvent : BaseEvent
        {
            public EditorEvent(int evtID)
            {
                pEventID = evtID;
            }
        }

        public class EditorEventEx<T> : EditorEvent
        {
            public T mParam;
            public EditorEventEx(int evtID, T param)
                : base(evtID)
            {
                mParam = param;
            }
            public T GetData() { return mParam; }
        }
        public class EditorEventBus : EventManager
        {
            public void NotifyEvent(EditorEvent evt)
            {
                if (evt == null)
                {
                    return;
                }
                base.NotifyEvent(evt.pEventID, evt);
            }

            public void RegisterEditorEvent(int evtID, EventFuntion func)
            {
                if (func != null)
                {
                    base.RegisterMsgHandler(evtID, func);
                }
            }

            public void UnRegisterEditorEvent(int evtID, EventFuntion func)
            {
                if (func != null)
                {
                    base.UnRegisterMsgHandler(evtID, func);
                }
            }

            public void UnRegisterAllEvent()
            {
                base.UnRegisterAllMsgHandlers();
            }
        }
        #endregion

        #region define member
        protected Dictionary<int, GTEditorSubView> AllSubViews = new Dictionary<int, GTEditorSubView>();
        public GTEditorSubView CurrentActiveView { get; set; }
        public GTEditorSubView LastActiveView { get; set; }
        public EditorEventBus EventBus = new EditorEventBus();
        public bool ViewActive { get; set; }
        public GTEditorTimer Timer = new GTEditorTimer();
        #endregion

        #region define common function
        public void AddSubView(GTEditorSubView view)
        {
            if (AllSubViews == null)
            {
                AllSubViews = new Dictionary<int, GTEditorSubView>();
            }
            if (view == null)
            {
                return;
            }
            GTEditorSubView subView = null;
            if (AllSubViews.TryGetValue(view.ViewID, out  subView))
            {
                return;
            }
            AllSubViews.Add(view.ViewID, view);
        }
        public void RemoveSubView(GTEditorSubView view)
        {
            if (view == null)
            {
                return;
            }
            if (AllSubViews == null || AllSubViews.Count == 0)
            {
                return;
            }
            view.OnDestroy();
            AllSubViews.Remove(view.ViewID);
        }
        public void ClearAllSubView()
        {
            if (AllSubViews == null || AllSubViews.Count == 0)
            {
                return;
            }
            Dictionary<int, GTEditorSubView>.Enumerator em = AllSubViews.GetEnumerator();
            for (int i = 0; i < AllSubViews.Count; i++)
            {
                em.MoveNext();
                KeyValuePair<int, GTEditorSubView> kvp = em.Current;
                if (kvp.Value == null)
                {
                    continue;
                }
                kvp.Value.OnDestroy();
            }
            AllSubViews.Clear();
        }
        public GTEditorSubView GetSubViewByID(int viewID)
        {
            if (AllSubViews == null || AllSubViews.Count == 0)
            {
                return null;
            }
            GTEditorSubView subView = null;
            AllSubViews.TryGetValue(viewID, out subView);
            return subView;
        }
        public virtual void GoToSubView(int viewID, GTEditorSubView context, object param = null)
        {
            if (AllSubViews == null || AllSubViews.Count == 0)
            {
                Helper.LogError("GTEditorView GoToSubView:Error caused by null subview collection");
                return;
            }
            GTEditorSubView view = null;
            if (AllSubViews.TryGetValue(viewID, out view))
            {
                if (context != null)
                {
                    context.OnInActive(view);
                }
                CurrentActiveView = view;
                LastActiveView = context;
                CurrentActiveView.OnActive(context, param);
            }
        }
        public void NotityAllSubView(EditorWindowEvtType evtType, object param)
        {
            if (AllSubViews != null && AllSubViews.Count > 0)
            {
                Dictionary<int, GTEditorSubView>.Enumerator em = AllSubViews.GetEnumerator();
                for (int i = 0; i < AllSubViews.Count; i++)
                {
                    em.MoveNext();
                    KeyValuePair<int, GTEditorSubView> kvp = em.Current;
                    GTEditorSubView subView = kvp.Value;
                    if (subView == null)
                    {
                        continue;
                    }
                    switch (evtType)
                    {
                        case EditorWindowEvtType.OnGUI:
                            subView.OnGUI();
                            break;
                        case EditorWindowEvtType.Update:
                            subView.Update();
                            break;
                        case EditorWindowEvtType.OnInspectorUpdate:
                            subView.OnInspectorUpdate();
                            break;
                        case EditorWindowEvtType.OnSelectionChange:
                            subView.OnSelectionChange();
                            break;
                        case EditorWindowEvtType.OnFocus:
                            subView.OnFoucs();
                            break;
                        case EditorWindowEvtType.OnLostFocus:
                            subView.OnLostFocus();
                            break;
                        case EditorWindowEvtType.OnHierarchyChange:
                            subView.OnHierarchyChange();
                            break;
                        case EditorWindowEvtType.OnProjectChange:
                            subView.OnProjectChange();
                            break;
                        case EditorWindowEvtType.OnEnable:
                            subView.OnEnable();
                            break;
                        case EditorWindowEvtType.OnDisable:
                            subView.OnDisable();
                            break;
                        case EditorWindowEvtType.OnDestroy:
                            subView.OnDestroy();
                            break;
                        default:
                            break;
                    }
                }

            }
        }
        #endregion


        #region define notification
        public class Notification
        {
            public string[] Texts { get; set; }
            public float Time { get; set; }
        }
        private void CloseNotification(object obj)
        {
            RemoveNotification();
            Notification oldNo = obj as Notification;
            if (CacheNotification != null)
            {
                CacheNotification.Remove(oldNo);
            }
            NotificationActive = false;
            if (CacheNotification != null && CacheNotification.Count > 0)
            {
                Notification notify = null;
                for (int i = 0; i < CacheNotification.Count; i++)
                {
                    notify = CacheNotification[i];
                    if (notify == null) continue;
                    if (notify.Texts == null || notify.Texts.Length == 0 || notify.Time <= 0)
                    {
                        CacheNotification.Remove(notify);
                        i--;
                    }
                    break;
                }
                if (notify != null)
                {
                    DoCacheNotification(notify.Time, notify, notify.Texts);
                }
            }
        }
        public GTEditorTimer.Clock Clock = null;
        private List<Notification> CacheNotification = new List<Notification>();
        private bool NotificationActive = false;
        public void SetNotification(float time = 1, params string[] texts)
        {
            if (time <= 0 || texts == null || texts.Length == 0) return;
            if (NotificationActive)
            {
                Notification no = new Notification() { Time = time, Texts = texts };
                CacheNotification.Add(no); return;
            }
            Clock = new GTEditorTimer.Clock(time, CloseNotification, null, false);
            Timer.AddClock(Clock);

            if (texts == null || texts.Length == 0) return;
            string text = string.Empty;
            for (int i = 0; i < texts.Length; i++)
            {
                string tempStr = texts[i];
                if (string.IsNullOrEmpty(tempStr)) continue;
                if (string.IsNullOrEmpty(text))
                {
                    text += tempStr;
                }
                else
                {
                    text += "\n" + tempStr;
                }
            }
            NotificationActive = true;
            ShowNotification(new GUIContent(text));
        }
        private void DoCacheNotification(float time = 1, object param = null, params string[] texts)
        {
            if (time <= 0 || texts == null || texts.Length == 0)
            {
                NotificationActive = false;
                return;
            }

            if (NotificationActive)
            {
                Notification no = new Notification() { Time = time, Texts = texts };
                CacheNotification.Add(no); return;
            }
            Clock = new GTEditorTimer.Clock(time, CloseNotification, param, false);
            Timer.AddClock(Clock);

            if (texts == null || texts.Length == 0) return;
            string text = string.Empty;
            for (int i = 0; i < texts.Length; i++)
            {
                string tempStr = texts[i];
                if (string.IsNullOrEmpty(tempStr)) continue;
                if (string.IsNullOrEmpty(text))
                {
                    text += tempStr;
                }
                else
                {
                    text += "\n" + tempStr;
                }
            }
            NotificationActive = true;
            ShowNotification(new GUIContent(text));
        }
        #endregion

        #region define virtual method
        public virtual void OnWindowGUI()
        {
            //NotityAllSubView(EditorWindowEvtType.OnGUI, null);
        }
        public virtual void OnEditorInspectorUpdate()
        {
            NotityAllSubView(EditorWindowEvtType.OnInspectorUpdate, null);
        }
        public virtual void OnWindowFoucs()
        {
            NotificationActive = false;
            NotityAllSubView(EditorWindowEvtType.OnFocus, null);
        }
        public virtual void OnWindowLostFocus()
        {
            NotificationActive = false;
            NotityAllSubView(EditorWindowEvtType.OnLostFocus, null);
        }
        public virtual void OnEditorHierarchyChange()
        {
            NotityAllSubView(EditorWindowEvtType.OnHierarchyChange, null);
        }

        public virtual void OnEditorProjectChange()
        {
            NotityAllSubView(EditorWindowEvtType.OnProjectChange, null);
        }
        public virtual void OnEditorSelectionChange()
        {
            NotityAllSubView(EditorWindowEvtType.OnSelectionChange, null);
        }
        public virtual void WindowUpdate()
        {
            Timer.Update();
            NotityAllSubView(EditorWindowEvtType.Update, null);
        }
        public virtual void OnWindowEnable()
        {
            NotityAllSubView(EditorWindowEvtType.OnEnable, null);
        }
        public virtual void OnWindowDisable()
        {
            NotityAllSubView(EditorWindowEvtType.OnDisable, null);
        }
        public virtual void OnWindowDestroy()
        {
            if (EventBus != null)
            {
                EventBus.UnRegisterAllMsgHandlers();
            }
            NotityAllSubView(EditorWindowEvtType.OnDestroy, null);
        }
        #endregion

        #region define main gui function
        void OnGUI()
        {
            OnWindowGUI();
        }
        void OnInspectorUpdate()
        {
            OnEditorInspectorUpdate();
        }
        void OnFoucs()
        {
            ViewActive = true;
            OnWindowFoucs();
        }
        void OnLostFocus()
        {
            ViewActive = false;
            OnWindowLostFocus();
        }
        void OnHierarchyChange()
        {
            OnEditorHierarchyChange();
        }

        void OnProjectChange()
        {
            OnEditorProjectChange();
        }
        void OnSelectionChange()
        {
            OnEditorSelectionChange();
        }
        void Update()
        {
            WindowUpdate();
        }
        void OnEnable()
        {
            OnWindowEnable();
        }
        void OnDisable()
        {
            OnWindowDisable();
        }
        void OnDestroy()
        {
            OnWindowDestroy();
        }
        #endregion

        #region define utils function
        #endregion
    }
}
#endif