using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Modular;
using GameLogic;

namespace Air2000
{
    [ContextProperty(typeof(ResController))]
    public class ResContext : Context
    {
        [ExternalInject(typeof(LoginContext))]
        public LoginContext LoginContext { get; set; }

        [ExternalInject(typeof(LoginContext))]
        public LoginController LoginController { get; set; }

        [ExternalInject(typeof(LoginContext))]
        public LoginViewController LoginViewController { get; set; }
    }
}
