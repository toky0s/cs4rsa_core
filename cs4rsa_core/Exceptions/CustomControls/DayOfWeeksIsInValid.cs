using System;

namespace cs4rsa_core.Exceptions.CustomControls
{
    public class DayOfWeeksIsInValid : Exception
    {
        public DayOfWeeksIsInValid(string message) : base(message)
        {
        }
    }
}
