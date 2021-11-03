using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
