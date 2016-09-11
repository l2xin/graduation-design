using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000.Modular
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
        public bool IgnoreInject = false;
        public ContextPropertyAttribute(Type PropertyType, bool IgnoreInject = false, bool Singleton = true)
        {
            this.PropertyType = PropertyType;
            this.IgnoreInject = IgnoreInject;
        }
    }

    /// <summary>
    /// Legacy property for extension class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ContextLegacyPropertyAttribute : Attribute
    {
        public Type PropertyType;
        public Type LegacyPropertyType;
        public ContextLegacyPropertyAttribute(Type PropertyType, Type LegacyPropertyType)
        {
            this.PropertyType = PropertyType;
            this.LegacyPropertyType = LegacyPropertyType;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class InternalInjectAttribute : Attribute
    {
        public Type RegionType;
        public InternalInjectAttribute(Type RegionType = null) { this.RegionType = RegionType; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ExternalInjectAttribute : Attribute
    {
        public Type ContextType;
        public ExternalInjectAttribute(Type ContextType)
        { this.ContextType = ContextType; }
    }

    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    //public class IgnoreInjectAttribute : Attribute
    //{
    //    public IgnoreInjectAttribute() { }
    //}


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingletonAttribute : Attribute
    {
        public SingletonAttribute() { }
    }
}
