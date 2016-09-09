/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: IModelListener.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/3 22:15:22
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public enum IModelListenerEventType
    {
        OnModelInited = 1,
        OnModelDestroyed = 2,
        OnViewAssigned = 3,
        OnModelStateChanged = 4,
    }
    public interface IModelListener
    {
        void OnModelInited();
        void OnModelDestroyed();
        void OnViewAssigned(GTView varView);
        void OnModelStateChanged(ModelState varNewModelState);
    }
}
