using System;

namespace cs4rsa_core.Exceptions
{
    internal class SchoolClassIsNotBelongSpecialSubjectException : Exception
    {
        public SchoolClassIsNotBelongSpecialSubjectException(string message) : base(message)
        {
        }
    }
}
