using LightMessageBus.Interfaces;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Update subject database message.
    /// </summary>
    public class UpdateSubjectDatabase : IMessage
    {
        public object Source { get; set; }
    }
}
