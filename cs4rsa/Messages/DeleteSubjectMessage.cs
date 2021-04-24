using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;
using cs4rsa.Models;

namespace cs4rsa.Messages
{
    class DeleteSubjectMessage : Cs4rsaMessage
    {
        public readonly new SubjectModel Source;
        public DeleteSubjectMessage(SubjectModel source) : base(source)
        {
            Source = source;
        }
    }
}
