/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GTEditorTableView.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/3/24 11:19:05
            // Modify History:
            //
//----------------------------------------------------------------*/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Air2000
{
    #region define clazz & enum
    public interface GTEditorTableViewListener
    {
        void OnFoldoutButtonClick(params object[] args);
        void OnEntityButtonClick(params object[] args);
        void OnToolbarButtonClick(params object[] args);
        void OnShowSubTable(params object[] args);
    }

    public enum GTEditorTableViewListenerEvtType
    {


        OnFoldoutButtonClick,
        OnEntityButtonClick,
        OnToolbarButtonClick,
        OnShowSubTable,
    }
    #endregion

    public abstract class GTEditorTableView
    {
        #region define member
        protected List<GTEditorTableViewListener> Listeners;
        public GTEditorTableViewListener SingleListener { get; set; }
        #endregion

        #region define function for listener
        public void AddListener(GTEditorTableViewListener varListener)
        {
            if (Listeners.Contains(varListener))
            {
                return;
            }
            Listeners.Add(varListener);
        }
        public void RemoveListener(GTEditorTableViewListener varListener)
        {
            Listeners.Remove(varListener);
        }
        public void RemoveAllListener()
        {
            Listeners.Clear();
        }
        public void NotifyListener(GTEditorTableViewListenerEvtType varEventType, params object[] args)
        {
            if (Listeners != null && Listeners.Count > 0)
            {
                for (int i = 0; i < Listeners.Count; i++)
                {
                    GTEditorTableViewListener listener = Listeners[i];
                    if (listener == null)
                    {
                        continue;
                    }
                    switch (varEventType)
                    {
                        case GTEditorTableViewListenerEvtType.OnFoldoutButtonClick:
                            listener.OnFoldoutButtonClick(args);
                            break;
                        case GTEditorTableViewListenerEvtType.OnEntityButtonClick:
                            listener.OnEntityButtonClick(args);
                            break;
                        case GTEditorTableViewListenerEvtType.OnToolbarButtonClick:
                            listener.OnToolbarButtonClick(args);
                            break;
                        case GTEditorTableViewListenerEvtType.OnShowSubTable:
                            listener.OnShowSubTable(args);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region define virtual method
        public virtual void OnCreate() { }
        protected virtual void InitView(GTableAttribute varTableAttr, GTableLayoutAttribute varTableLayoutAttr,
            GTableToolbarAttribute varTableToolbarAttr, GTableTitleAttribute varTableTitleAttr,
            GTableEntityAttribute varTableEntityAttr, PropertyInfo varTableProperty,
            List<GTableButtonAttribute> varToolbarBtnAtts, List<GTableButtonAttribute> varTitleBtnAttrs,
            GTableButtonAttribute varFoldoutBtnAttr, GTableButtonAttribute varDetailBtnAttr,
            GTableToolbarPopFieldAttribute varTableToolbarPopFieldAttr) { }
        public virtual void OnDestroy() { }
        #endregion

        #region define util function

        #endregion

        #region define public static function
        public static GTEditorTableView DrawTableView(object obj, GTEditorTableViewListener varListener)
        {
            if (obj == null)
            {
                return null;
            }
            PropertyInfo[] propertiesInfo = obj.GetType().GetProperties();
            if (propertiesInfo == null)
            {
                return null;
            }
            GTableAttribute tableAttr = null;
            PropertyInfo tableProperty = null;
            for (int i = 0; i < propertiesInfo.Length; i++)
            {
                PropertyInfo propInfo = propertiesInfo[i];
                if (propInfo == null)
                {
                    return null;
                }
                tableAttr = Attribute.GetCustomAttribute(propInfo, typeof(GTableAttribute)) as GTableAttribute;
                if (tableAttr != null)
                {
                    tableProperty = propInfo;
                    break;
                }
            }
            if (tableAttr == null)
            { return null; }
            if (string.IsNullOrEmpty(tableAttr.TableViewInstance))
            {
                return null;
            }
            PropertyInfo tableViewPropertyInfo = obj.GetType().GetProperty(tableAttr.TableViewInstance);
            if (tableViewPropertyInfo == null)
            {
                return null;
            }
            GTEditorTableView tableView = tableViewPropertyInfo.GetValue(obj, null) as GTEditorTableView;
            if (tableView == null)
            {
                return null;
            }
            GTableLayoutAttribute layoutAttr = Attribute.GetCustomAttribute(tableProperty, typeof(GTableLayoutAttribute)) as GTableLayoutAttribute;
            GTableToolbarAttribute toolbarAttr = Attribute.GetCustomAttribute(tableProperty, typeof(GTableToolbarAttribute)) as GTableToolbarAttribute;
            GTableTitleAttribute titleAttr = Attribute.GetCustomAttribute(tableProperty, typeof(GTableTitleAttribute)) as GTableTitleAttribute;
            GTableEntityAttribute entity = Attribute.GetCustomAttribute(tableProperty, typeof(GTableEntityAttribute)) as GTableEntityAttribute;
            GTableToolbarPopFieldAttribute toolbarPopFieldAttr = Attribute.GetCustomAttribute(tableProperty, typeof(GTableToolbarPopFieldAttribute)) as GTableToolbarPopFieldAttribute;
            List<GTableButtonAttribute> toolbarBtnAttrs = new List<GTableButtonAttribute>();
            List<GTableButtonAttribute> titleBtnAttrs = new List<GTableButtonAttribute>();
            GTableButtonAttribute foldoutBtnAttr = null;
            GTableButtonAttribute detailBtnAttr = null;
            GTableButtonAttribute[] btnAttrs = Attribute.GetCustomAttributes(tableProperty, typeof(GTableButtonAttribute)) as GTableButtonAttribute[];
            if (btnAttrs != null && btnAttrs.Length > 0)
            {
                for (int i = 0; i < btnAttrs.Length; i++)
                {
                    GTableButtonAttribute attr = btnAttrs[i];
                    if (attr == null)
                    {
                        continue;
                    }
                    if (attr.ButtonType == GTableBtnType.Entity_Foldout)
                    {
                        foldoutBtnAttr = attr;
                        continue;
                    }
                    if (attr.ButtonType == GTableBtnType.Entity_Detail)
                    {
                        detailBtnAttr = attr;
                        continue;
                    }
                    if ((int)GTableBtnType.Toolbar_Add <= (int)attr.ButtonType && (int)attr.ButtonType <= (int)GTableBtnType.Toolbar_CustomN)
                    {
                        toolbarBtnAttrs.Add(attr);
                    }
                    else if ((int)GTableBtnType.Title_Help <= (int)attr.ButtonType && (int)attr.ButtonType <= (int)GTableBtnType.Title_CustomN)
                    {
                        titleBtnAttrs.Add(attr);
                    }
                }
            }
            tableView.InitView(tableAttr, layoutAttr, toolbarAttr, titleAttr, entity, tableProperty, toolbarBtnAttrs, titleBtnAttrs, foldoutBtnAttr, detailBtnAttr, toolbarPopFieldAttr);
            tableView.SingleListener = varListener;
            tableView.AddListener(varListener);
            return tableView;
        }
        #endregion
    }
    public class GTEditorTableViewEx<TParentType, TableEntityType> : GTEditorTableView
    {
        #region define inner clazz
        public class EntityFieldInfo
        {
            public GTEntityFieldAttribute Attribute;
            public GTableButtonAttribute[] BtnAttrArray;
            public PropertyInfo PropertyInfo;
        }
        #endregion

        #region define member
        public TParentType TableParentData { get; set; }
        public List<TableEntityType> ListTableData { get; set; }
        public List<TableEntityType> SelectedTableData { get; set; }
        private Vector2 Scroll = Vector2.zero;
        public float MarginLeft { get; set; }
        public float ToolbarMarginLeft { get; set; }
        public float ToolbarMarginTop { get; set; }
        public bool SelectAllLines { get; set; }
        public bool LastSelectAllLines { get; set; }
        public Type TargetCreateType { get; set; }
        public int TargetCreateTypeNameIndex { get; set; }
        #endregion

        #region define constructor
        public GTEditorTableViewEx(TParentType TableParentData, List<TableEntityType> TableData)
        {
            Listeners = new List<GTEditorTableViewListener>();
            this.TableParentData = TableParentData;
            this.ListTableData = TableData;
            SelectedTableData = new List<TableEntityType>();
        }
        #endregion

        #region define view function
        protected override void InitView(GTableAttribute varTableAttr, GTableLayoutAttribute varTableLayoutAttr,
            GTableToolbarAttribute varTableToolbarAttr, GTableTitleAttribute varTableTitleAttr,
            GTableEntityAttribute varTableEntityAttr, PropertyInfo varTableProperty,
            List<GTableButtonAttribute> varToolbarBtnAtts, List<GTableButtonAttribute> varTitleBtnAttrs
            , GTableButtonAttribute varFoldoutBtnAttr, GTableButtonAttribute varDetailBtnAttr,
            GTableToolbarPopFieldAttribute varTableToolbarPopFieldAttr)
        {
            if (TableParentData == null || ListTableData == null)
            {
                return;
            }
            if (varTableLayoutAttr == null || varTableAttr == null || varTableTitleAttr == null || varTableEntityAttr == null || varTableProperty == null)
            {
                return;
            }

            if (varTableLayoutAttr.Height <= 0)
            {
                GUILayout.BeginVertical("box", GUILayout.Width(varTableLayoutAttr.Width));
            }
            else
            {
                GUILayout.BeginVertical("box", GUILayout.Width(varTableLayoutAttr.Width), GUILayout.Height(varTableLayoutAttr.Height));
            }

            if (varTableToolbarAttr != null)
            {
                ToolbarMarginLeft = varTableToolbarAttr.MarginLeft;
                ToolbarMarginTop = varTableToolbarAttr.MarginTop;
                if (varTableToolbarAttr.Height <= 0.0f)
                {
                    GUILayout.BeginHorizontal("box", GUILayout.Width(varTableToolbarAttr.Width));
                    GUILayout.Space(ToolbarMarginLeft);
                }
                else
                {
                    GUILayout.BeginHorizontal("box", GUILayout.Width(varTableToolbarAttr.Width), GUILayout.Height(varTableToolbarAttr.Height));
                    GUILayout.Space(ToolbarMarginLeft);
                }
                if (string.IsNullOrEmpty(varTableToolbarAttr.Name) == false)
                {
                    EditorGUILayout.LabelField(varTableToolbarAttr.Name, GUILayout.Width(varTableToolbarAttr.NameStrWidth));
                }
                if (varTableToolbarAttr.CreatePopField == true)
                {
                    if (varTableToolbarPopFieldAttr != null && varTableToolbarPopFieldAttr.ParentType != null && varTableToolbarPopFieldAttr.Width > 0)
                    {
                        if (string.IsNullOrEmpty(varTableToolbarPopFieldAttr.Text) && varTableToolbarPopFieldAttr.TextStrWidth > 0)
                        {
                            EditorGUILayout.LabelField(varTableToolbarPopFieldAttr.Text, GUILayout.Width(varTableToolbarPopFieldAttr.TextStrWidth));
                        }
                        List<Type> allType = Helper.GetAllSubClass(varTableToolbarPopFieldAttr.ParentType);

                        if (allType != null && allType.Count > 0)
                        {
                            List<string> allTypeName = new List<string>();
                            for (int i = 0; i < allType.Count; i++)
                            {
                                Type type = allType[i];
                                if (type == null) continue;
                                string typeName = Helper.GetTypeNameWithoutNamespcae(type.FullName);
                                allTypeName.Add(typeName);
                            }
                            if (allTypeName != null && allTypeName.Count > 0)
                            {
                                if (varTableToolbarPopFieldAttr.Height <= 0)
                                {
                                    TargetCreateTypeNameIndex = EditorGUILayout.Popup(TargetCreateTypeNameIndex, allTypeName.ToArray(), GUILayout.Width(varTableToolbarPopFieldAttr.Width));
                                }
                                else
                                {
                                    TargetCreateTypeNameIndex = EditorGUILayout.Popup(TargetCreateTypeNameIndex, allTypeName.ToArray(), GUILayout.Width(varTableToolbarPopFieldAttr.Width), GUILayout.Height(varTableToolbarPopFieldAttr.Height));
                                }

                                if (TargetCreateTypeNameIndex <= allType.Count)
                                {
                                    TargetCreateType = allType[TargetCreateTypeNameIndex];
                                }
                            }
                        }
                    }
                }
                varToolbarBtnAtts = SortButton(varToolbarBtnAtts);
                if (varToolbarBtnAtts != null && varToolbarBtnAtts.Count > 0)
                {
                    for (int i = 0; i < varToolbarBtnAtts.Count; i++)
                    {
                        GTableButtonAttribute toolbarBtnAttr = varToolbarBtnAtts[i];
                        if (toolbarBtnAttr == null || toolbarBtnAttr.Width <= 0f || string.IsNullOrEmpty(toolbarBtnAttr.Text))
                        {
                            continue;
                        }
                        if (toolbarBtnAttr.Height <= 0f)
                        {
                            if (GUILayout.Button(new GUIContent(toolbarBtnAttr.Text, toolbarBtnAttr.Tips), GUILayout.Width(toolbarBtnAttr.Width)))
                            {
                                OnTableBtnClick(toolbarBtnAttr, toolbarBtnAttr.ButtonType);
                            }
                        }
                        else
                        {
                            if (GUILayout.Button(new GUIContent(toolbarBtnAttr.Text, toolbarBtnAttr.Tips), GUILayout.Width(toolbarBtnAttr.Width), GUILayout.Height(toolbarBtnAttr.Height)))
                            {
                                OnTableBtnClick(toolbarBtnAttr, toolbarBtnAttr.ButtonType);
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();

                if (varTableTitleAttr != null)
                {
                    MarginLeft = varTableTitleAttr.MarginLeft;
                    bool ShowFoldoutBtn = false;
                    float FoldoutBtnWidth = 0.0f;
                    float FoldoutBtnHeight = 0.0f;
                    string FoldoutBtnText = string.Empty;
                    string FoldoutBtnTooltips = string.Empty;


                    bool ShowDetailBtn = false;
                    float DetailBtnWidth = 0.0f;
                    float DetailBtnHeight = 0.0f;
                    string DetailBtnText = string.Empty;
                    string DetailBtnTooltips = string.Empty;

                    if (varTableTitleAttr.CreateFoldoutBtn)
                    {
                        if (varFoldoutBtnAttr != null && varFoldoutBtnAttr.Width > 0 && string.IsNullOrEmpty(varFoldoutBtnAttr.Text) == false)
                        {
                            FoldoutBtnHeight = varFoldoutBtnAttr.Height;
                            FoldoutBtnWidth = varFoldoutBtnAttr.Width;
                            FoldoutBtnText = varFoldoutBtnAttr.Text;
                            FoldoutBtnTooltips = varFoldoutBtnAttr.Tips;
                            ShowFoldoutBtn = true;
                        }
                        else
                        {
                            varTableTitleAttr.CreateFoldoutBtn = false;
                            ShowFoldoutBtn = false;
                        }
                    }

                    if (varTableTitleAttr.CreateDetailBtn)
                    {
                        if (varDetailBtnAttr != null && varDetailBtnAttr.Width > 0 && string.IsNullOrEmpty(varDetailBtnAttr.Text) == false)
                        {
                            DetailBtnHeight = varDetailBtnAttr.Height;
                            DetailBtnWidth = varDetailBtnAttr.Width;
                            DetailBtnText = varDetailBtnAttr.Text;
                            DetailBtnTooltips = varDetailBtnAttr.Tips;
                            ShowDetailBtn = true;
                        }
                        else
                        {
                            varTableTitleAttr.CreateDetailBtn = false;
                            ShowDetailBtn = false;
                        }
                    }


                    if (varTableEntityAttr != null)
                    {
                        List<EntityFieldInfo> entityFieldInfos = new List<EntityFieldInfo>();
                        PropertyInfo[] entityProperties = varTableEntityAttr.EntityType.GetProperties();
                        if (entityProperties == null || entityProperties.Length == 0)
                        {
                            return;
                        }
                        for (int i = 0; i < entityProperties.Length; i++)
                        {
                            PropertyInfo propInfo = entityProperties[i];
                            if (propInfo == null)
                            {
                                continue;
                            }
                            GTEntityFieldAttribute attr = Attribute.GetCustomAttribute(propInfo, typeof(GTEntityFieldAttribute)) as GTEntityFieldAttribute;
                            if (attr != null)
                            {
                                EntityFieldInfo entityFieldInfo = new EntityFieldInfo() { Attribute = attr, PropertyInfo = propInfo };
                                entityFieldInfos.Add(entityFieldInfo);
                                GTableButtonAttribute[] btnAttrs = Attribute.GetCustomAttributes(propInfo, typeof(GTableButtonAttribute)) as GTableButtonAttribute[];
                                if (btnAttrs != null && btnAttrs.Length > 0)
                                {
                                    entityFieldInfo.BtnAttrArray = btnAttrs;
                                }
                            }
                        }
                        entityFieldInfos = SortEntityField(entityFieldInfos);
                        if (entityFieldInfos != null && entityFieldInfos.Count > 0)
                        {
                            GUILayout.Space(3.0f);
                            GUILayout.BeginHorizontal(GUILayout.Width(varTableTitleAttr.Width));
                            GUILayout.Space(MarginLeft);
                            if (ShowFoldoutBtn)
                            {
                                GUILayout.Space(FoldoutBtnWidth);
                            }
                            if (ShowDetailBtn)
                            {
                                GUILayout.Space(DetailBtnWidth);
                            }
                            for (int i = 0; i < entityFieldInfos.Count; i++)
                            {
                                EntityFieldInfo fieldInfo = entityFieldInfos[i];
                                GTEntityFieldAttribute fieldAttr = fieldInfo.Attribute;
                                if (fieldAttr == null)
                                {
                                    continue;
                                }
                                PropertyInfo propertyInfo = fieldInfo.PropertyInfo;
                                if (propertyInfo == null)
                                {
                                    continue;
                                }
                                if (propertyInfo.Name.Equals("Toggle"))
                                {
                                    LastSelectAllLines = SelectAllLines;
                                    SelectAllLines = EditorGUILayout.Toggle(SelectAllLines, GUILayout.Width(fieldAttr.Width));
                                    continue;
                                }
                                if (fieldAttr.Height <= 0)
                                {
                                    EditorGUILayout.LabelField(new GUIContent(fieldAttr.FieldShowName, fieldAttr.FieldShowTips), GUILayout.Width(fieldAttr.Width));
                                }
                                else
                                {
                                    EditorGUILayout.LabelField(new GUIContent(fieldAttr.FieldShowName, fieldAttr.FieldShowTips), GUILayout.Width(fieldAttr.Width), GUILayout.Height(fieldAttr.Height));

                                }
                            }
                            GUILayout.EndHorizontal();
                            bool showDetailView = false;
                            for (int i = 0; i < ListTableData.Count; i++)
                            {
                                object entityData = ListTableData[i];
                                if (entityData == null)
                                {
                                    continue;
                                }

                                GUILayout.Space(varTableLayoutAttr.LinePadding);
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(MarginLeft);
                                bool showSubTable = false;
                                for (int j = 0; j < entityFieldInfos.Count; j++)
                                {
                                    EntityFieldInfo fieldInfo = entityFieldInfos[j];
                                    GTEntityFieldAttribute fieldAttr = fieldInfo.Attribute;
                                    if (fieldAttr == null)
                                    {
                                        continue;
                                    }
                                    PropertyInfo columnPropertyInfo = fieldInfo.PropertyInfo;
                                    if (columnPropertyInfo == null)
                                    {
                                        continue;
                                    }
                                    if (fieldAttr.ReadOnly == true)
                                    {
                                        object value = columnPropertyInfo.GetValue(entityData, null);
                                        if (value != null)
                                        {
                                            EditorGUILayout.LabelField(value.ToString(), GUILayout.Width(fieldAttr.Width));
                                        }
                                    }
                                    else if (columnPropertyInfo.PropertyType == typeof(string))
                                    {
                                        string str = (string)columnPropertyInfo.GetValue(entityData, null);
                                        if (fieldAttr.Height <= 0)
                                        {
                                            str = EditorGUILayout.TextField(str, GUILayout.Width(fieldAttr.Width));

                                        }
                                        else
                                        {
                                            str = EditorGUILayout.TextField(str, GUILayout.Width(fieldAttr.Width), GUILayout.Height(fieldAttr.Height));
                                        }
                                        columnPropertyInfo.SetValue(entityData, str, null);
                                    }
                                    else if (columnPropertyInfo.PropertyType == typeof(int))
                                    {
                                        int iData = (int)columnPropertyInfo.GetValue(entityData, null);
                                        if (fieldAttr.Height <= 0)
                                        {
                                            iData = EditorGUILayout.IntField(iData, GUILayout.Width(fieldAttr.Width));

                                        }
                                        else
                                        {
                                            iData = EditorGUILayout.IntField(iData, GUILayout.Width(fieldAttr.Width), GUILayout.Height(fieldAttr.Height));
                                        }
                                        columnPropertyInfo.SetValue(entityData, iData, null);
                                    }
                                    else if (columnPropertyInfo.PropertyType == typeof(float))
                                    {
                                        float fData = (float)columnPropertyInfo.GetValue(entityData, null);
                                        if (fieldAttr.Height <= 0)
                                        {
                                            fData = EditorGUILayout.FloatField(fData, GUILayout.Width(fieldAttr.Width));

                                        }
                                        else
                                        {
                                            fData = EditorGUILayout.FloatField(fData, GUILayout.Width(fieldAttr.Width), GUILayout.Height(fieldAttr.Height));
                                        }
                                        columnPropertyInfo.SetValue(entityData, fData, null);
                                    }
                                    else if (columnPropertyInfo.PropertyType == typeof(bool))
                                    {
                                        bool bData = (bool)columnPropertyInfo.GetValue(entityData, null);

                                        if (columnPropertyInfo.Name.Equals("Foldout"))
                                        {
                                            if (ShowFoldoutBtn)
                                            {
                                                if (FoldoutBtnHeight <= 0)
                                                {
                                                    if (GUILayout.Button(new GUIContent(FoldoutBtnText, FoldoutBtnTooltips), GUILayout.Width(FoldoutBtnWidth)))
                                                    {
                                                        if (bData == false)
                                                        {
                                                            bData = true;
                                                        }
                                                        else
                                                        {
                                                            bData = false;
                                                        }
                                                        OnTableBtnClick(varFoldoutBtnAttr, bData, entityData);
                                                    }
                                                }
                                                else
                                                {
                                                    if (GUILayout.Button(new GUIContent(FoldoutBtnText, FoldoutBtnTooltips), GUILayout.Width(FoldoutBtnWidth), GUILayout.Height(FoldoutBtnHeight)))
                                                    {
                                                        if (bData == false)
                                                        {
                                                            bData = true;
                                                        }
                                                        else
                                                        {
                                                            bData = false;
                                                        }
                                                        OnTableBtnClick(varFoldoutBtnAttr, bData, entityData);
                                                    }
                                                }
                                                showSubTable = bData;

                                                columnPropertyInfo.SetValue(entityData, bData, null);
                                            }
                                        }
                                        else if (columnPropertyInfo.Name.Equals("Detail"))
                                        {
                                            if (ShowDetailBtn)
                                            {
                                                if (DetailBtnHeight <= 0)
                                                {
                                                    if (GUILayout.Button(new GUIContent(DetailBtnText, DetailBtnTooltips), GUILayout.Width(DetailBtnWidth)))
                                                    {
                                                        if (bData == false)
                                                        {
                                                            bData = true;
                                                        }
                                                        else
                                                        {
                                                            bData = false;
                                                        }
                                                        OnTableBtnClick(varDetailBtnAttr, varDetailBtnAttr.ButtonType, entityData);
                                                    }
                                                }
                                                else
                                                {
                                                    if (GUILayout.Button(new GUIContent(DetailBtnText, DetailBtnTooltips), GUILayout.Width(DetailBtnWidth), GUILayout.Height(DetailBtnHeight)))
                                                    {
                                                        if (bData == false)
                                                        {
                                                            bData = true;
                                                        }
                                                        else
                                                        {
                                                            bData = false;
                                                        }
                                                        OnTableBtnClick(varDetailBtnAttr, varDetailBtnAttr.ButtonType, entityData);
                                                    }
                                                }
                                                showDetailView = bData;
                                                if (showDetailView)
                                                {

                                                    //       GUILayout.EndHorizontal();



                                                    i = ListTableData.Count;


                                                }
                                                columnPropertyInfo.SetValue(entityData, bData, null);
                                            }
                                        }
                                        else if (columnPropertyInfo.Name.Equals("Toggle"))
                                        {
                                            if (LastSelectAllLines != SelectAllLines)
                                            {
                                                bData = SelectAllLines;
                                            }
                                            if (fieldAttr.Height <= 0)
                                            {
                                                bData = EditorGUILayout.Toggle(bData, GUILayout.Width(fieldAttr.Width));
                                            }
                                            else
                                            {
                                                bData = EditorGUILayout.Toggle(bData, GUILayout.Width(fieldAttr.Width), GUILayout.Height(fieldAttr.Height));
                                            }
                                            columnPropertyInfo.SetValue(entityData, bData, null);
                                        }
                                        else
                                        {
                                            if (fieldAttr.Height <= 0)
                                            {
                                                bData = EditorGUILayout.Toggle(bData, GUILayout.Width(fieldAttr.Width));
                                            }
                                            else
                                            {
                                                bData = EditorGUILayout.Toggle(bData, GUILayout.Width(fieldAttr.Width), GUILayout.Height(fieldAttr.Height));
                                            }
                                            columnPropertyInfo.SetValue(entityData, bData, null);
                                        }

                                    }
                                    else if (columnPropertyInfo.PropertyType.IsEnum)
                                    {
                                        Enum eData = columnPropertyInfo.GetValue(entityData, null) as Enum;
                                        if (fieldAttr.Height <= 0)
                                        {
                                            eData = EditorGUILayout.EnumPopup(eData, GUILayout.Width(fieldAttr.Width));

                                        }
                                        else
                                        {
                                            eData = EditorGUILayout.EnumPopup(eData, GUILayout.Width(fieldAttr.Width), GUILayout.Height(fieldAttr.Height));
                                        }
                                        columnPropertyInfo.SetValue(entityData, eData, null);
                                    }

                                    if (fieldInfo.BtnAttrArray != null && fieldInfo.BtnAttrArray.Length > 0)
                                    {
                                        for (int k = 0; k < fieldInfo.BtnAttrArray.Length; k++)
                                        {
                                            GTableButtonAttribute btnAttr = fieldInfo.BtnAttrArray[k];
                                            if (btnAttr == null || btnAttr.Width == 0 || string.IsNullOrEmpty(btnAttr.Text))
                                            {
                                                continue;
                                            }
                                            if (btnAttr.Height <= 0f)
                                            {
                                                if (GUILayout.Button(new GUIContent(btnAttr.Text, btnAttr.Tips), GUILayout.Width(btnAttr.Width)))
                                                {
                                                    OnTableBtnClick(btnAttr, btnAttr.ButtonType, entityData);
                                                }
                                            }
                                            else
                                            {
                                                if (GUILayout.Button(new GUIContent(btnAttr.Text, btnAttr.Tips), GUILayout.Width(btnAttr.Width), GUILayout.Height(btnAttr.Height)))
                                                {
                                                    OnTableBtnClick(btnAttr, btnAttr.ButtonType, entityData);
                                                }
                                            }
                                        }
                                    }
                                }
                                GUILayout.EndHorizontal();
                                if (showDetailView)
                                {
                                    MemberInfo memberInfo = entityData.GetType();
                                    if (memberInfo == null) return;
                                    GTDetailViewAttribute detailViewAttr = Attribute.GetCustomAttribute(memberInfo, typeof(GTDetailViewAttribute)) as GTDetailViewAttribute;
                                    if (detailViewAttr == null || detailViewAttr.ViewType == null) return;
                                    Assembly assembly = Assembly.GetAssembly(detailViewAttr.ViewType);
                                    if (assembly == null) return;
                                    object obj = assembly.CreateInstance(detailViewAttr.ViewType.FullName);
                                    if (obj == null) return;
                                    obj.GetType().InvokeMember("OnEnable", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { entityData });
                                    obj.GetType().InvokeMember("DrawView", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { });
                                }
                                if (showSubTable)
                                {
                                    GTEditorTableView view = GTEditorTableView.DrawTableView(entityData, SingleListener);
                                    if (view != null)
                                    {
                                        NotifyListener(GTEditorTableViewListenerEvtType.OnShowSubTable, view);
                                    }
                                    GUILayout.FlexibleSpace();
                                }
                                RefreshSelectedEntity();
                            }


                        }
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
        }
        #endregion



        #region define common util function
        public List<EntityFieldInfo> SortEntityField(List<EntityFieldInfo> fields)
        {
            if (fields == null || fields.Count == 0)
            {
                return null;
            }
            IEnumerable<EntityFieldInfo> query = null;
            query = from item in fields orderby item.Attribute.FieldNumber select item;
            List<EntityFieldInfo> newArray = new List<EntityFieldInfo>();
            foreach (var item in query)
            {
                newArray.Add(item);
            }
            return newArray;
        }

        public List<GTableButtonAttribute> SortButton(List<GTableButtonAttribute> btns)
        {
            if (btns == null || btns.Count == 0)
            {
                return null;
            }
            IEnumerable<GTableButtonAttribute> query = null;
            query = from item in btns orderby item.ButtonNumber select item;
            List<GTableButtonAttribute> newArray = new List<GTableButtonAttribute>();
            foreach (var item in query)
            {
                newArray.Add(item);
            }
            return newArray;
        }
        protected void AddNewEntity()
        {
            Assembly assembly = null;
            if (TargetCreateType != null)
            {
                assembly = Assembly.GetAssembly(TargetCreateType);
            }
            else
            {
                assembly = Assembly.GetAssembly(typeof(TableEntityType));
            }
            if (assembly == null)
            {
                return;
            }
            object obj = null;
            if (TargetCreateType != null)
            {
                obj = assembly.CreateInstance(TargetCreateType.FullName);
            }
            else
            {
                obj = assembly.CreateInstance(typeof(TableEntityType).FullName);
            }
            if (obj == null)
            {
                return;
            }
            if (ListTableData == null)
            {
                ListTableData = new List<TableEntityType>();
            }
            ListTableData.GetType().InvokeMember("Add", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, ListTableData, new object[] { obj });
        }
        protected void RefreshSelectedEntity()
        {
            if (ListTableData == null || ListTableData.Count == 0)
            {
                return;
            }
            SelectedTableData.Clear();
            for (int i = 0; i < ListTableData.Count; i++)
            {
                TableEntityType entity = ListTableData[i];
                if (entity == null)
                {
                    continue;
                }
                PropertyInfo toggleProp = entity.GetType().GetProperty("Toggle");
                if (toggleProp == null)
                {
                    continue;
                }
                bool b = false;
                if (toggleProp.GetValue(entity, null) != null)
                {
                    bool.TryParse(toggleProp.GetValue(entity, null).ToString(), out b);
                }
                if (b)
                {
                    SelectedTableData.Add(entity);
                }
            }
        }
        protected void DeleteSelectedEntity()
        {
            if (SelectedTableData == null || SelectedTableData.Count == 0 || ListTableData == null || ListTableData.Count == 0)
            {
                return;
            }
            for (int i = 0; i < SelectedTableData.Count; i++)
            {
                TableEntityType entity = SelectedTableData[i];
                if (entity == null)
                {
                    continue;
                }
                ListTableData.Remove(entity);
            }
            SelectedTableData.Clear();
        }
        protected void RemoveAllEntity()
        {
            if (ListTableData == null || ListTableData.Count == 0)
            {
                return;
            }
            if (EditorUtility.DisplayDialog("Warnning", "Clear all table data,sure do?", "Confirm"))
            {
                ListTableData.Clear();
            }
        }
        protected void CopyEntity()
        {
            if (SelectedTableData == null || SelectedTableData.Count == 0 || ListTableData == null || ListTableData.Count == 0)
            {
                return;
            }
            for (int i = 0; i < SelectedTableData.Count; i++)
            {
                TableEntityType entity = SelectedTableData[i];
                if (entity == null)
                {
                    continue;
                }
                Assembly assembly = Assembly.GetAssembly(entity.GetType());
                if (assembly == null)
                {
                    return;
                }
                object obj = assembly.CreateInstance(entity.GetType().FullName);
                if (obj == null)
                {
                    return;
                }
                obj.GetType().InvokeMember("Clone", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { entity });
                ListTableData.GetType().InvokeMember("Add", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, ListTableData, new object[] { obj });
            }
        }
        protected void PasteEntity()
        {

        }
        protected void OnTableBtnClick(GTableButtonAttribute btnAttr, params object[] args)
        {
            if (btnAttr == null)
            {
                return;
            }
            switch (btnAttr.ButtonType)
            {
                case GTableBtnType.Toolbar_Add:
                    AddNewEntity();
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Toolbar_Delete:
                    DeleteSelectedEntity();
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Toolbar_Clear:
                    RemoveAllEntity();
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Toolbar_Copy:
                    CopyEntity();
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Toolbar_Paste:
                    PasteEntity();
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Toolbar_Custom1:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Toolbar_Custom2:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Toolbar_Custom3:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Toolbar_CustomN:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;


                case GTableBtnType.Title_Help:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Title_Doc:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Title_Custom1:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Title_Custom2:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Title_Custom3:

                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;
                case GTableBtnType.Title_CustomN:

                    NotifyListener(GTEditorTableViewListenerEvtType.OnToolbarButtonClick, args);
                    break;


                case GTableBtnType.Entity_Help:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnEntityButtonClick, args);
                    break;
                case GTableBtnType.Entity_Doc:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnEntityButtonClick, args);
                    break;
                case GTableBtnType.Entity_Detail:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnEntityButtonClick, args);
                    break;
                case GTableBtnType.Entity_Foldout:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnFoldoutButtonClick, args);
                    break;
                case GTableBtnType.Entity_Review:
				NotifyListener(GTEditorTableViewListenerEvtType.OnEntityButtonClick, args);
                    break;
                case GTableBtnType.Entity_Custom1:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnEntityButtonClick, args);
                    break;
                case GTableBtnType.Entity_Custom2:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnEntityButtonClick, args);
                    break;
                case GTableBtnType.Entity_Custom3:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnEntityButtonClick, args);
                    break;
                case GTableBtnType.Entity_CustomN:
                    NotifyListener(GTEditorTableViewListenerEvtType.OnEntityButtonClick, args);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
#endif