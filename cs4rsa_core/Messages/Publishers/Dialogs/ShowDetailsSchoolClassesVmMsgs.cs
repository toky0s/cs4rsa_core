using CommunityToolkit.Mvvm.Messaging.Messages;
using cs4rsa_core.Dialogs.DialogResults;

namespace cs4rsa_core.Messages.Publishers.Dialogs
{
    internal class ShowDetailsSchoolClassesVmMsgs
    {
        /// <summary>
        /// Thoát lựa chọn lớp học
        /// </summary>
        internal sealed class ExitChooseMsg : ValueChangedMessage<ClassGroupResult>
        {
            public ExitChooseMsg(ClassGroupResult value) : base(value)
            {
            }
        }
    }
}
