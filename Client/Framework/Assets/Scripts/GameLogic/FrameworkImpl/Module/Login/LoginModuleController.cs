using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Modular;
using Air2000;

namespace GameLogic
{
    public class LoginController : ContextController
    {
        [InternalInject]
        public static LoginContext Context { get; set; }
        [InternalInject]
        public static LoginController Instance { get; set; }
    }
}
