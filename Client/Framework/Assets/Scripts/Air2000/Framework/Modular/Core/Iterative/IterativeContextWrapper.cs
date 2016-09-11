using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Modular;
using System.Reflection;

namespace Air2000.Modular
{
    public class IterativeContextWrapper : Context
    {
        private Dictionary<string, object> m_ChildContexts;
        public string IdentifyName;

        public IterativeContextWrapper(string identifyName) : base()
        {
            this.IdentifyName = identifyName;
            m_ChildContexts = new Dictionary<string, object>();
            AddProperty(new PropertyKey(this.GetType(), true), m_ChildContexts);
        }

        private T AssemblyContext<T>(object context, string identifyName)
            where T : Context
        {
            object ctx = null;
            if (m_ChildContexts.TryGetValue(identifyName, out ctx) == true)
            {
                return ctx as T;
            }

            FieldInfo[] fieldInfos = context.GetType().GetFields();
            MemberInfo[] memberInfos = context.GetType().GetMembers();
            object[] customAttribs = context.GetType().GetCustomAttributes(true);

            RegisterPropertyAttribute[] attribs = Attribute.GetCustomAttributes(context.GetType(), typeof(RegisterPropertyAttribute), true) as RegisterPropertyAttribute[];
            if (attribs != null && attribs.Length > 0)
            {
                for (int i = 0; i < attribs.Length; i++)
                {
                    RegisterPropertyAttribute attrib = attribs[i];
                    if (attrib == null) continue;
                    object prop = Assembly.GetAssembly(attrib.PropertyType).CreateInstance(attrib.PropertyType.FullName);
                    if (prop == null) continue;
                    context.GetType().InvokeMember("AddProperty", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { new PropertyKey(attrib.PropertyType, attrib.IgnoreInject), prop });
                }
            }
            context.GetType().InvokeMember("InternalInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { });

            m_ChildContexts.Add(identifyName, context);

            Dictionary<string, object>.Enumerator it = m_ChildContexts.GetEnumerator();
            for (int i = 0; i < m_ChildContexts.Count; i++)
            {
                it.MoveNext();
                KeyValuePair<string, object> kvp = it.Current;
                object tempContext = it.Current.Value;
                if (tempContext == null) continue;
                tempContext.GetType().InvokeMember("ExternalInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, tempContext, new object[] { });
            }
            return context as T;
        }

        public T GetContext<T>(string identifyName)
            where T : IterativeContextWrapper
        {
            if (m_ChildContexts == null)
                return default(T);
            object ctx = null;
            m_ChildContexts.TryGetValue(identifyName, out ctx);
            return ctx as T;
        }

        public object GetContext(string identifyName)
        {
            if (m_ChildContexts == null)
                return null;
            object ctx = null;
            m_ChildContexts.TryGetValue(identifyName, out ctx);
            return ctx;
        }

        public T RegisterContext<T>(string identifyName)
            where T : IterativeContextWrapper
        {
            object context = Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName, true,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                null, new object[] { identifyName }, null, null);
            return AssemblyContext<T>(context, identifyName);
        }

        public void UnregisterContext<T>()
        {
        }

        public override void ExternalInject()
        {
            base.ExternalInject();
            if (m_ChildContexts != null || m_ChildContexts.Count > 0)
            {
                Dictionary<string, object>.Enumerator it = m_ChildContexts.GetEnumerator();
                for (int i = 0; i < m_ChildContexts.Count; i++)
                {
                    it.MoveNext();
                    KeyValuePair<string, object> kvp = it.Current;
                    object context = it.Current;
                    if (context == null) continue;
                    context.GetType().InvokeMember("ExternalInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { });
                }
            }
        }
    }
}
