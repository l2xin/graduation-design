using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000.Module
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ContextSettingAttribute : Attribute
    {
        public Type ModuleControllerType { get; set; }
        public Type ViewControllerType { get; set; }
        public ContextSettingAttribute(Type ModuleControllerType, Type ViewControllerType)
        {
            this.ModuleControllerType = ModuleControllerType;
            this.ViewControllerType = ViewControllerType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NotSingletonAttribute : Attribute
    {
        public NotSingletonAttribute() { }
    }
}
