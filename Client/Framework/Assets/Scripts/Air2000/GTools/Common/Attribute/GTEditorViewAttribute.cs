/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTEditorViewAttribute.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/27 15:54:09
            // Modify History:
            //
//----------------------------------------------------------------*/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public class GTDetailViewAttribute : Attribute
    {
        public Type ViewType { get; set; }
        public GTDetailViewAttribute(Type type)
        {
            this.ViewType = type;
        }
    }
}
#endif