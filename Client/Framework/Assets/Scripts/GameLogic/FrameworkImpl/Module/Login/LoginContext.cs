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
    [ContextProperty(typeof(ViewController))]
    public class LoginContext<T1, T2> : ViewContext<T1, T2>
         where T1 : ContextController
        where T2 : ViewController
    {

    }
}
