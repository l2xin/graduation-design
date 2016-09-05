/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: PrefabLightmapData.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/21 15:49:41
            // Modify History:
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class PrefabLightmapData : MonoBehaviour
{
    [System.Serializable]
    public struct RendererInfo
    {
        public Renderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
        public string RootPath;
        public void UpdateRootPath(Transform parent)
        {
            if (parent == null) return;
            if (renderer == null) return;
            RootPath = GenerateRootPath(parent, renderer.transform, RootPath);
        }
        public void UpdateDependency(Transform parent)
        {
            if (parent == null) return;
            if (string.IsNullOrEmpty(RootPath)) return;
            Transform renderTransform = parent.Find(RootPath);
            if (renderTransform == null) return;
			Renderer[]renderers = renderTransform.GetComponentsInChildren<MeshRenderer>(true);
			if (renderers.Length >= 1) {
				renderer = renderers[0];
			}
        }
    }

    public List<RendererInfo> AllRendererInfo;

    void Awake()
    {
        LoadLightmap();
    }

    public void SaveLightmap(Transform parent)
    {
        if (AllRendererInfo == null)
        {
            AllRendererInfo = new List<RendererInfo>();
        }
        AllRendererInfo.Clear();

        Renderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
        if (renderers != null && renderers.Length > 0)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (renderer == null)
                {
                    continue;
                }
                RendererInfo info = new RendererInfo();
                info.renderer = renderer;
                info.lightmapIndex = renderer.lightmapIndex;
                info.lightmapOffsetScale = renderer.lightmapScaleOffset;
                info.UpdateRootPath(parent);
                AllRendererInfo.Add(info);
            }
        }
    }

    public void LoadLightmap()
    {
        if (AllRendererInfo == null || AllRendererInfo.Count == 0)
        {
            return;
        }
        for (int i = 0; i < AllRendererInfo.Count; i++)
        {
            RendererInfo rendererInfo = AllRendererInfo[i];
            if (rendererInfo.renderer == null)
            {
                continue;
            }
            rendererInfo.renderer.lightmapIndex = rendererInfo.lightmapIndex;
            rendererInfo.renderer.lightmapScaleOffset = rendererInfo.lightmapOffsetScale;
        }
    }

    public static string GenerateRootPath(Transform varFinalParentTran, Transform varTran, string varTargetRoot)
    {
        if (varFinalParentTran == null || varTran == null)
        {
            return string.Empty;
        }
        if (varFinalParentTran != varTran)
        {
            if (string.IsNullOrEmpty(varTargetRoot))
            {
                varTargetRoot = varTran.name;
            }
            else
            {
                varTargetRoot = varTran.name + "/" + varTargetRoot;
            }
            return GenerateRootPath(varFinalParentTran, varTran.parent, varTargetRoot);
        }
        else
        {
            return varTargetRoot;
        }
    }
}
