using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.ViewModels;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Message này sẽ được xử lý bởi MainWindow nhằm xác định số lượng môn
    /// và số tín chỉ hiện tại trong phần Search.
    /// </summary>
    public class SubjectItemChangeMessage : Cs4rsaMessage
    {
        public new SearchSessionViewModel Source;
        public SubjectItemChangeMessage(SearchSessionViewModel source) : base(source)
        {
            Source = source;
        }
    }
}
