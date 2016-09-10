using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Air2000;
using Air2000.Module;

namespace GameLogic
{
    [ContextLegacyProperty(typeof(LoginController), typeof(ContextController))]
    [ContextLegacyProperty(typeof(LoginViewController), typeof(ViewController))]
    public class LoginContext : ViewContext
    {
        [InternalInject]
        public static LoginContext Instance { get; set; }
        [InternalInject]
        public static LoginController Controller { get; set; }
        [InternalInject]
        public static LoginViewController ViewController { get; set; }
    }
}
