using cs4rsa_core.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Messages
{
    class SettingChangeMessage : Cs4rsaMessage
    {
        public SettingChangeMessage(object source) : base(source)
        {
        }
    }
}
