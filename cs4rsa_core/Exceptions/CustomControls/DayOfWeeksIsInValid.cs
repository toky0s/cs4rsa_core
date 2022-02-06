using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Exceptions.CustomControls
{
    public class DayOfWeeksIsInValid : Exception
    {
        public DayOfWeeksIsInValid(string message) : base(message)
        {
        }
    }
}
