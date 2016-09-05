/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: NGUIDelayRender.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/7/18 19:06:30
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

namespace Air2000
{
#if UNITY_EDITOR
    [CustomEditor(typeof(NGUIDelayRender))]
    public class InspecDelayRender : Editor
    {
        public NGUIDelayRender Instance;
        void OnEnable()
        {
            Instance = target as NGUIDelayRender;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Instance == null)
            {
                return;
            }
            GTEditorHelper.BeginContents();
            if (GUILayout.Button("Get All Renderer In Children"))
            {
                Renderer[] renderers = Instance.GetComponentsInChildren<Renderer>(true);
                Instance.Renderers = new List<Renderer>(renderers);
            }
            GTEditorHelper.EndContents();
        }
    }
#endif
    public class NGUIDelayRender : MonoBehaviour
    {
        public int DelayFrame;
        private int Count = 0;
        public List<Renderer> Renderers = new List<Renderer>();
        void Awake()
        {
            Count = DelayFrame;
            if (Renderers == null || Renderers.Count == 0)
            {
                enabled = false;
                return;
            }
            for (int i = 0; i < Renderers.Count; i++)
            {
                Renderer renderer = Renderers[i];
                if (renderer == null) continue;
                renderer.enabled = false;
            }
        }
        void Update()
        {
            Count--;
            if (Count <= 0)
            {
                ActiveRenderer();
            }
        }
        void ActiveRenderer()
        {
            if (Renderers == null || Renderers.Count == 0)
            {
                return;
            }
            for (int i = 0; i < Renderers.Count; i++)
            {
                Renderer renderer = Renderers[i];
                if (renderer == null) continue;
                renderer.enabled = true;
            }
            enabled = false;
        }
    }
}
