using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Messages.Publishers.Dialogs
{
    internal sealed class SessionInputVmMsgs
    {
        internal sealed class ExitFindStudentMsg : ValueChangedMessage<Student>
        {
            public ExitFindStudentMsg(Student value) : base(value)
            {

            }
        }
    }
}
