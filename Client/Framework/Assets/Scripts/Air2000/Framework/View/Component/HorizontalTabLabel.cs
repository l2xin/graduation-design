/*----------------------------------------------------------------
            // Copyright (C) 20015 南昌光速科技有限公司
            // 版权所有。 
            //
            // 文件名： HorizontalTabLabel.cs
            // 文件功能描述：处理水平方向的Toggle，按下文字变黄, 抬起文字变蓝效果;
            // 
            // 创建标识：陈伟超_20150528

//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

namespace QiuMo
{
	[RequireComponent(typeof(UILabel))]
	public class HorizontalTabLabel : MonoBehaviour
	{
		public UIToggle mToggle;
		public UILabel mLabel; 

		void Awake () 
		{
			if(mLabel == null)
			{
				mLabel = GetComponent<UILabel>();
			}
			
			if(mToggle == null)
			{
				mToggle = NGUITools.FindInParents<UIToggle>(gameObject);
			}

			if(mToggle != null)
			{
				EventDelegate.Add(mToggle.onChange, ChangeValue);
			}
		}

		public void ChangeValue()
		{
			if(mToggle == null)
			{
				return;
			}

			if(mToggle == UIToggle.current)
			{
				if(UIToggle.current.value)
				{
					// 选中状态;
					SwitchCheck();
				}
				else
				{
					// 未选中状态;
					SwitchUnCheck();
				}
			}
		}

		private void SwitchCheck()
		{
			if(mLabel == null)
			{
				return;
			}

			mLabel.effectStyle = UILabel.Effect.Outline;
			mLabel.effectDistance = 0.5f * Vector2.one;

			Color tempColor = Color.white;

			tempColor.r = 46f / 255f;
			tempColor.g = 78f / 255f;
			tempColor.b = 103f / 255f;

			mLabel.effectColor = tempColor;

			tempColor.r = 1f;
			tempColor.g = 213f / 255f;
			tempColor.b = 104f / 255f;

			mLabel.color = tempColor;

			if(mLabel.fontSize != 26)
			{
				mLabel.fontSize = 26;
				mLabel.AssumeNaturalSize();
			}
		}

		private void SwitchUnCheck()
		{
			if(mLabel == null)
			{
				return;
			}

			mLabel.effectStyle = UILabel.Effect.None;

			Color tempColor = Color.white;

			tempColor.r = 33f / 255f;
			tempColor.g = 67f / 255f;
			tempColor.b = 94f / 255f;
			
			mLabel.color = tempColor;

			if(mLabel.fontSize != 26)
			{
				mLabel.fontSize = 26;
				mLabel.AssumeNaturalSize();
			}
		}
		
	}
}

