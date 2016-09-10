using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Air2000;
using Air2000.Module;

namespace GameLogic
{
    public class Main : MonoBehaviour
    {
        void Start()
        {
            var appContext = AppContext.GetInstance();
            appContext.RegisterContext<LoginContext>();

            //appContext.UnregisterContext<>


        }
    }
}
