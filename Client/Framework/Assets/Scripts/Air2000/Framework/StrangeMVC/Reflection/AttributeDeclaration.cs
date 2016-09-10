﻿using System;
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
        public string PropertyName;
        public bool Override = false;
        public ContextPropertyAttribute(Type PropertyType, bool Override = false, string PropertyName = "")
        {
            this.PropertyType = PropertyType;
            this.Override = Override;
            this.PropertyName = PropertyName;
        }
    }

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



    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SingletonAttribute : Attribute
    {
        public SingletonAttribute() { }
    }
}
