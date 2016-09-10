using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Modular;

namespace Air2000
{
    [ContextProperty(typeof(WindowController))]
    public class WindowContext : Context
    {
        [InternalInject]
        public static WindowController Instance { get; set; }
    }
}
