/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTEditorSubView.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/23 16:19:23
            // Modify History:
            //
//----------------------------------------------------------------*/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Air2000
{
    public class GTEditorSubView
    {
        public GTEditorView AppContext;
        public int ViewID { get; set; }
        public bool Active { get; set; }
        public GTEditorSubView(GTEditorView context)
        {
            AppContext = context;
        }
        public virtual void OnActive(GTEditorSubView context, object param = null) { Active = true; }

        public virtual void OnInActive(GTEditorSubView context, object param = null) { Active = false; }

        public virtual void OnGUI() { }
        public virtual void OnInspectorUpdate() { }
        public virtual void OnFoucs() { }
        public virtual void OnLostFocus() { }
        public virtual void OnHierarchyChange() { }
        public virtual void OnProjectChange() { }
        public virtual void OnSelectionChange() { }
        public virtual void Update() { }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnDestroy() { }
    }
}
#endif