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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ContextPropertyAttribute : Attribute
    {
        public Type PropertyType;
        public ContextPropertyAttribute(Type PropertyType)
        {
            this.PropertyType = PropertyType;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class InternalInjectAttribute : Attribute
    {
        public InternalInjectAttribute() { }

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ExternalInjectAttribute : Attribute
    {
        public Type ContextType;
        public ExternalInjectAttribute(Type ContextType)
        { this.ContextType = ContextType; }
    }



    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingletonAttribute : Attribute
    {
        public SingletonAttribute() { }
    }
}
