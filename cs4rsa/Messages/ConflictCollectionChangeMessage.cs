using cs4rsa.BaseClasses;
using cs4rsa.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Messages
{
    class ConflictCollectionChangeMessage : Cs4rsaMessage
    {
        public new List<IConflictModel> Source;
        public ConflictCollectionChangeMessage(List<IConflictModel> source) : base(source)
        {
            Source = source;
        }
    }
}
