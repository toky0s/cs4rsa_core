using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;
using cs4rsa.Models;

namespace cs4rsa.Messages
{
    public class ChoicesChangedMessage : Cs4rsaMessage
    {
        public new List<ClassGroupModel> Source;
        public ChoicesChangedMessage(List<ClassGroupModel> source) : base(source)
        {
            Source = source;
        }
    }
}
