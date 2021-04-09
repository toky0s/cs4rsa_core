using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;

namespace cs4rsa.Messages
{
    /// <summary>
    /// Message này được publish khi người dùng thêm một ClassGroup vào danh sách chọn
    /// </summary>
    class ClassGroupAddedMessage : Cs4rsaMessage
    {
        public ClassGroupAddedMessage(object source) : base(source)
        {

        }
    }
}
