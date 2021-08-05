using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;

namespace cs4rsa.Messages
{
    /// <summary>
    /// Yêu cầu update cơ sở dữ liệu môn học tới MainSchedulingViewModel.
    /// </summary>
    public class UpdateSubjectDatabase : Cs4rsaMessage
    {
        public UpdateSubjectDatabase(object source) : base(source)
        {
        }
    }
}
