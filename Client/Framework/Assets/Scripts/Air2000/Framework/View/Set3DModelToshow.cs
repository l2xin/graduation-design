/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Set3DModelToshow.cs
			// Describle: 3D模型加载
			// Created By:  hsu
			// Date&Time:  2016/1/19 15:52:00
            // Modify History:
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameLogic;

namespace Air2000
{
    public class Set3DModelToshow
    {
        static Set3DModelToshow mSingle = null;
        private GameObject mModelRoot;
        private GameObject mModelParent;
        private RenderTexture mRenderTexture;
        private GameObject mCurShowModel;
        private int WIDTH = 256;
        private int HEIGHT = 256;
        private Camera mCamera;
        private int mLayer = LayerMask.NameToLayer("ThrModel");
        /// <summary>
        /// 所有模型.
        /// </summary>
        private Dictionary<int, GameObject> mModels = new Dictionary<int, GameObject>();
        static public Set3DModelToshow getSingle()
        {
            if (mSingle == null)
            {
                mSingle = new Set3DModelToshow();
            }
            return mSingle;
        }
        /// <summary>
        /// 创建3D模型显示结点.
        /// </summary>
        /// <param name="varModel">模型.</param>
        /// <param name="varUITexture">UITexture 用于显示的Texture.</param>
        /// <param name="varWidth">RenderTexture的宽</param>
        /// <param name="varHeight">RenderTexture的高</param>
        public void CreateModelShowRoot(GameObject varModel, UITexture varUITexture, int varWidth, int varHeight)
        {
            if (varModel == null || varUITexture == null)
            {
                return;
            }
            if (mModelRoot == null)
            {
                mModelRoot = new GameObject("Show3DModelRoot");
                GameObject tempCameraObj = new GameObject("Camera");
                tempCameraObj.transform.SetParent(mModelRoot.transform);
                mCamera = tempCameraObj.AddComponent<Camera>();
                mCamera.cullingMask = Helper.OnlyIncluding(mLayer);
                mCamera.clearFlags = CameraClearFlags.SolidColor;
                mCamera.orthographic = true;
                mCamera.orthographicSize = 1.15f;
                mCamera.farClipPlane = 100;
                mCamera.transform.localPosition = new Vector3(1000, 0, 0);
                Color tempColor = new Color(0, 0, 0, 0);
                mCamera.backgroundColor = tempColor;
                mModelParent = new GameObject("ModelParent");
                mModelParent.transform.SetParent(mModelRoot.transform);
                mModelParent.transform.localPosition = new Vector3(1000, 0, 3);
                mModelParent.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            if (mModelRoot != null)
            {
                mRenderTexture = RenderTexture.GetTemporary(varWidth, varHeight, 24, RenderTextureFormat.ARGB32,RenderTextureReadWrite.Linear,4);
                mCamera.targetTexture = mRenderTexture;
            }
            varModel.transform.parent = mModelParent.transform;
            varModel.transform.localScale = Vector3.one;
            varModel.transform.localPosition = Vector3.zero;
            NGUITools.SetLayer(varModel, mLayer);

            //			Helper.SetLayer(varModel,GameScene.LAYER.ThrModel);



            mCurShowModel = varModel;

            EnableAllModel();

            GameObject tmpObj = null;
            if (mModels.TryGetValue(mCurShowModel.GetHashCode(), out tmpObj) == false)
            {
                mModels.Add(mCurShowModel.GetHashCode(), mCurShowModel);
            }

            mCurShowModel.gameObject.SetActive(true);

            varUITexture.shader = Shader.Find("Unlit/Premultiplied Colored");
            varUITexture.mainTexture = mCamera.targetTexture;
        }
        private void EnableAllModel()
        {
            if (mModels != null)
            {
                Dictionary<int, GameObject>.Enumerator tempEn = mModels.GetEnumerator();
                for (int i = 0; i < mModels.Count; i++)
                {
                    tempEn.MoveNext();
                    KeyValuePair<int, GameObject> tempValue = tempEn.Current;

                    if (tempValue.Value != null)
                    {
                        tempValue.Value.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// 设置模型位置.
        /// </summary>
        /// <param name="varVe3">Variable ve3.</param>
        public void SetModelRotation(Vector3 varVe3)
        {
            if (mCurShowModel == null)
            {
                return;
            }
            mCurShowModel.transform.localEulerAngles = varVe3;
        }
        /// <summary>
        /// 设置模型旋转.
        /// </summary>
        /// <param name="varVe3">Variable ve3.</param>
        public void SetModelPosition(Vector3 varVe3)
        {
            if (mCurShowModel == null)
            {
                return;
            }
            mCurShowModel.transform.localPosition = varVe3;
        }
        /// <summary>
        /// 设置模型缩放.
        /// </summary>
        /// <param name="varVe3">Variable ve3.</param>
        public void SetModelScale(Vector3 varVe3)
        {
            if (mCurShowModel == null)
            {
                return;
            }
            mCurShowModel.transform.localScale = varVe3;
        }
        /// <summary>
        /// 设置相机的视野.
        /// </summary>
        /// <param name="varValue">Variable value.</param>
        public void SetCameraFieldOfView(float varValue)
        {
            if (mCamera != null)
            {
                mCamera.fieldOfView = varValue;
            }
        }
        /// <summary>
        /// 显示最后一个被添加的模型.
        /// </summary>
        public void ShowModelsLastOne()
        {
            if (mModels != null)
            {
                Dictionary<int, GameObject>.Enumerator tempEn = mModels.GetEnumerator();
                for (int i = 0; i < mModels.Count; i++)
                {
                    tempEn.MoveNext();
                    KeyValuePair<int, GameObject> tempValue = tempEn.Current;

                    if (i == mModels.Count - 1)
                    {
                        if (tempValue.Value != null)
                        {
                            tempValue.Value.SetActive(true);
                            mCurShowModel = tempValue.Value;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 激活/死一个模型对象.
        /// </summary>
        /// <param name="varHashCode">Variable hash code.</param>
        /// <param name="varFrag">If set to <c>true</c> variable frag.</param>
        public void EnableModel(int varHashCode, bool varFrag)
        {
            if (mModels != null)
            {
                GameObject tempObj;
                if (mModels.TryGetValue(varHashCode, out tempObj))
                {
                    tempObj.SetActive(varFrag);
                }
            }
        }
        /// <summary>
        /// 建立3D模型显示结点,默认RenderTextureFormat 宽高:256 * 256.
        /// </summary>
        public void CreateModelShowRoot(GameObject varModel, UITexture varUITexture)
        {
            CreateModelShowRoot(varModel, varUITexture, WIDTH, HEIGHT);
        }
        /// <summary>
        /// 销毁当前模型.
        /// </summary>
        /// <param name="varHashCode">Variable hash code.</param>
        public void DestoryShowModel()
        {
            if (mCurShowModel != null)
            {
                CharacterPoolController.Pool(mCurShowModel);
            }
        }
        public GameObject GetCurShowModel()
        {
            return mCurShowModel;
        }
        /// <summary>
        /// 销毁模型(不用模型显示销毁).
        /// </summary>
        public void DestoryShowModel(int varHashCode)
        {
            if (mModels != null)
            {
                if (mModels.ContainsKey(varHashCode))
                {
                    GameObject tempObj;
                    if (mModels.TryGetValue(varHashCode, out tempObj))
                    {
                        //GameObject.Destroy(tempObj);
                        CharacterPoolController.Pool(mCurShowModel);
                        mModels.Remove(varHashCode);
                    }
                }
            }
            ShowModelsLastOne();
        }
        /// <summary>
        /// 销毁结点,在不掉用的情况下销毁.
        /// </summary>
        public void DestoryModelRoot()
        {
            if (mModelRoot != null)
            {
                GameObject.Destroy(mModelRoot);
            }
            RenderTexture.ReleaseTemporary(mRenderTexture);
        }
        /// <summary>
        /// 水平偏移,旋转模型--沿Y轴.
        /// </summary>
        /// <param name="varV2">Variable v2.</param>
        public void Roate_X_ModelBaseOn_Y(Vector2 varV2)
        {
            if (varV2 != Vector2.zero && mCurShowModel != null)
            {
                mCurShowModel.transform.localEulerAngles = new Vector3(mCurShowModel.transform.localEulerAngles.x,
                                                                    mCurShowModel.transform.localEulerAngles.y - varV2.x,
                                                                    mCurShowModel.transform.localEulerAngles.z);
            }
        }
        /// <summary>
        /// 垂直偏移,旋转模型--沿Y轴.
        /// </summary>
        /// <param name="varV2">Variable v2.</param>
        public void Roate_Y_ModelBaseOn_Y(Vector2 varV2)
        {
            if (varV2 != Vector2.zero && mCurShowModel != null)
            {
                mCurShowModel.transform.localEulerAngles = new Vector3(mCurShowModel.transform.localEulerAngles.x,
                                                                    mCurShowModel.transform.localEulerAngles.y - varV2.y,
                                                                    mCurShowModel.transform.localEulerAngles.z);
            }
        }
        /// <summary>
        /// 水平偏移,旋转模型--沿X轴.
        /// </summary>
        /// <param name="varV2">Variable v2.</param>
        public void Roate_X_ModelBaseOn_X(Vector2 varV2)
        {
            if (varV2 != Vector2.zero && mCurShowModel != null)
            {
                mCurShowModel.transform.localEulerAngles = new Vector3(mCurShowModel.transform.localEulerAngles.x - varV2.x,
                                                                    mCurShowModel.transform.localEulerAngles.y,
                                                                    mCurShowModel.transform.localEulerAngles.z);
            }
        }
        /// <summary>
        /// 垂直偏移,旋转模型--沿X轴.
        /// </summary>
        /// <param name="varV2">Variable v2.</param>
        public void Roate_Y_ModelBaseOn_X(Vector2 varV2)
        {
            if (varV2 != Vector2.zero && mCurShowModel != null)
            {
                mCurShowModel.transform.localEulerAngles = new Vector3(mCurShowModel.transform.localEulerAngles.x - varV2.y,
                                                                    mCurShowModel.transform.localEulerAngles.y,
                                                                    mCurShowModel.transform.localEulerAngles.z);
            }
        }
    }
}