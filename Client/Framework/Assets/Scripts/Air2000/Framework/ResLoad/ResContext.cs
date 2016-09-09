using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Air2000.Module;

namespace Air2000
{
    public class ResContext<T> : Context<T>
        where T : ResController
    {
    }
}
