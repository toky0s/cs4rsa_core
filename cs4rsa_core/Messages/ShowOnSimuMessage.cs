using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Message này được public mỗi khi người dùng yêu cầu hiển thị
    /// một combi lên mô phỏng.
    /// </summary>
    public class ShowOnSimuMessage : Cs4rsaMessage
    {
        public new CombinationModel Source;
        public ShowOnSimuMessage(CombinationModel source) : base(source)
        {
            Source = source;
        }
    }
}
