/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ContextManager.cs
			// Describle:  Manage all context.
            // Created By:  hsu
			// Date&Time:  2016/3/3 19:20:03
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Air2000;

namespace Air2000.Module
{
    public class ContextManager
    {
        private static ContextManager m_Instance;
        private Dictionary<Type, List<object>> m_Contexts;
        public static ContextManager GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new ContextManager();
            }
            return m_Instance;
        }

        //public T CreateContext<T>() where T : Context
        //{
        //    if (typeof(T).IsAbstract == false)
        //    {
        //        object context = Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName);
        //        if (context == null)
        //        {
        //            Debug.LogError("ModuleManager::CreateContext: create context fail,type is " + typeof(T).FullName);
        //            return null;
        //        }
        //        FieldInfo[] fieldInfos = context.GetType().GetFields();
        //        MemberInfo[] memberInfos = context.GetType().GetMembers();
        //        object[] customAttribs = context.GetType().GetCustomAttributes(true);
        //        PropertyInfo[] propertyInfos = context.GetType().GetProperties();
        //        if (propertyInfos == null)
        //        {
        //            return null;
        //        }
        //        GTableAttribute tableAttr = null;
        //        PropertyInfo tableProperty = null;
        //        for (int i = 0; i < propertyInfos.Length; i++)
        //        {
        //            PropertyInfo propInfo = propertyInfos[i];
        //            if (propInfo == null)
        //            {
        //                return null;
        //            }
        //            tableAttr = Attribute.GetCustomAttribute(propInfo, typeof(GTableAttribute)) as GTableAttribute;
        //            if (tableAttr != null)
        //            {
        //                tableProperty = propInfo;
        //                break;
        //            }
        //        }

        //        //ContextSettingAttribute ctxSettingAttrib = typeof(T).
        //    }
        //    return default(T);
        //}

        public T Add<T>()
            where T : class
        {
            object context = Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName);
            return context as T;
        }
    }
}
