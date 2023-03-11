using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Dialogs.DialogResults;

namespace Cs4rsa.Messages.Publishers.Dialogs
{
    public class ShowDetailsSchoolClassesVmMsgs
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
