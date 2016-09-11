/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: LoadLevelSimulationOperation.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/4/18 14:36:48
            // Modify History:
            //
//----------------------------------------------------------------*/
using System.Collections;
using UnityEngine;

namespace Air2000.Res
{
    public class LoadLevelSimulationOperation : LoadOperation
    {
        private AsyncOperation mOperation = null;
        public LoadLevelSimulationOperation(string assetBundleName, string levelName, bool isAdditive)
        {
            //if (string.IsNullOrEmpty(assetBundleName)) return;
            //if (string.IsNullOrEmpty(levelName)) return;
            //string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
            //if (levelPaths == null || levelPaths.Length == 0)
            //{
            //    return;
            //}
            //if (isAdditive)
            //    mOperation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(levelPaths[0]);
            //else
            //    mOperation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
            if (isAdditive)
                Application.LoadLevelAdditive(levelName);
            else
                Application.LoadLevel(levelName);

        }
        public override bool Update() { return false; }
        public override bool IsDone() { return mOperation == null || mOperation.isDone; }
    }
}
