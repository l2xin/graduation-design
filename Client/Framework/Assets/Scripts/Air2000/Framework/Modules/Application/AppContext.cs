using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Modular;
using System.Reflection;

namespace Air2000
{
    public class AppContext : Context
    {
        private Dictionary<Type, object> m_Contexts;
        private static AppContext m_Instance;

        private AppContext() : base()
        {
            m_Instance = this;
        }
        public static AppContext GetInstance()
        {
            if (m_Instance == null)
            {
                new AppContext();
            }
            return m_Instance;
        }

        public T RegisterContext<T>()
            where T : Context
        {
            if (m_Contexts == null) m_Contexts = new Dictionary<Type, object>();
            object ctx = null;
            if (m_Contexts.TryGetValue(typeof(T), out ctx) == true)
            {
                return ctx as T;
            }
            object context = Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName);

            FieldInfo[] fieldInfos = context.GetType().GetFields();
            MemberInfo[] memberInfos = context.GetType().GetMembers();
            object[] customAttribs = context.GetType().GetCustomAttributes(true);

            //ContextLegacyPropertyAttribute[] legacyAttribs = Attribute.GetCustomAttributes(context.GetType(), typeof(ContextLegacyPropertyAttribute), true) as ContextLegacyPropertyAttribute[];
            //if (legacyAttribs != null && legacyAttribs.Length > 0)
            //{
            //    for (int i = 0; i < legacyAttribs.Length; i++)
            //    {
            //        ContextLegacyPropertyAttribute attrib = legacyAttribs[i];
            //        if (attrib == null) continue;
            //        object prop = Assembly.GetAssembly(attrib.PropertyType).CreateInstance(attrib.PropertyType.FullName);
            //        if (prop == null) continue;
            //        context.GetType().InvokeMember("AddProperty", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { attrib.LegacyPropertyType, prop });
            //    }
            //}

            ContextPropertyAttribute[] attribs = Attribute.GetCustomAttributes(context.GetType(), typeof(ContextPropertyAttribute), true) as ContextPropertyAttribute[];
            if (attribs != null && attribs.Length > 0)
            {
                for (int i = 0; i < attribs.Length; i++)
                {
                    ContextPropertyAttribute attrib = attribs[i];
                    if (attrib == null) continue;
                    object prop = Assembly.GetAssembly(attrib.PropertyType).CreateInstance(attrib.PropertyType.FullName);
                    if (prop == null) continue;
                    context.GetType().InvokeMember("AddProperty", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { new PropertyKey(attrib.PropertyType, attrib.IgnoreInject), prop });
                }
            }
            context.GetType().InvokeMember("InternalInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { });


            //if (memberInfos != null && memberInfos.Length > 0)
            //{

            //}

            //InternalInjectAttribute[] internalInjectAttribs = Attribute.GetCustomAttributes(context.GetType(), typeof(InternalInjectAttribute), true) as InternalInjectAttribute[];
            //if (internalInjectAttribs != null && internalInjectAttribs.Length > 0)
            //{
            //    for (int i = 0; i < internalInjectAttribs.Length; i++)
            //    {
            //        InternalInjectAttribute attrib = internalInjectAttribs[i];
            //        if (attrib == null) continue;

            //        object prop = Assembly.GetAssembly(attrib.PropertyType).CreateInstance(attrib.PropertyType.FullName);
            //        if (prop == null) continue;
            //        context.GetType().InvokeMember("AddProperty", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { attrib.PropertyType, prop });
            //    }
            //}


            //PropertyInfo[] propertyInfos = context.GetType().GetProperties();


            m_Contexts.Add(typeof(T), context);

            Dictionary<Type, object>.Enumerator it = m_Contexts.GetEnumerator();
            for (int i = 0; i < m_Contexts.Count; i++)
            {
                it.MoveNext();
                KeyValuePair<Type, object> kvp = it.Current;
                object tempContext = it.Current.Value;
                if (tempContext == null) continue;
                tempContext.GetType().InvokeMember("ExternalInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, tempContext, new object[] { this });

                //Dictionary<Type, object>.Enumerator it2 = m_Contexts.GetEnumerator();
                //for (int j = 0; j < m_Contexts.Count; j++)
                //{
                //    it2.MoveNext();
                //    KeyValuePair<Type, object> kvp2 = it2.Current;
                //    object tempContext2 = it2.Current.Value;
                //    if (tempContext2 == null) continue;
                //    if (tempContext2 == tempContext) continue;
                //    if (tempContext2 == context) continue;
                //    tempContext.GetType().InvokeMember("ExternalInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, tempContext, new object[] { this });
                //}
            }
            return context as T;
        }

        public T GetContext<T>()
            where T : Context
        {
            if (m_Contexts == null)
                return default(T);
            object ctx = null;
            m_Contexts.TryGetValue(typeof(T), out ctx);
            return ctx as T;
        }


        public object GetContext(Type contextType)
        {
            if (m_Contexts == null)
                return null;
            object ctx = null;
            m_Contexts.TryGetValue(contextType, out ctx);
            return ctx;
        }

        public void UnregisterContext()
        {

        }
    }
}
