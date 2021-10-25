using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;

namespace cs4rsa_core.Messages
{
    public class ExitImportSubjectMessage : Cs4rsaMessage
    {
        public new SessionManagerResult Source { get; set; }
        public ExitImportSubjectMessage(SessionManagerResult source) : base(source)
        {
            Source = source;
        }
    }
}
