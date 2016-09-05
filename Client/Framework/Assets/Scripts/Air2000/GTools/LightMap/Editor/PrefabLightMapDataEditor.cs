/*----------------------------------------------------------------
            // Copyright © 2015 NCSpeedLight
            // 
            // FileName: PrefabLightMapDataEditor.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/21 15:49:41
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NCSpeedLight
{
    public class PrefabLightMapDataEditor : Editor
    {
        [MenuItem("GTools/Lightmap/Save LightmapInfo", false, 0)]
        public static void SaveLightMapInfo()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null) return;
            PrefabLightmapData data = go.GetComponent<PrefabLightmapData>();
            if (data == null)
            {
                data = go.AddComponent<PrefabLightmapData>();
            }
            data.SaveLightmap(go.transform);
            EditorUtility.SetDirty(go);
        }

        [MenuItem("GTools/Lightmap/Load LightmapInfo", false, 0)]
        public static void LoadLightmapInfo()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null) return;
            PrefabLightmapData data = go.GetComponent<PrefabLightmapData>();
            if (data == null) return;
            data.LoadLightmap();
            EditorUtility.SetDirty(go);
        }

        [MenuItem("GTools/Lightmap/Clear LightmapInfo", false, 0)]
        public static void ClearLightmapInfo()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null) return;
            PrefabLightmapData data = go.GetComponent<PrefabLightmapData>();
            if (data == null) return;
            data.AllRendererInfo.Clear();
            EditorUtility.SetDirty(go);
        }

        [MenuItem("GTools/Lightmap/Update Dependency", false, 0)]
        public static void UpdateDependency()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null) return;
            PrefabLightmapData data = go.GetComponent<PrefabLightmapData>();
            if (data == null) return;
            if (data.AllRendererInfo == null || data.AllRendererInfo.Count == 0) return;
            for (int i = 0; i < data.AllRendererInfo.Count; i++)
            {
				PrefabLightmapData.RendererInfo info = data.AllRendererInfo[i];
                info.UpdateDependency(go.transform);
				data.AllRendererInfo.RemoveAt(i);
				data.AllRendererInfo.Insert(i,info);
            }
            EditorUtility.SetDirty(go);
        }
    }
}
