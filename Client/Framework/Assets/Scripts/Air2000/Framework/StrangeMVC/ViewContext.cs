using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Air2000.Module
{
    public abstract class ViewContext : Context
    {
        public ViewController ViewController { get; set; }

        public ViewContext() : base()
        {
        }
        //public T GetViewController<T>()
        //{
        //    GetViewController();
        //}
    }
}
