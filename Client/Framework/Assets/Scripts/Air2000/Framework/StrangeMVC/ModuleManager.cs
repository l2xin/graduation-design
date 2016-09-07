/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ModuleManager.cs
			// Describle:  Manage all modules.
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
    public class ModuleManager
    {
        private static ModuleManager m_Instance;

        public static ModuleManager GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new ModuleManager();
            }
            return m_Instance;
        }

        public T CreateContext<T>() where T : Context
        {
            if (typeof(T).IsAbstract == false)
            {
                object context = Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName);
                if (context == null)
                {
                    Debug.LogError("ModuleManager::CreateContext: create context fail,type is " + typeof(T).FullName);
                    return null;
                }

                //PropertyInfo[] propertiesInfo = obj.GetType().GetProperties();
                //if (propertiesInfo == null)
                //{
                //    return null;
                //}
                //GTableAttribute tableAttr = null;
                //PropertyInfo tableProperty = null;
                //for (int i = 0; i < propertiesInfo.Length; i++)
                //{
                //    PropertyInfo propInfo = propertiesInfo[i];
                //    if (propInfo == null)
                //    {
                //        return null;
                //    }
                //    tableAttr = Attribute.GetCustomAttribute(propInfo, typeof(GTableAttribute)) as GTableAttribute;
                //    if (tableAttr != null)
                //    {
                //        tableProperty = propInfo;
                //        break;
                //    }
                //}

                //ContextSettingAttribute ctxSettingAttrib = typeof(T).
            }
            return default(T);
        }
    }
}
