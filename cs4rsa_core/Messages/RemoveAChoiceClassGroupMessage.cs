using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Messages
{
    public class RemoveAChoiceClassGroupMessage : Cs4rsaMessage
    {
        public new string Source;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Tên của class group cần loại bỏ.</param>
        public RemoveAChoiceClassGroupMessage(string source) : base(source)
        {
            Source = source;
        }
    }
}
