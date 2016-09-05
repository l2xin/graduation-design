/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: LabelKey.cs
			// Describle: 本地化文本
			// Created By:  John
			// Date&Time:  2016年5月12日 09:17:30
            // Modify History:
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Air2000;

namespace GameLogic
{
	public class LabelKey : MonoBehaviour 
	{
		public string mLabelKey = string.Empty;

		void Awake()
		{
			if(string.IsNullOrEmpty(mLabelKey) == true)
			{
				Helper.LogError("!!! LabelKey.cs mLabelKey is NULL");
			}
			UIHelper.SetLabelText(this.transform,Localization.Get(mLabelKey));
		}
	}
}