/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTableAttribute.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/24 14:12:03
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;

namespace Air2000
{
    public enum GTableDataCollectionType
    {
        List,
        Array,
        Dictionary,
    }
    public enum GTableBtnType
    {
        Toolbar_Add = 1,
        Toolbar_Delete,
        Toolbar_Clear,
        Toolbar_Copy,
        Toolbar_Paste,
        Toolbar_Custom1,
        Toolbar_Custom2,
        Toolbar_Custom3,
        Toolbar_CustomN = 20,

        Title_Help = 21,
        Title_Doc,
        Title_Custom1,
        Title_Custom2,
        Title_Custom3,
        Title_CustomN = 40,

        Entity_Help = 41,
        Entity_Doc,
        Entity_Detail,
        Entity_Foldout,
        Entity_Review,
        Entity_Custom1,
        Entity_Custom2,
        Entity_Custom3,
        Entity_CustomN = 60,
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class GTableButtonAttribute : Attribute
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public GTableBtnType ButtonType { get; set; }
        public string Text { get; set; }
        public string Tips { get; set; }
        public int ButtonNumber { get; set; }
        public GTableButtonAttribute() { }
    }


    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class GTableToolbarPopFieldAttribute : Attribute
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public string Text { get; set; }
        public float TextStrWidth { get; set; }
        public Type ParentType { get; set; }
        public GTableToolbarPopFieldAttribute() { }
    }



    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class GTableAttribute : Attribute
    {
        public int TableID { get; set; }
        public GTableDataCollectionType CollectionType { get; set; }
        public string TableViewInstance { get; set; }
        public GTableAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class GTableLayoutAttribute : Attribute
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float LinePadding { get; set; }
        public float ColumnPadding { get; set; }
        public GTableLayoutAttribute() { Height = 0; Width = 0; LinePadding = 0; ColumnPadding = 0; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class GTableToolbarAttribute : Attribute
    {

        public float Width { get; set; }
        public float Height { get; set; }
        public float MarginLeft { get; set; }
        public float MarginTop { get; set; }
        public string Name { get; set; }
        public float NameStrWidth { get; set; }
        public float ButtonCount { get; set; }
        public bool CreatePopField { get; set; }
        public GTableToolbarAttribute() { Name = string.Empty; CreatePopField = false; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class GTableTitleAttribute : Attribute
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float MarginLeft { get; set; }
        public Type MapType { get; set; }
        public bool CreateFoldoutBtn { get; set; }
        //public object[] FoldoutBtn { get; set; }
        //public object[] CustomBtnArgs { get; set; }
        public bool CreateDetailBtn { get; set; }
        public GTableTitleAttribute()
        {
            CreateFoldoutBtn = false;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class GTableEntityAttribute : Attribute
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public Type EntityType { get; set; }
        public GTableEntityAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class GTEntityFieldAttribute : Attribute
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public int FieldNumber { get; set; }
        public string FieldShowName { get; set; }
        public string FieldShowTips { get; set; }
        public bool Show { get; set; }
        public bool ReadOnly { get; set; }

        public GTEntityFieldAttribute()
        {
            Show = true;
            FieldShowName = string.Empty;
            FieldShowTips = string.Empty;
            ReadOnly = false;
        }
    }
}
