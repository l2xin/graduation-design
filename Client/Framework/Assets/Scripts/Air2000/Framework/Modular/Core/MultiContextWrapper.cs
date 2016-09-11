using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Modular;
using System.Reflection;

namespace Air2000.Modular
{
    public class IterationContextWrapper : Context
    {
        private Dictionary<Type, object> ChildContexts;

        public IterationContextWrapper() : base()
        {
            ChildContexts = new Dictionary<Type, object>();
            AddProperty(new PropertyKey(this.GetType(), true), ChildContexts);
        }

        private T AssemblyContext<T>(object context)
            where T : Context
        {
            object ctx = null;
            if (ChildContexts.TryGetValue(typeof(T), out ctx) == true)
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

            ChildContexts.Add(typeof(T), context);

            Dictionary<Type, object>.Enumerator it = ChildContexts.GetEnumerator();
            for (int i = 0; i < ChildContexts.Count; i++)
            {
                it.MoveNext();
                KeyValuePair<Type, object> kvp = it.Current;
                object tempContext = it.Current.Value;
                if (tempContext == null) continue;
                tempContext.GetType().InvokeMember("ExternalInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, tempContext, new object[] { });
            }
            return context as T;
        }

        public T GetContext<T>()
            where T : IterationContextWrapper
        {
            if (ChildContexts == null)
                return default(T);
            object ctx = null;
            ChildContexts.TryGetValue(typeof(T), out ctx);
            return ctx as T;
        }

        public object GetContext(Type contextType)
        {
            if (ChildContexts == null)
                return null;
            object ctx = null;
            ChildContexts.TryGetValue(contextType, out ctx);
            return ctx;
        }

        public T RegisterContext<T>()
            where T : IterationContextWrapper
        {
            object context = Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName);
            return AssemblyContext<T>(context);
        }

        public void UnregisterContext<T>()
        {
        }

        public override void ExternalInject()
        {
            base.ExternalInject();
        }
    }
}
