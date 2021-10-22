using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Message này được publish khi người dùng thêm một ClassGroup vào danh sách chọn
    /// </summary>
    class ClassGroupAddedMessage : Cs4rsaMessage
    {
        public new ClassGroupModel Source;
        public ClassGroupAddedMessage(ClassGroupModel source) : base(source)
        {
            Source = source;
        }
    }
}
