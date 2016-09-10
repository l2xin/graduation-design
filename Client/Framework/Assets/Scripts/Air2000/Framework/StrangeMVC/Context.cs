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
        public Dictionary<Type, List<object>> m_Properties { get; set; }
        public Context()
        {
            EventProcessor = new ContextEventProcessor();
            m_Properties = new Dictionary<Type, List<object>>();
            List<object> list = new List<object>();
            list.Add(this);
            m_Properties.Add(this.GetType(), list);

            list = new List<object>();
            list.Add(EventProcessor);
            m_Properties.Add(typeof(ContextEventProcessor), list);

            //Properties = new List<object>();
            //Properties.Add(EventProcessor);
        }
        public ContextController GetController() { return Controllerer; }
        public void AddProperty(Type type, object prop)
        {
            if (m_Properties == null) m_Properties = new Dictionary<Type, List<object>>();

            List<object> props = null;
            if (m_Properties.TryGetValue(type, out props))
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
                m_Properties.Add(type, props);
            }
        }
        public object GetProperty(Type type)
        {
            if (m_Properties == null) return null;
            List<object> props = null;
            if (m_Properties.TryGetValue(type, out props))
            {
                return props[0];
            }
            return null;
        }
        public void InternalInject()
        {
            if (m_Properties == null) return;
            Dictionary<Type, List<object>>.Enumerator it = m_Properties.GetEnumerator();
            for (int i = 0; i < m_Properties.Count; i++)
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
                        PropertyInfo[] propertyInfos = prop.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                        if (propertyInfos != null && propertyInfos.Length > 0)
                        {
                            for (int k = 0; k < propertyInfos.Length; k++)
                            {
                                PropertyInfo propertyInfo = propertyInfos[k];
                                if (propertyInfo == null) continue;
                                InternalInjectAttribute[] internalInjectAttribs = propertyInfo.GetCustomAttributes(typeof(InternalInjectAttribute), false) as InternalInjectAttribute[];
                                if (internalInjectAttribs != null && internalInjectAttribs.Length > 0)
                                {
                                    InternalInjectAttribute internalInjectAttrib = internalInjectAttribs[0];
                                    object value = null;
                                    if (internalInjectAttrib.RegionType != null)
                                    {
                                        value = GetProperty(internalInjectAttrib.RegionType);
                                    }
                                    else
                                    {
                                        value = GetProperty(propertyInfo.PropertyType);
                                    }
                                    propertyInfo.SetValue(prop, value, null);
                                }
                            }
                        }
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
