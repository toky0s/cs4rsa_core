using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightMessageBus.Interfaces;

namespace cs4rsa.BaseClasses
{
    public class Cs4rsaMessage : IMessage
    {
        public object Source { get; set; }

        public Cs4rsaMessage(object source)
        {
            Source = source;
        }
    }
}
