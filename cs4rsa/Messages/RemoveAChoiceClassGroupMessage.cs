using cs4rsa.BaseClasses;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Messages
{
    public class RemoveAChoiceClassGroupMessage : Cs4rsaMessage
    {
        public new ClassGroupModel Source;
        public RemoveAChoiceClassGroupMessage(ClassGroupModel source) : base(source)
        {
            Source = source;
        }
    }
}
