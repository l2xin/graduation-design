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
    public abstract class Context : PropertyObject
    {
        [InternalInject]
        public ContextController Controllerer { get; set; }
        public ContextEventProcessor EventProcessor { get; set; }
        public Dictionary<Type, List<object>> Properties { get; set; }
        public Context()
        {
            EventProcessor = new ContextEventProcessor();
            Properties = new Dictionary<Type, List<object>>();
            List<object> list = new List<object>();
            list.Add(this);
            Properties.Add(this.GetType(), list);

            list = new List<object>();
            list.Add(EventProcessor);
            Properties.Add(typeof(ContextEventProcessor), list);

            //Properties = new List<object>();
            //Properties.Add(EventProcessor);
        }
        public ContextController GetController() { return Controllerer; }
        public void AddProperty(Type type, object prop)
        {
            if (Properties == null) Properties = new Dictionary<Type, List<object>>();

            List<object> props = null;
            if (Properties.TryGetValue(type, out props))
            {
                if (props == null) props = new List<object>();
                if (props.Contains(prop) == false)
                {
                    props.Add(prop);
                }
            }
            else
            {
                props = new List<object>();
                props.Add(prop);
                Properties.Add(type, props);
            }
        }
        public object GetProperty(Type type)
        {
            if (Properties == null) return null;
            List<object> props = null;
            if (Properties.TryGetValue(type, out props))
            {
                return props[0];
            }
            return null;
        }
        public void InternalInject()
        {
            if (Properties == null) return;
            Dictionary<Type, List<object>>.Enumerator it = Properties.GetEnumerator();
            for (int i = 0; i < Properties.Count; i++)
            {
                it.MoveNext();
                KeyValuePair<Type, List<object>> kvp = it.Current;
                List<object> props = kvp.Value;
                if (props != null && props.Count > 0)
                {
                    for (int j = 0; j < props.Count; j++)
                    {
                        object prop = props[j];
                        if (prop == null) continue;
                        MemberInfo[] memberInfos = prop.GetType().GetMembers();

                        if (memberInfos != null && memberInfos.Length > 0)
                        {
                            for (int k = 0; k < memberInfos.Length; k++)
                            {
                                MemberInfo memberInfo = memberInfos[k];
                                if (memberInfo == null) continue;
                                InternalInjectAttribute[] internalInjectAttribs = memberInfo.GetCustomAttributes(typeof(InternalInjectAttribute), false) as InternalInjectAttribute[];
                                if (internalInjectAttribs != null && internalInjectAttribs.Length > 0)
                                {
                                    InternalInjectAttribute internalInjectAttrib = internalInjectAttribs[0];
                                    object value = GetProperty(memberInfo.DeclaringType);
                                    prop.GetType().InvokeMember(memberInfo.Name, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, prop, new object[] { value });
                                }
                            }
                        }

                        //InternalInjectAttribute[] attribs =   if (memberInfos != null && memberInfos.Length > 0)
                        //{

                        //}
                        //prop.GetType().InvokeMember("ExecuteInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, prop, new object[] { context });
                    }
                }
            }
        }
        public void ExternalInject()
        {

        }

        public void UpdateDependency(Context context)
        {

        }
    }
}
