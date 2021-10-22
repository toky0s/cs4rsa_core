using LightMessageBus.Interfaces;

namespace cs4rsa_core.BaseClasses
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
