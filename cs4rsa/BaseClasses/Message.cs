using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightMessageBus.Interfaces;

namespace cs4rsa.BaseClasses
{
    public class Message : IMessage
    {
        public object Source { get; set; }

        public Message(object source)
        {
            Source = source;
        }
    }
}
