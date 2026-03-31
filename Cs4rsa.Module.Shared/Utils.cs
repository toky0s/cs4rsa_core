using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.Shared
{
    public class Utils
    {
        public static bool IsProduction()
        {
            #if DEBUG
            return false;
            #else
            return true;
            #endif
        }
    }
}
