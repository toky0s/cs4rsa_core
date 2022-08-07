using cs4rsa_core.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace cs4rsa_core.Messages.Publishers
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
