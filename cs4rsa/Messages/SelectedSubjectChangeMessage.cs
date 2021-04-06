using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Models;
using cs4rsa.BaseClasses;

namespace cs4rsa.Messages
{
    public class SelectedSubjectChangeMessage : Message
    {
        public SelectedSubjectChangeMessage(object source) : base(source)
        {

        }
    }
}
