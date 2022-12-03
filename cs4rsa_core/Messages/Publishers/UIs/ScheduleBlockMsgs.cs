using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Utils.Models;

namespace Cs4rsa.Messages.Publishers.UIs
{
    internal sealed class ScheduleBlockMsgs
    {
        /// <summary>
        /// ScheduleBlockSelectedMsg
        /// 
        /// Khi người dùng thực hiện chọn một ScheduleBlock
        /// </typeparam>
        internal sealed class SelectedMsg : ValueChangedMessage<TimeBlock>
        {
            public SelectedMsg(TimeBlock scheduleItem) : base(scheduleItem)
            {
            }
        }
    }
}
