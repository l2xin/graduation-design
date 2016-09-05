/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: IModelManagerListener.cs
			// Describle: 监听
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:45:09
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public interface IModelManagerListener
    {
        void OnOneModelInited(GTModel varModel);
        void OnAllModelInited();
        void OnOneModelRemoved(GTModel varModel);
        void OnAllModelRemoved();
    }
}
