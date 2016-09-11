/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: GameSqlite.cs
            // Describle: 提供底层的数据库查询操作
            // Created By:  陈伟超
            // Date&Time:  2015/3/25 12:15:18
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.IO;
using System.Collections;
using Community.CsharpSqlite;
using UnityEngine;
using Air2000.Res;

namespace Air2000
{
    public class LocalConfig : IDisposable
    {
        private static LocalConfig mInstance;//类实例
        private bool mIsDisposed = false;
        private SQLiteDB mDbConnect = null;//数据库连接
        private const string DATABASE_NAME = "game";//数据库名称
        private const string DATABASE_FILE_PATH = "database/";//数据库文件路径

        public static LocalConfig GetSingleton()
        {
            if (mInstance == null)
            {
                mInstance = new LocalConfig();
            }

            return mInstance;
        }

        private LocalConfig()
        {

        }

        ~LocalConfig()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Close()
        {
            Dispose(true);
        }

        protected void Dispose(bool Diposing)
        {
            if (!mIsDisposed)
            {
                if (Diposing)
                {
                    //Clean Up managed resources  
                    if (mDbConnect != null)
                    {
                        mDbConnect.Close();
                        mDbConnect = null;
                    }
                }
                //Clean up unmanaged resources  
            }
            mIsDisposed = true;
        }

        public void SetupDatabasePath(LoadAssetCallback func)
        {
            if (IsOpen())
            {
                if (func != null)
                {
                    func(null, null);
                }
                return;
            }

            string root = "sdcard/db/QiuMo.bytes";
            //string root = "C:/QiuMo.bytes";
            if (File.Exists(root))
            {
                Helper.Log("Get Database Success");
                byte[] tembyte = File.ReadAllBytes(root);
                if (tembyte != null)
                {
                    Open(tembyte);
                }
                if (func != null)
                {
                    func(null, null);
                }
                return;
            }
            else
            {
                Helper.Log("File not exist!");
            }

            string filePath = "{0}{1}";
            Helper.Format(filePath, DATABASE_FILE_PATH, DATABASE_NAME);

            LoadAssetCallback tempCallback = delegate(UnityEngine.Object varObj, ResourceLoadParam varParam)
            {
                OnLoadDBFileFinsh(varObj, varParam);
                if (func != null)
                {
                    func(varObj, varParam);
                }
            };

            ResContext.LoadAssetAsync(filePath, DATABASE_NAME, typeof(TextAsset), tempCallback);
        }

        void OnLoadDBFileFinsh(object o, ResourceLoadParam param)
        {
            TextAsset temAsset = o as TextAsset;
            if (temAsset == null || temAsset.bytes == null)
            {
                Helper.LogError(GetType().ToString() + ": Load database fail");
                return;
            }
            Open(temAsset.bytes);
        }

        /// <summary>
        /// 判断本地数据库是否打开
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            if (mDbConnect != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 打开本地数据库
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool Open(byte[] bytes)
        {
            if (mDbConnect != null)
            {
                return true;
            }
            if (bytes == null)
            {
                Helper.LogError(GetType().ToString() + ": Open database fail caused by null bytes");
                return false;
            }
            byte[] tmpBytes = bytes;
            if (tmpBytes == null)
            {
                return false;
            }
            MemoryStream ms = new MemoryStream(tmpBytes);
            {
                try
                {
                    mDbConnect = new SQLiteDB();
                    mDbConnect.OpenStream("db", ms);
                    Helper.Log(GetType().ToString() + ": Open database success");
                }
                catch (Exception e)
                {
                    ms.Dispose();
                    Helper.Log(GetType().ToString() + ": Open database fail,error " + e.ToString());
                }
            }
            return true;
        }

        /// <summary>
        /// 判断DB文件是否存在
        /// </summary>
        /// <returns></returns>
        private bool IsDbExist()
        {
            string filePath = "{0}database/{1}";
            filePath = Helper.Format(filePath, ResContext.GetSavePath(), DATABASE_NAME);

            if (File.Exists(filePath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 读取指定表格中与id对应的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool GetLocalItem(string tableName, uint id, out LocalItem item)
        {
            string tmpSqlStr = "select * from {0} where id = {1} ;";
            tmpSqlStr = Helper.Format(tmpSqlStr, tableName, id);
            return GetLocalItem(tmpSqlStr, out item);
        }

        /// <summary>
        /// 读取本地表格
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="outItems"></param>
        /// <returns></returns>
        public bool GetLocalItem(string sqlStr, out LocalItem outItems)
        {
            outItems = null;
            if (sqlStr == string.Empty)
            {
                return false;
            }
            SQLiteQuery tmpReader = new SQLiteQuery(mDbConnect, sqlStr);
            if (tmpReader != null)
            {
                outItems = new LocalItem(tmpReader);
                return true;
            }
            else
            {
                tmpReader.Release();
                return false;
            }
        }
    }

    /// <summary>
    /// 表格映射结构体
    /// </summary>
    public class LocalItem : IDisposable
    {
        public int mId;//主键
        private SQLiteQuery mReader;
        private bool mIsDisposed = false;

        public LocalItem(SQLiteQuery reader)
        {
            mReader = reader;
        }


        ~LocalItem()
        {
            Dispose(false);
        }

        public void Close()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    //Clean Up managed resources  
                    if (mReader != null)
                    {
                        mReader.Release();
                        mReader = null;
                    }
                }
                //Clean up unmanaged resources  
            }
            mIsDisposed = true;
        }



        public int pID
        {
            get { return mId; }
            set { mId = value; }
        }


        public int pColumnCount
        {
            get
            {
                if (mReader == null)
                {
                    return 0;
                }
                if (mReader.Names == null)
                {
                    return 0;
                }
                return mReader.Names.Length;
            }
        }

        /// <summary>
        /// 获取字符类型数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetString(int index, string defaultValue)
        {
            if (mReader == null || index >= pColumnCount || index < 0)
            {
                return string.Empty;
            }
            if (mReader.GetFieldType(index) != Sqlite3.SQLITE_TEXT)
            {
                return defaultValue;
            }
            try
            {
                return mReader.GetString(index);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取整型类型数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetInterger(int index, int defaultValue)
        {
            if (mReader == null || index >= pColumnCount || index < 0)
            {
                return defaultValue;
            }
            if (mReader.GetFieldType(index) != Sqlite3.SQLITE_INTEGER)
            {
                return defaultValue;
            }
            try
            {
                return mReader.GetInteger(index);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取浮点型数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public float GetFloat(int index, float defaultValue)
        {
            try
            {
                return mReader.GetFloat(index);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// 通过索引查找列名
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetColumnNameByIndex(int index)
        {
            if (mReader == null || index < 0 || index >= pColumnCount || mReader.Names == null)
            {
                return string.Empty;
            }
            return mReader.Names[index];
        }


        /// <summary>
        /// 通过列名查找索引
        /// </summary>
        /// <param name="columuName"></param>
        /// <returns></returns>
        public int GetColumnIndexByName(string columuName)
        {
            if (mReader == null || string.IsNullOrEmpty(columuName))
            {
                return -1;
            }
            try
            {
                return mReader.GetFieldIndex(columuName);
            }
            catch (Exception e)
            {
                Helper.LogError(e.Message);
            }
            return -1;
        }

        /// <summary>
        /// 通过列名查找对应的值，返回字符串.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetValueByColumnName(string columnName, string defaultValue)
        {
            if (mReader == null || string.IsNullOrEmpty(columnName))
            {
                return null;
            }
            try
            {
                int tempColNumber = GetColumnIndexByName(columnName);
                if (tempColNumber < 0 || tempColNumber >= pColumnCount)
                {
                    return defaultValue;
                }

                return mReader.GetString(tempColNumber);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return defaultValue;
            }
        }


        /// <summary>
        /// 通过列名查找对应的值，返回浮点型.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public float GetValueByColumnName(string columnName, float defaultValue)
        {
            try
            {
                int tmpIndex = GetColumnIndexByName(columnName);
                if (tmpIndex < 0)
                {
                    return defaultValue;
                }
                return GetFloat(tmpIndex, defaultValue);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return defaultValue;
            }
        }


        /// <summary>
        /// 通过列名查找对应的值，返回整型.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetValueByColumnName(string columnName, int defaultValue)
        {
            try
            {
                int tempFileldIndex = mReader.GetFieldIndex(columnName);
                if (tempFileldIndex < 0)
                {
                    return -1;
                }
                return mReader.GetInteger(tempFileldIndex);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// go to next row .if this row is end return false 
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            if (mReader == null)
            {
                return false;
            }
            return mReader.Step();
        }
    }
}