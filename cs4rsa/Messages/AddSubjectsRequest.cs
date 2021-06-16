using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;
using cs4rsa.Models;

namespace cs4rsa.Messages
{
    /// <summary>
    /// publisher: SubjectImporter
    /// subcriber: SearchViewModel
    /// work: Add Subject.
    /// </summary>
    public class AddSubjectsRequest : Cs4rsaMessage
    {
        public new List<SubjectModel> Source;
        public AddSubjectsRequest(List<SubjectModel> source) : base(source)
        {
            Source = source;
        }
    }
}
