using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Exceptions
{
    public class RegisterCodeIsInvalidException : Exception
    {
        public RegisterCodeIsInvalidException()
        {
        }

        public RegisterCodeIsInvalidException(string message) : base(message)
        {
        }

        public RegisterCodeIsInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RegisterCodeIsInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
