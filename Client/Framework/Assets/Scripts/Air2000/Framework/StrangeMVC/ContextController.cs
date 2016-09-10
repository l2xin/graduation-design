﻿/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ContextController.cs
			// Describle: The context controllers
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:20:03
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000;

namespace Air2000.Module
{
    public abstract class ContextController : PropertyObject
    {
        public EventHandlerQueue AppEventHandlersQueue;
        public EventHandlerQueue NetworkEventHandlersQueue;
        public EventHandlerQueue PlayerEventHandlersQueue;
    }
}
