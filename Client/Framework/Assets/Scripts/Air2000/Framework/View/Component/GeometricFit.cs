/*----------------------------------------------------------------
            // Copyright (C) 2015 南昌光速科技有限公司
            // 版权所有。 
            //
            // 文件名： GeometricFit.cs
            // 文件功能描述：对UIWidget做等比适配, 适用于全屏适配，超出屏幕部分，水平方向裁掉左边，竖直方向留中间
            //
			// 创建标识：陈伟超_20150527
            
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Air2000;

namespace QiuMo
{
	[RequireComponent(typeof(UIWidget))]
	public class GeometricFit : MonoBehaviour 
	{
		private UIWidget mWidget;

		void Awake () 
		{
			mWidget = GetComponent<UIWidget>();

			if(mWidget == null)
			{
				return;
			}

			FitParent();
			Fit();
		}

		void FitParent()
		{
			UIWidget tempWidget = transform.parent.GetComponent<UIWidget>();
			if(tempWidget == null)
			{
				return;
			}

            GameObject tempTarget = WindowManager.GetSingleton().pRootObject;
			Transform mTrans = tempTarget.transform;
			
			tempWidget.leftAnchor.target = mTrans;
			tempWidget.rightAnchor.target = mTrans;
			tempWidget.bottomAnchor.target = mTrans;
			tempWidget.topAnchor.target = mTrans;
			
			mWidget.ResetAnchors();
			
			SetAnchor(FTAnchor.HorizontalPivot.Right, tempWidget.leftAnchor, 0);
			SetAnchor(FTAnchor.HorizontalPivot.Right, tempWidget.rightAnchor, 0);
			SetAnchor(FTAnchor.VerticalPivot.Bottom, tempWidget.bottomAnchor, 0);
			SetAnchor(FTAnchor.VerticalPivot.Top, tempWidget.topAnchor, 0);
		}

		void Fit()
		{
            Vector2 tempScreenVec = WindowManager.GetSingleton().pScreenResolution;

			/// 屏幕宽/图片宽的比例与屏幕高/图片高的比例做比较，取最大值;
			float tempXRate = tempScreenVec.x / (float)mWidget.width;
			float temYRate = tempScreenVec.y / (float)mWidget.height;

			if(tempXRate > temYRate)
			{
				transform.localScale = new Vector3(tempXRate, tempXRate, 1);
			}
			else
			{
				transform.localScale = new Vector3(temYRate, temYRate, 1);
			}
		}

		void SetAnchor(FTAnchor.HorizontalPivot varPivot, UIRect.AnchorPoint varAnchorPoint, float varAbsolute)
		{
			if(varPivot == FTAnchor.HorizontalPivot.Left)
			{
				varAnchorPoint.Set(0f, varAbsolute);
			}
			else if(varPivot == FTAnchor.HorizontalPivot.Center)
			{
				varAnchorPoint.Set(0.5f, varAbsolute);
			}
			else
			{
				varAnchorPoint.Set(1f, varAbsolute);
			}
		}
		
		void SetAnchor(FTAnchor.VerticalPivot varPivot, UIRect.AnchorPoint varAnchorPoint, float varAbsolute)
		{
			if(varPivot == FTAnchor.VerticalPivot.Bottom)
			{
				varAnchorPoint.Set(0f, varAbsolute);
			}
			else if(varPivot == FTAnchor.VerticalPivot.Center)
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