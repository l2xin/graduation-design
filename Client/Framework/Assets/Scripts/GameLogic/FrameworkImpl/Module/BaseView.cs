/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: BaseView.cs
			// Describle:  视图
			// Created By:  hsu
			// Date&Time:  2016/3/3 20:25:02
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000;
using UnityEngine;

namespace GameLogic
{
    public class BaseView : GTView
    {
        public BaseModel pModel
        {
            get { return mBaseModel as BaseModel; }
            set { mBaseModel = value; }
        }
        public BaseListenerControl pListenerControl
        {
            get { return mListenerControl as BaseListenerControl; }
            set { mListenerControl = value; }
        }
    }
}
