using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Air2000.Module
{
    public class ViewContext<T1, T2> : Context<T1>
        where T1 : ContextController
        where T2 : ViewController
    {
        public T2 ViewController { get; set; }

        public ViewContext() : base()
        {
            ViewController = Assembly.GetAssembly(typeof(T2)).CreateInstance(typeof(T2).FullName) as T2;
        }
        public T GetViewController<T>()
        {
            GetViewController();
        }
    }
}
