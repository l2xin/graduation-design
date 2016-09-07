﻿/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ApplicationEventProcessor.cs
			// Describle: Application event processor.
			// Created By:  hsu
			// Date&Time:  2016/1/19 10:03:15
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Air2000
{
    public class ApplicationEventProcessor : EventProcessor
    {
        public static ApplicationEventProcessor m_Instance;
        /// <summary>
        /// Get a single instance of type 'ApplicationEventProcessor'
        /// </summary>
        /// <returns></returns>
        public static ApplicationEventProcessor GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new ApplicationEventProcessor();
            }
            return m_Instance;
        }
        private ApplicationEventProcessor() { }
    }

    public enum ApplicationEventType
    {
        GE_HeroChange,//角色发生切换
        GE_SceneBegin, //场景开始
        GE_LevelWasLoaded, //关卡加载完成
        GE_SceneWasLoaded, //场景加载完成
        GE_SceneEnd, //场景结束

        /// <summary>
        /// 主界面蒙板打开/关闭
        /// </summary>
        GE_SetMainMask,

        GE_ClientCommond,

        GE_NetWorkState,

        GE_EditorCreateCharacter,
    }

}
