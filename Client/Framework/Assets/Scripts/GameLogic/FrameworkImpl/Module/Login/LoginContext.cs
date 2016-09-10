using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Air2000;
using Air2000.Modular;

namespace GameLogic
{
    [ContextProperty(typeof(LoginController), false, true)]
    [ContextProperty(typeof(LoginViewController), false, true)]
    public class LoginContext : ViewContext
    {
        [InternalInject]
        public static LoginContext Instance { get; set; }
        [InternalInject]
        public static LoginController Controller { get; set; }
        [InternalInject]
        public static LoginViewController ViewController { get; set; }


        [ExternalInject(typeof(ResContext))]
        public ResController ResController { get; set; }

        [ExternalInject(typeof(VideoContext))]
        public VideoContoller VideoContoller { get; set; }

        [ExternalInject(typeof(AudioContext))]
        public Air2000.AudioController1 AudioController { get; set; }

        [ExternalInject(typeof(NetworkContext))]
        public NetworkController NetworkController { get; set; }

        [ExternalInject(typeof(WindowContext))]
        public WindowController WindowController { get; set; }
    }
}
