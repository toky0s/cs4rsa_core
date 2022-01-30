using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Exceptions
{
    internal class SchoolClassIsNotBelongSpecialSubjectException : Exception
    {
        public SchoolClassIsNotBelongSpecialSubjectException(string message) : base(message)
        {
        }
    }
}
