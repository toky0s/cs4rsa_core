using cs4rsa_core.BaseClasses;
using System.Collections.Generic;

namespace cs4rsa_core.Messages
{
    public class AddCombinationMessage : Cs4rsaMessage
    {
        public new List<int> Source;
        public AddCombinationMessage(List<int> source) : base(source)
        {
            Source = source;
        }
    }
}
