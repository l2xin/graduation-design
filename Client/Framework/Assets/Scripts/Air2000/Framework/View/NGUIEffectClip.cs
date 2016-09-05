/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: NGUIEffectClip.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/9/1 14:09:00
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
using Air2000;
#endif

namespace Air2000
{

#if UNITY_EDITOR
    [CustomEditor(typeof(NGUIEffectClip))]
    public class InspecEffectClipForNGUI : Editor
    {
        public NGUIEffectClip Instance;
        void OnEnable()
        {
            Instance = target as NGUIEffectClip;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Instance == null)
            {
                return;
            }
            if (Instance.Panel == null)
            {
                EditorGUILayout.HelpBox("Please assign an UIPanel component!", MessageType.Error);
            }
            if (Instance.Camera == null)
            {
                EditorGUILayout.HelpBox("We will use WindowManager.UICamera when Application is playing", MessageType.Warning);
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



    [ExecuteInEditMode]
    public class NGUIEffectClip : MonoBehaviour
    {
        public string Identify = "identify name";
        public List<Renderer> Renderers = new List<Renderer>();
        public UIPanel Panel;
        public UICamera Camera;

        private int PanelWidthAttribID;
        private int PanelHeightAttribID;
        private int PanelCenterAndSharpnessAttribID;

        void OnEnable()
        {
            if (Application.isPlaying)
            {
                Camera = WindowManager.GetSingleton().pUICamera;
            }
        }
        void Start()
        {
            // initialize shader attribs
            PanelWidthAttribID = Shader.PropertyToID("_PanelWidth");
            PanelHeightAttribID = Shader.PropertyToID("_PanelHeight");
            PanelCenterAndSharpnessAttribID = Shader.PropertyToID("_PanelCenterAndSharpness");

            SetClipInfo();
        }
        void Update() { SetClipInfo(); }
        void SetClipInfo()
        {
            if (Panel && Panel.hasClipping && Camera && Camera.GetComponent<Camera>() && Renderers != null && Renderers.Count > 0)
            {
                Vector2 soft = Panel.clipSoftness;
                Vector2 sharpness = new Vector2(1000.0f, 1000.0f);

                if (soft.x > 0f)
                {
                    sharpness.x = Panel.baseClipRegion.z / soft.x;
                }
                if (soft.y > 0f)
                {
                    sharpness.y = Panel.baseClipRegion.w / soft.y;
                }

                Vector4 panelCenterAndSharpness;

                Camera camera = Camera.GetComponent<Camera>();

                Vector3[] panelWorldCorners = Panel.worldCorners;
                Vector3 leftBottom = camera.WorldToViewportPoint(panelWorldCorners[0]);
                Vector3 topRight = camera.WorldToViewportPoint(panelWorldCorners[2]);
                Vector3 center = Vector3.Lerp(leftBottom, topRight, 0.5f);

                panelCenterAndSharpness.x = center.x;
                panelCenterAndSharpness.y = center.y;
                panelCenterAndSharpness.z = sharpness.x;
                panelCenterAndSharpness.w = sharpness.y;


                for (int i = 0; i < Renderers.Count; i++)
                {
                    Renderer renderer = Renderers[i];
                    Material mat = null;
                    if (!Application.isPlaying)
                        mat = renderer.sharedMaterial;
                    else
                        mat = renderer.material;

                    if (mat == null) continue;
                    mat.SetFloat(PanelWidthAttribID, topRight.x - leftBottom.x);
                    mat.SetFloat(PanelHeightAttribID, topRight.y - leftBottom.y);
                    mat.SetVector(PanelCenterAndSharpnessAttribID, panelCenterAndSharpness);
                }
            }
        }
    }
}
