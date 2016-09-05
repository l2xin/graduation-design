/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ModifyRenderQForNGUI.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/5/24 18:50:31
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
    [CustomEditor(typeof(ModifyRenderQForNGUI))]
    public class InspecModifyRenderQ : Editor
    {
        public ModifyRenderQForNGUI Instance;
        void OnEnable()
        {
            Instance = target as ModifyRenderQForNGUI;
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
    public class ModifyRenderQForNGUI : MonoBehaviour
    {
        public string Identify = "NULL";
        public List<Renderer> Renderers = new List<Renderer>();
        private List<int> mRenderMaterialHis = new List<int>();
        //public List<ParticleSystem> Particles = new List<ParticleSystem>();
        public int TargetRenderQ;

#if UNITY_EDITOR

        void Update()
        {
            FixMaterialRenderQueue();
        }
#endif
        void Awake()
        {
            FixMaterialRenderQueue();
        }
        void OnEnable()
        {
            FixMaterialRenderQueue();
        }
        void OnDisable()
        {
            RevertMaterialRenderQueue();
        }
        void OnDestroy()
        {
            RevertMaterialRenderQueue();
        }

        public void FixMaterialRenderQueue()
        {
            if (TargetRenderQ < 0) return;
            if (mRenderMaterialHis != null) mRenderMaterialHis.Clear();
            if (Renderers == null || Renderers.Count == 0)
            {
                Debug.LogError("ModifyRenderQ.cs Renderers is NULL Been attach Obj named : " + this.transform.name);
                return;
            }

            for (int i = 0; i < Renderers.Count; i++)
            {
                Renderer renderer = Renderers[i];
                if (renderer == null || renderer.material == null)
                {
                    mRenderMaterialHis.Add(-1);
                    continue;
                }
                Material tempMaterial = GetMaterial(renderer);
                mRenderMaterialHis.Add(tempMaterial.renderQueue);
                tempMaterial.renderQueue = TargetRenderQ;
            }

        }

        public void DynamicSetup(int targetQueue)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            Renderers = new List<Renderer>(renderers);
            FixMaterialRenderQueue();
        }

        private void RevertMaterialRenderQueue()
        {
            if (Renderers != null && Renderers.Count > 0)
            {
                for (int i = 0; i < Renderers.Count; i++)
                {
                    Renderer renderer = Renderers[i];
                    if (renderer == null || renderer.material == null) continue;
                    Material tempMaterial = GetMaterial(renderer);
                    if (tempMaterial == null) continue;
                    if (mRenderMaterialHis.Count > i)
                    {
                        tempMaterial.renderQueue = mRenderMaterialHis[i];
                    }
                }
            }
        }

        private Material GetMaterial(Renderer varRender)
        {
            if (varRender == null)
            {
                return null;
            }

#if UNITY_EDITOR
            Material tempMaterial = varRender.material;
#else
			Material tempMaterial = varRender.sharedMaterial;
#endif
            return tempMaterial;
        }

    }


}
