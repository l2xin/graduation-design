using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Air2000;
using Air2000.Module;

namespace GameLogic
{
    [ContextProperty(typeof(LoginController))]
    [ContextProperty(typeof(LoginViewController))]
    public class LoginContext : ViewContext
    {

    }
}
