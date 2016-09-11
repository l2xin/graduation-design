using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Modular;
using System.Reflection;

namespace Air2000
{
    [ContextProperty(typeof(AppService))]
    public class AppContext : Context
    {
        private Dictionary<Type, object> AllContexts;
        public static AppContext Instance { get; set; }
        private AppContext() : base()
        {
            Instance = this;
            AllContexts = new Dictionary<Type, object>();
            List<object> list = new List<object>();
            list.Add(AllContexts);
            AddProperty(new PropertyKey(this.GetType(), true), list);

        }

        private static T AssemblyContext<T>(object context)
            where T : Context
        {
            object ctx = null;
            if (AppContext.Instance.AllContexts.TryGetValue(typeof(T), out ctx) == true)
            {
                return ctx as T;
            }

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
                    context.GetType().InvokeMember("AddProperty", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { new PropertyKey(attrib.PropertyType, attrib.IgnoreInject), prop });
                }
            }
            context.GetType().InvokeMember("InternalInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, context, new object[] { });

            AppContext.Instance.AllContexts.Add(typeof(T), context);

            Dictionary<Type, object>.Enumerator it = AppContext.Instance.AllContexts.GetEnumerator();
            for (int i = 0; i < AppContext.Instance.AllContexts.Count; i++)
            {
                it.MoveNext();
                KeyValuePair<Type, object> kvp = it.Current;
                object tempContext = it.Current.Value;
                if (tempContext == null) continue;
                tempContext.GetType().InvokeMember("ExternalInject", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, tempContext, new object[] { });
            }
            return context as T;
        }

        public static T RegisterContext<T>()
            where T : Context
        {
            if (AppContext.Instance == null)
            {
                AppContext obj = new AppContext();
                AssemblyContext<AppContext>(obj);
            }
            object context = Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName);
            return AssemblyContext<T>(context);
        }

        public static T GetContext<T>()
            where T : Context
        {
            if (AppContext.Instance.AllContexts == null)
                return default(T);
            object ctx = null;
            AppContext.Instance.AllContexts.TryGetValue(typeof(T), out ctx);
            return ctx as T;
        }


        public static object GetContext(Type contextType)
        {
            if (AppContext.Instance.AllContexts == null)
                return null;
            object ctx = null;
            AppContext.Instance.AllContexts.TryGetValue(contextType, out ctx);
            return ctx;
        }

        public void UnregisterContext()
        {

        }
    }
}
