using cs4rsa.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Messages
{
    /// <summary>
    /// publisher: SubjectImporter
    /// subcriber: SearchViewModel
    /// work: Xoá tất cả SubjectModel trong SearchViewModel.
    /// </summary>
    public class CleanSubjectModelsRequest : Cs4rsaMessage
    {
        public CleanSubjectModelsRequest(object source) : base(source)
        {
        }
    }
}
