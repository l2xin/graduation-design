using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000;

namespace Air2000.Modular
{
    public class ViewContextController : ContextController
    {
        [ExternalInject(typeof(WindowContext))]
        public WindowContext WindowContext { get; set; }
    }
}
