using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Air2000.Modular
{
    public abstract class ViewContext : Context
    {
        //[InternalInject]
        //public ViewController ViewController { get; set; }
        public ViewContext() : base()
        {
        }
        //public T GetViewController<T>()
        //{
        //    GetViewController();
        //}
    }
}
