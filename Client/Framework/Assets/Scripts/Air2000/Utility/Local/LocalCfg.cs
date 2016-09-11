/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: LocalCfg.cs
			// Describle: 配置文件相关
			// Created By:  hsu
			// Date&Time:  2016/3/5 15:29:38
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.IO;
using UnityEngine;
using Air2000.Res;

namespace Air2000
{
    public class LocalCfg
    {
        public delegate void LocalCfgCallback(object varParam);
        public class Handle
        {
            public LocalCfgCallback Callback { get; set; }
            public Type TargetType { get; set; }
            public string FilePath { get; set; }
            public GameObjPool.ObjFlag CfgObjFlag { get; set; }
            public Handle(LocalCfgCallback varFunc, Type varTargetType, string varFilePath, GameObjPool.ObjFlag varCfgObjFlag = GameObjPool.ObjFlag.UseOriObj)
            {
                Callback = varFunc;
                TargetType = varTargetType;
                FilePath = varFilePath;
                CfgObjFlag = varCfgObjFlag;
            }
            public void Excute(object varTargetObj)
            {
                if (Callback != null)
                {
                    Callback(varTargetObj);
                }
            }
        }
        private static LocalCfg mInstance;

        private LocalCfg()
        {
        }

        public static LocalCfg GetSingleton()
        {
            if (mInstance == null)
            {
                mInstance = new LocalCfg();
            }
            return mInstance;
        }

        public bool SerializeObjToFile(object varObj, string varFilePath)
        {
            if (varObj == null)
            {
                Helper.LogError("LocalCfg SerializeObjToFile: Error caused by null obj");
                return false;
            }
            if (string.IsNullOrEmpty(varFilePath))
            {
                Helper.LogError("LocalCfg SerializeObjToFile: Error caused by null varFilePath");
                return false;
            }
            try
            {
                using (var file = File.Create(varFilePath))
                {
                    Serializer.Serialize(file, varObj);
                }
            }
            catch (Exception e)
            {
                Helper.LogError("LocalCfg SerializeObjToFile Exception: filePath: " + varFilePath);
                Helper.LogError(e.Message);
                return false;
            }
            return true;
        }

        public void DeserializeObjFromFile(Handle varHandle)
        {
            if (Application.isPlaying == false)
            {
                return;
            }
            if (varHandle == null)
            {
                return;
            }
            GameObjPool.GetSingleton().LoadObj(new GameObjPool.LoadTask(varHandle.FilePath, typeof(TextAsset), OnLoadCfgFileCallback, varHandle, varHandle.CfgObjFlag));
        }

        public T DeserializeObjFromFile<T>(string varFileName)
        {
            T t = default(T);
            if (string.IsNullOrEmpty(varFileName))
            {
                return t;
            }
            string filePath = ResContext.GetSavePath() + "/" + varFileName;
            byte[] bytes = null;
            try
            {
                bytes = File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                Helper.Log(e.Message);
            }
            if (bytes == null || bytes.Length == 0)
            {
                Helper.Log("LocalCfg DeserializeObjFromFile:Error caused by null file bytes,path: " + filePath);
                return t;
            }
            return DeserializeObjFromBytes<T>(bytes);
        }

        public T DeserializeObjFromBytes<T>(byte[] varBytes)
        {
            T t = default(T);
            if (varBytes == null)
            {
                Helper.LogError("LocalCfg DeserializeObjFromTextAsset: Error caused by null byte[] instance");
                return t;
            }
            Stream stream = new MemoryStream(varBytes, false);
            if (stream == null)
            {
                Helper.LogError("LocalCfg DeserializeObjFromTextAsset: Error caused by null MemoryStream instance");
                return t;
            }
            try
            {
                t = Serializer.Deserialize<T>(stream);
            }
            catch (Exception e)
            {
                Helper.LogError("LocalCfg DeserializeObjFromTextAsset: Error Msg   " + e.Message);
            }
            return t;
        }

        public T DeserializeObjFromTextAsset<T>(TextAsset varTextAsset)
        {
            T t = default(T);
            if (varTextAsset == null)
            {
                Helper.LogError("LocalCfg DeserializeObjFromTextAsset: Error caused by null TextAsset instance");
                return t;
            }
            Stream stream = new MemoryStream(varTextAsset.bytes, false);
            if (stream == null)
            {
                Helper.LogError("LocalCfg DeserializeObjFromTextAsset: Error caused by null MemoryStream instance");
                return t;
            }
            try
            {
                t = Serializer.Deserialize<T>(stream);
            }
            catch (Exception e)
            {
                Helper.LogError("LocalCfg DeserializeObjFromTextAsset: Error Msg   " + e.Message);
            }
            return t;
        }

        private void OnLoadCfgFileCallback(CacheObjInfo varObjInfo, object varParam)
        {
            Handle handle = varParam as Handle;
            if (handle == null)
            {
                Helper.LogError("LocalCfg OnLoadCfgFileCallback: Error caused by null Handle instance");
                return;
            }
            if (varObjInfo == null)
            {
                Helper.LogError("LocalCfg OnLoadCfgFileCallback: Error caused by null CacheObjInfo instance");
                return;
            }
            if (varObjInfo.GetObj() == null)
            {
                Helper.LogError("LocalCfg OnLoadCfgFileCallback: Error caused by null CacheObjInfo.GetObj() instance");
                return;
            }
            TextAsset textAsset = varObjInfo.GetObj() as TextAsset;
            if (textAsset == null)
            {
                Helper.LogError("LocalCfg OnLoadCfgFileCallback: Error caused by null TextAsset instance");
                return;
            }
            Stream stream = new MemoryStream(textAsset.bytes, false);
            if (stream == null)
            {
                Helper.LogError("LocalCfg OnLoadCfgFileCallback: Error caused by null MemoryStream instance");
                return;
            }
            object targetObj = Serializer.Deserialize(handle.TargetType, stream);
            varObjInfo.Recycle();
            handle.Callback(targetObj);
        }
    }
}
