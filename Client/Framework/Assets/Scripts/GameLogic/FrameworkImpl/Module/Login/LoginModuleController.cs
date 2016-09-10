using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Module;
using Air2000;

namespace GameLogic
{
    public class LoginController : ContextController
    {
        [InternalInject]
        public static LoginContext Context { get; set; }
        [InternalInject(typeof(ContextController))]
        public static LoginController Instance { get; set; }
    }
}
