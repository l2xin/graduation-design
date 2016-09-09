/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Context.cs
			// Describle: The abstract context
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
using System.Reflection;

namespace Air2000.Module
{
    public class Context<T>: PropertyObject
        where T : ContextController
    {
        [InternalInject]
        public ContextController Controllerer { get; set; }
        public ContextEventProcessor EventProcessor { get; set; }
        public Dictionary<Type, List<object>> Properties { get; set; }
        public Context()
        {
            Controllerer = Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName) as T;

            EventProcessor = new ContextEventProcessor();
            //Properties = new List<object>();
            //Properties.Add(EventProcessor);
        }
        public ContextController GetController() { return Controllerer; }
    }
}
