using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Dialogs.DialogResults;

namespace Cs4rsa.Messages.Publishers.Dialogs
{
    internal class SessionInputVmMsgs
    {
        /// <summary>
        /// Thoát tìm kiếm account
        /// </summary>
        internal sealed class ExitSearchAccountMsg : ValueChangedMessage<StudentResult>
        {
            public ExitSearchAccountMsg(StudentResult value) : base(value)
            {
            }
        }
    }
}
