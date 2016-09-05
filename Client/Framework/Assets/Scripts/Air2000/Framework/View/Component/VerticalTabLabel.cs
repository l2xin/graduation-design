/*----------------------------------------------------------------
            // Copyright (C) 20015 南昌光速科技有限公司
            // 版权所有。 
            //
            // 文件名： VerticalTabLabel.cs
            // 文件功能描述：处理Toggle，按下文字变黄, 抬起文字变白效果;
            // 
            // 创建标识：陈伟超_20150528

//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

namespace QiuMo
{
	[RequireComponent(typeof(UILabel))]
	public class VerticalTabLabel : MonoBehaviour
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
		
			SwitchOutLine();
		
			Color tempColor = Color.white;
			tempColor.r = 254f / 255f;
			tempColor.g = 244f / 255f;
			tempColor.b = 45f / 255f;
			
			mLabel.color = tempColor;
		}
		
		private void SwitchUnCheck()
		{
			if(mLabel == null)
			{
				return;
			}
			
			SwitchOutLine();
			
			Color tempColor = Color.white;
			mLabel.color = tempColor;
		}

		void SwitchOutLine()
		{
			if(mLabel.effectStyle != UILabel.Effect.Outline)
			{
				mLabel.effectStyle = UILabel.Effect.Outline;
				mLabel.effectDistance = 0.5f * Vector2.one;
				
				Color tempColor = Color.white;
				
				tempColor.r = 46f / 255f;
				tempColor.g = 78f / 255f;
				tempColor.b = 103f / 255f;
				
				mLabel.effectColor = tempColor;
			}
		}
		
	}
}

