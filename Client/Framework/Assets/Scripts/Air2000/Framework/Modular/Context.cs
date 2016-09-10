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

namespace Air2000.Modular
{
    public class PropertyKey
    {
        public Type Type;
        //public string Name;
        public bool IgnoreInject = false;
        public PropertyKey(Type type, bool ignoreInject = false)
        {
            Type = type;
            IgnoreInject = ignoreInject;
        }
        public override bool Equals(object obj)
        {
            PropertyKey key = obj as PropertyKey;
            if (key != null)
            {
                return Type.Equals(key.Type);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
    public abstract class Context : PropertyObject
    {
        [IgnoreInject]
        public ContextEventProcessor EventProcessor { get; set; }
        [IgnoreInject]
        public Dictionary<PropertyKey, List<object>> m_Properties { get; set; }
        public Context()
        {
            EventProcessor = new ContextEventProcessor();
            m_Properties = new Dictionary<PropertyKey, List<object>>();
            List<object> list = new List<object>();

            // add current context to container
            list.Add(this);
            m_Properties.Add(new PropertyKey(this.GetType()), list);

            list = new List<object>();
            list.Add(EventProcessor);
            m_Properties.Add(new PropertyKey(typeof(ContextEventProcessor)), list);
        }

        public void AddProperty(PropertyKey key, object prop)
        {
            if (m_Properties == null) m_Properties = new Dictionary<PropertyKey, List<object>>();

            List<object> props = null;
            if (m_Properties.TryGetValue(key, out props))
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
                m_Properties.Add(key, props);
            }
        }
        public object GetProperty(Type type)
        {
            if (m_Properties == null) return null;
            PropertyKey key = new PropertyKey(type);
            List<object> props = null;
            if (m_Properties.TryGetValue(key, out props))
            {
                return props[0];
            }
            return null;
        }
        public void InternalInject()
        {
            if (m_Properties == null) return;
            Dictionary<PropertyKey, List<object>>.Enumerator it = m_Properties.GetEnumerator();
            for (int i = 0; i < m_Properties.Count; i++)
            {
                it.MoveNext();
                KeyValuePair<PropertyKey, List<object>> kvp = it.Current;
                if (kvp.Key.IgnoreInject)
                    continue;
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

        public void ExternalInject(AppContext context)
        {
            if (context == null) return;
            Dictionary<PropertyKey, List<object>>.Enumerator it = m_Properties.GetEnumerator();
            for (int i = 0; i < m_Properties.Count; i++)
            {
                it.MoveNext();
                KeyValuePair<PropertyKey, List<object>> kvp = it.Current;
                if (kvp.Key.IgnoreInject)
                    continue;
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
                                ExternalInjectAttribute[] externalInjectAttribs = propertyInfo.GetCustomAttributes(typeof(ExternalInjectAttribute), false) as ExternalInjectAttribute[];
                                if (externalInjectAttribs != null && externalInjectAttribs.Length > 0)
                                {
                                    ExternalInjectAttribute externalInjectAttrib = externalInjectAttribs[0];
                                    object targetContext = context.GetContext(externalInjectAttrib.ContextType);
                                    if (targetContext == null) continue;
                                    object value = null;

                                    value = targetContext.GetType().InvokeMember("GetProperty", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, targetContext, new object[] { propertyInfo.PropertyType });
                                    //if (externalInjectAttrib.RegionType != null)
                                    //{
                                    //    value = context.GetProperty(externalInjectAttrib.RegionType);
                                    //}
                                    //else
                                    //{
                                    //    value = context.GetProperty(propertyInfo.PropertyType);
                                    //}
                                    propertyInfo.SetValue(prop, value, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UpdateDependency(Context context)
        {

        }
    }
}
