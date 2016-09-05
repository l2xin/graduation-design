/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ScreenTagManager.cs
			// Describle: 屏幕标签管理
			// Created By:  hsu
			// Date&Time:  2016/1/5 19:11:09
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Air2000
{
    public delegate void ScreenTagDel(object varParam);
    /// <summary>
    /// 屏幕标签管理
    /// </summary>
    public class ScreenTagManager : MonoBehaviour
    {
        #region private member
        private static ScreenTagManager mInstance;
        private Transform mTransform;
        private List<ScreenTag> mScreenTags;
        private GameObject mCameraObject;
        private Camera mCamera;
        #endregion

        #region public property
        public Transform pTransform
        {
            get { return mTransform; }
            set { mTransform = value; }
        }
        #endregion

        #region private method
        private ScreenTagManager()
        {
            mScreenTags = new List<ScreenTag>();
        }
        private void Update()
        {
            if (mCamera == null && mCameraObject != null)
            {
                mCamera = mCameraObject.GetComponentInChildren<Camera>();
            }
            if (mScreenTags != null && mScreenTags.Count > 0)
            {
                for (int i = 0; i < mScreenTags.Count; i++)
                {
                    ScreenTag tmpTag = mScreenTags[i];
                    if (tmpTag == null || tmpTag.mRelativeObj == null)
                    {
                        continue;
                    } if (tmpTag.mRelativeObj == null || tmpTag.mRelativeObj.transform == null)
                    {
                        Destroy(tmpTag.gameObject);
                        mScreenTags.Remove(tmpTag);
                    }
                    Vector3 tmpWorldPos = tmpTag.mRelativeObj.transform.position;
                    tmpWorldPos += tmpTag.mOffset;
                    if (Helper.IsInViewPort(mCamera, tmpWorldPos) == false)
                    {
                        tmpTag.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (tmpTag.gameObject.activeSelf == false)
                        {
                            tmpTag.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        #endregion

        #region public method
        public static ScreenTagManager GetSingleton()
        {
            if (mInstance == null)
            {
                GameObject tmpObj = new GameObject("ScreenTags");
                if (WindowManager.GetSingleton().pRootObject != null)
                {
                    tmpObj.transform.SetParent(WindowManager.GetSingleton().pRootObject.transform);
                    tmpObj.transform.localScale = Vector3.one;
                }
                GameObject.DontDestroyOnLoad(tmpObj);
                mInstance = tmpObj.AddComponent<ScreenTagManager>();
                mInstance.pTransform = tmpObj.transform;
            }
            if (mInstance.mCameraObject == null)
            {
                Helper.LogWarning("Please set a Camera for translating!");
            }
            return mInstance;
        }

        /// <summary>
        /// 设置依赖变换的相机
        /// </summary>
        /// <param name="varCamera"></param>
        public void SetTranslateCamera(GameObject varCameraObject)
        {
            if (varCameraObject == null)
            {
                Helper.LogError("Please set a Camera for translating!");
                return;
            }
            mCameraObject = varCameraObject;
        }

        public ScreenTag CreateScreenTag(GameObject varRelativeObj, Vector3 varOffset, GameObject varView)
        {
            if (mCameraObject == null)
            {
                Helper.LogError("Please set a Camera for translating!");
                return null;
            }
            if (varRelativeObj == null)
            {
                Helper.LogError("CreateScreenTag fail caused by null varRelativeObj");
                return null;
            }
            if (varView == null)
            {
                Helper.LogError("CreateScreenTag fail caused by null varView");
                return null;
            }
            //            GameObject tmpObj = new GameObject("SceneTag " + varView.GetHashCode());
            //            tmpObj.transform.SetParent (mTransform);
            varView.name = "SceneTag " + varView.GetHashCode();
            varView.transform.SetParent(mTransform);
            varView.transform.localScale = Vector3.one;
            ScreenTag tmpTag = varView.AddComponent<ScreenTag>();
            tmpTag.pOffset = varOffset;
            tmpTag.pRelativeObj = varRelativeObj;
            tmpTag.pView = varView;
            tmpTag.pCameraObj = mCameraObject;
            if (mScreenTags == null)
            {
                mScreenTags = new List<ScreenTag>();
            }
            mScreenTags.Add(tmpTag);
            return tmpTag;
        }

        public void RemoveScreenTag(ScreenTag varScreenTag)
        {
            if (varScreenTag == null)
            {
                return;
            }
            if (mScreenTags == null || mScreenTags.Count == 0)
            {
                return;
            }
            mScreenTags.Remove(varScreenTag);
        }

        public void RemoveAllScreenTags()
        {
            if (mScreenTags != null && mScreenTags.Count > 0)
            {
                for (int i = 0; i < mScreenTags.Count; i++)
                {
                    ScreenTag tmpTag = mScreenTags[i];
                    DestroyImmediate(tmpTag.gameObject);
                }
                mScreenTags.Clear();
            }
        }
        #endregion
    }
}
