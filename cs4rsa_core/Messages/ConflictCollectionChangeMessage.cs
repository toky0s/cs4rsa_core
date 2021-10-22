using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using cs4rsa_core.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Thông báo có sự thay đổi về danh sách các xung đột thời gian của một
    /// </summary>
    class ConflictCollectionChangeMessage : Cs4rsaMessage
    {
        public new List<ConflictModel> Source;
        public ConflictCollectionChangeMessage(List<ConflictModel> source) : base(source)
        {
            Source = source;
        }
    }
}
