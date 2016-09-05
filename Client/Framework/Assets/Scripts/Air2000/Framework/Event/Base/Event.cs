/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Event.cs
			// Describle:事件基类
			// Created By:  hsu
			// Date&Time:  2016/1/19 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public abstract class BaseEvent
    {
        protected int mEventID;
        public int pEventID
        {
            get { return mEventID; }
            set { mEventID = value; }
        }
    }
}
