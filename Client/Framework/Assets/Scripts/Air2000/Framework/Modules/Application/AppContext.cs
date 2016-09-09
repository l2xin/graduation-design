using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Module;
using System.Reflection;

namespace Air2000
{
    [Singleton]
    public class AppContext<T> : Context<T>
        where T : AppController
    {
        private Dictionary<Type, List<object>> m_Contexts;
        private static AppContext<T> m_Instance;

        private AppContext() : base()
        {
            m_Instance = this;
        }
        public static AppContext<T> GetInstance()
        {
            if (m_Instance == null)
            {
                new AppContext<T>();
            }
            return m_Instance;
        }

        public T1 RegisterContext<T1>()
            where T1 : class
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
                    object obj = Assembly.GetAssembly(attrib.PropertyType).CreateInstance(attrib.PropertyType.FullName);
                    if (obj == null) continue;
                    context.GetType().InvokeMember("AddProperty", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { attrib.PropertyType, obj });

                }
            }


            PropertyInfo[] propertyInfos = context.GetType().GetProperties();

            List<object> objs = null;
            if (m_Contexts.TryGetValue(typeof(T1), out objs))
            {

            }
            return context as T1;
        }

        public void UnregisterContext()
        {

        }
    }
}
