using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Modular;

namespace Air2000
{
    [ContextProperty(typeof(WindowService))]
    public class WindowContext : Context
    {
        [InternalInject]
        public static WindowContext Instance { get; set; }
        public void OpenView(Context context)
        {

        }
    }
}
