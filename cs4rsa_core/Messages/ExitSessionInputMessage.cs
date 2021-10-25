using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;

namespace cs4rsa_core.Messages
{
    public class ExitSessionInputMessage : Cs4rsaMessage
    {
        public new StudentResult Source { get; set; }
        public ExitSessionInputMessage(StudentResult source) : base(source)
        {
            Source = source;
        }
    }
}
