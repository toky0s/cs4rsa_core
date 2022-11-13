using CommunityToolkit.Mvvm.Messaging.Messages;

using cs4rsa_core.Dialogs.DialogResults;

namespace cs4rsa_core.Messages.Publishers.Dialogs
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
