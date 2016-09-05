/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: LoadDialog.cs
			// Describle:
			// Created By:  John
			// Date&Time:  2016年5月30日 16:36:55
            // Modify History: 
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Air2000;

namespace GameLogic
{
	public delegate void LoadDialogDelegate(object varParam);

	public class LoadDialog : MonoBehaviour
	{
		private object mParam;
		private LoadDialogDelegate mDel;
		public static string UI_PREFAB_FILE_PATH = "Dialog/LoadDialog";

		///哈希值用于对应;
		public LoadDialog Init(string varTip,LoadDialogDelegate varCallBack,object varParam)
		{
			mParam = varParam;
			mDel = varCallBack;

            //LabelDot tempDot = Helper.GetComponent<LabelDot>(this.transform,"Loding/Label");

            //if(tempDot != null)
            //{
            //    tempDot.mText = varTip;
            //}

			return this;
		}


		public void EndLoad()
		{
			WindowManager.GetSingleton().DestoryWidowByName(UI_PREFAB_FILE_PATH);
			if(mDel != null)
			{
				mDel(mParam);
			}
		}


	}
}

