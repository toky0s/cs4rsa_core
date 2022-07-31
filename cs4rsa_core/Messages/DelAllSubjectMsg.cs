using LightMessageBus.Interfaces;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Delete all downloaded subjects message.
    /// </summary>
    public class DelAllSubjectMsg : IMessage
    {
        public object Source { get; set; }
    }
}
