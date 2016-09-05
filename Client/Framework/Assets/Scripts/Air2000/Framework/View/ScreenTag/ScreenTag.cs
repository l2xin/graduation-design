/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ScreenTag.cs
			// Describle: 屏幕标签
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
    public class ScreenTag : MonoBehaviour
    {
        public bool mIsUsing;
        private int mRefCount;
        private ScreenTagDel mDel;
        public GameObject mRelativeObj;
        public GameObject mView;
        public Vector3 mOffset;
        public GameObject mCameraObj;

        /// <summary>
        /// 当前标签是否在使用中
        /// </summary>
        public bool pIsUsing
        {
            get { return mIsUsing; }
            set { mIsUsing = value; }
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        public int pRefCount
        {
            get { return mRefCount; }
            set { mRefCount = value; }
        }

        /// <summary>
        /// 回调函数
        /// </summary>
        public ScreenTagDel pCallback
        {
            get { return mDel; }
            set { mDel = value; }
        }

        /// <summary>
        /// 依赖的物体
        /// </summary>
        public GameObject pRelativeObj
        {
            get { return mRelativeObj; }
            set { mRelativeObj = value; }
        }

        /// <summary>
        /// 视图对象
        /// </summary>
        public GameObject pView
        {
            get { return mView; }
            set { mView = value; }
        }

        /// <summary>
        /// 相对物体的偏移量
        /// </summary>
        public Vector3 pOffset
        {
            get { return mOffset; }
            set { mOffset = value; }
        }

        public GameObject pCameraObj
        {
            get { return mCameraObj; }
            set { mCameraObj = value; }
        }

        void Awake()
        {
        }

        void Start()
        {
        }

        void Update()
        {

        }

        void OnBecameVisible()
        {
            if (enabled == false)
            {
                enabled = true;
            }
        }

        void OnBecameInvisible()
        {
            enabled = false;
        }

        /// <summary>
        /// 在该函数中更新视图的位置
        /// </summary>
        void LateUpdate()
        {
            if (mCameraObj == null)
            {
                return;
            }
            if (mView == null || mRelativeObj == null)
            {
                return;
            }
            Camera tmpCamera = mCameraObj.GetComponentInChildren<Camera>();
            if (tmpCamera == null)
            {
                return;
            }

            Vector3 tmpTargetWorldPos = mRelativeObj.transform.position;
            tmpTargetWorldPos += mOffset; //计算目标的世界坐标

            Vector3 tmpScreenPos = tmpCamera.WorldToScreenPoint(tmpTargetWorldPos);//计算屏幕坐标
            tmpScreenPos.z = 0;
            tmpScreenPos.x -= (Screen.width / 2.0f);
            tmpScreenPos.y -= (Screen.height / 2.0f);

            tmpScreenPos *= WindowManager.GetSingleton().pScreenScale;
            transform.localPosition = tmpScreenPos;
        }

        void OnDestroy()
        {

        }
    }
}
