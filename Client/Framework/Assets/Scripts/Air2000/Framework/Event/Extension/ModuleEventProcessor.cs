/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: NetWorkEventManager.cs
			// Describle: Network event processor.
			// Created By:  hsu
			// Date&Time:  2016/1/19 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Module;

namespace Air2000
{
    public class ModuleEventProcessor : EventProcessor
    {
        private Context m_ModuleContext;
        public Context ModuleContext
        {
            get { return m_ModuleContext; }
        }
        private ModuleEventProcessor(Context context) { }
    }
}