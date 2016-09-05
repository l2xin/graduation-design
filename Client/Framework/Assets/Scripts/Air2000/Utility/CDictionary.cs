/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: CDictionary.cs
			// Describle: 自定义的字典类
			// Created By:  ZQ-PC
			// Date&Time:  2016/1/27 10:31:43
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    /// <summary>
    /// 自定义的字典类,提供比较引用类型Key是否相等的方法
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class CDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public bool ContainsObjectKey(TKey key)
        {
            if (key == null)
            {
                return false;
            }
            Enumerator em = GetEnumerator();
            for (int i = 0; i < Count; i++)
            {
                em.MoveNext();
                KeyValuePair<TKey, TValue> kvp = em.Current;
                if (key.Equals(kvp.Key))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
