using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Module;
using System.Reflection;

namespace Air2000
{
    [Singleton]
    public class AppContext : Context
    {
        private Dictionary<Type, List<object>> m_Contexts;
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
            if (m_Contexts == null) m_Contexts = new Dictionary<Type, List<object>>();
            object context = Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName);

            FieldInfo[] fieldInfos = context.GetType().GetFields();
            MemberInfo[] memberInfos = context.GetType().GetMembers();
            object[] customAttribs = context.GetType().GetCustomAttributes(true);

            ContextPropertyAttribute[] attribs = Attribute.GetCustomAttributes(context.GetType(), typeof(ContextPropertyAttribute), true) as ContextPropertyAttribute[];
            if (attribs != null && attribs.Length > 0)
            {
                for (int i = 0; i < attribs.Length; i++)
                {
                    ContextPropertyAttribute attrib = attribs[i];
                    if (attrib == null) continue;
                    object prop = Assembly.GetAssembly(attrib.PropertyType).CreateInstance(attrib.PropertyType.FullName);
                    if (prop == null) continue;
                    context.GetType().InvokeMember("AddProperty", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { attrib.PropertyType, prop });
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

            List<object> objs = null;
            if (m_Contexts.TryGetValue(typeof(T), out objs))
            {

            }
            return context as T;
        }

        public void UnregisterContext()
        {

        }
    }
}
