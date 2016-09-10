using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Air2000;
using Air2000.Modular;

namespace GameLogic
{
    public class Main : MonoBehaviour
    {
        void Start()
        {
            var appContext = AppContext.GetInstance();
            WindowContext winContext = appContext.RegisterContext<WindowContext>();
            ResContext resContext = appContext.RegisterContext<ResContext>();
            appContext.RegisterContext<VideoContext>();
            appContext.RegisterContext<AudioContext>();
            appContext.RegisterContext<NetworkContext>();

            LoginContext loginContext = appContext.RegisterContext<LoginContext>();
            loginContext = LoginContext.Instance;
            LoginController loginController = LoginController.Instance;
            
            int a = 1;
            //appContext.UnregisterContext<>


        }
    }
}
