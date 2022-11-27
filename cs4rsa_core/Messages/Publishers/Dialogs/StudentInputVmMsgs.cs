using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Dialogs.DialogResults;

namespace Cs4rsa.Messages.Publishers.Dialogs
{
    internal sealed class StudentInputVmMsgs
    {
        internal sealed class ExitLoginMsg : ValueChangedMessage<LoginResult>
        {
            /// <summary>
            /// Thoát dialog lựa chọn sinh viên
            /// </summary>
            public ExitLoginMsg(LoginResult value) : base(value)
            {
            }
        }
    }
}
