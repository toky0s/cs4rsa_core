using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Models;

namespace Cs4rsa.Messages.Publishers
{
    internal sealed class AutoScheduleVmMsgs
    {
        /// <summary>
        /// Hiển thị lớp đã chọn lên lịch
        /// </summary>
        internal sealed class ShowOnSimuMsg : ValueChangedMessage<CombinationModel>
        {
            public ShowOnSimuMsg(CombinationModel value) : base(value)
            {
            }
        }
    }
}
