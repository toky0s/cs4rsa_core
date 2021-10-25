using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;

namespace cs4rsa_core.Messages
{
    public class ExitLoginMessage : Cs4rsaMessage
    {
        public new LoginResult Source;
        public ExitLoginMessage(LoginResult source) : base(source)
        {
            Source = source;
        }
    }
}
