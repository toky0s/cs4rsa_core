using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;
using cs4rsa.ViewModels;

namespace cs4rsa.Messages
{
    public class SubjectItemChangeMessage : Cs4rsaMessage
    {
        public new SearchViewModel Source;
        public SubjectItemChangeMessage(SearchViewModel source) : base(source)
        {
            Source = source;
        }
    }
}
