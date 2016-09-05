/*----------------------------------------------------------------
            // Copyright (C) 2015 南昌光速科技有限公司
            // 版权所有。 
            //
            // 文件名： FTAnchor.cs
            // 文件功能描述：对UIWidget做适配
            //
			// 创建标识：陈伟超_20150407
            
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Air2000;

namespace QiuMo
{
	[RequireComponent(typeof(UIWidget))]
	public class FTAnchor : MonoBehaviour 
	{
		public UIWidget mWidget;

		public enum HorizontalPivot
		{
			Left,
			Center,
			Right
		}

		public enum VerticalPivot
		{
			Bottom,
			Center,
			Top
		}

		public HorizontalPivot mLeftPivot;
		public int mLeft;

		public HorizontalPivot mRightPivot;
		public int mRight;

		public VerticalPivot mBottomPivot;
		public int mBottom;

		public VerticalPivot mTopPivot;
		public int mTop;

		private int i;

		void Awake () 
		{
			Adjust();
		}

		void OnDestroy()
		{
			mWidget = null;
		}

		public void Adjust()
		{
			if(mWidget == null)
			{
				mWidget = GetComponent<UIWidget>();
				if(mWidget == null)
				{
					return;
				}
			}

            GameObject tempTarget = WindowManager.GetSingleton().pRootObject;
			if (tempTarget == null) {
				return ;
			}
			Transform mTrans = tempTarget.transform;
			
			mWidget.leftAnchor.target = mTrans;
			mWidget.rightAnchor.target = mTrans;
			mWidget.bottomAnchor.target = mTrans;
			mWidget.topAnchor.target = mTrans;
			
			mWidget.ResetAnchors();
			
			SetAnchor(mLeftPivot, mWidget.leftAnchor, mLeft);
			SetAnchor(mRightPivot, mWidget.rightAnchor, mRight);
			SetAnchor(mBottomPivot, mWidget.bottomAnchor, mBottom);
			SetAnchor(mTopPivot, mWidget.topAnchor, mTop);
			
			
			//			mWidget.SetAnchor(tempTarget, mLeft, mBottom, mRight, mTop);
			//			mWidget.updateAnchors = UIRect.AnchorUpdate.OnStart;
		}

		void SetAnchor(HorizontalPivot varPivot, UIRect.AnchorPoint varAnchorPoint, float varAbsolute)
		{
			if(varPivot == HorizontalPivot.Left)
			{
				varAnchorPoint.Set(0f, varAbsolute);
			}
			else if(varPivot == HorizontalPivot.Center)
			{
				varAnchorPoint.Set(0.5f, varAbsolute);
			}
			else
			{
				varAnchorPoint.Set(1f, varAbsolute);
			}
		}

		void SetAnchor(VerticalPivot varPivot, UIRect.AnchorPoint varAnchorPoint, float varAbsolute)
		{
			if(varPivot == VerticalPivot.Bottom)
			{
				varAnchorPoint.Set(0f, varAbsolute);
			}
			else if(varPivot == VerticalPivot.Center)
			{
				varAnchorPoint.Set(0.5f, varAbsolute);
			}
			else
			{
				varAnchorPoint.Set(1f, varAbsolute);
			}
		}
		
	}
}