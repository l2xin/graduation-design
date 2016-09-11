using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Modular;

namespace Air2000.Res
{
    public class ResController : ContextController
    {
        [InternalInject]
        public static ResContext Context { get; set; }
    }
}
