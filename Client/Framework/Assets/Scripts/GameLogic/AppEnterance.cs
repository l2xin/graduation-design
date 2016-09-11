using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Air2000;
using Air2000.Modular;
using Air2000.Res;

using GameLogic;

namespace Air2000
{
    public class AppEnterance : MonoBehaviour
    {
        private static AppEnterance m_Instance;
        private AppEnterance() { m_Instance = this; }
        public static AppEnterance GetInstance() { return m_Instance; }
        protected virtual void Start()
        {
            // Application services
            AppContext.RegisterContext<WindowContext>();
            AppContext.RegisterContext<ResContext>();
            AppContext.RegisterContext<VideoContext>();
            AppContext.RegisterContext<AudioContext>();
            AppContext.RegisterContext<NetworkContext>();


            // Game custom services
            AppContext.RegisterContext<LoginContext>();

            LoginContext.Instance.RegisterContextEventHandler(3, OnEventCallback);

            LoginContext.Instance.NotifyEvent(new EventEX<AppEnterance>(3, this));

            int a = 1;
            //appContext.UnregisterContext<>


        }

        void OnEventCallback(Air2000.Event evt)
        {
            int c = 1;
        }
    }
}
