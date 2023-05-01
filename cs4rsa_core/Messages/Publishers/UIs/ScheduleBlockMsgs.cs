using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Controls;
using Cs4rsa.Models;

namespace Cs4rsa.Messages.Publishers.UIs
{
    internal sealed class ScheduleBlockMsgs
    {
        /// <summary>
        /// ScheduleBlockSelectedMsg
        /// 
        /// Khi người dùng thực hiện chọn một ScheduleBlock
        /// </typeparam>
        internal sealed class SelectedMsg : ValueChangedMessage<object>
        {
            public SelectedMsg(object scheduleItem) : base(scheduleItem)
            {
            }
        }

        /// <summary>
        /// ScheduleBlock HoveredMsg
        /// </summary>
        /// <remarks>
        /// Xảy ra khi người dùng thực hiện Hover lên một ScheduleBlock.
        /// Một giải pháp thay thế rõ ràng hơn Tooltip để hiển thị thông
        /// tin của block.
        /// </remarks>
        internal sealed class HoveredMsg : ValueChangedMessage<TimeBlock>
        {
            public HoveredMsg(TimeBlock timeBlock) : base(timeBlock)
            {
            }
        }
        /// <summary>
        /// ScheduleBlock LeavedMsg
        /// </summary>
        /// <remarks>
        /// Tắt hiển thị thông tin của ScheduleBlock.
        /// </remarks>
        internal sealed class LeavedMsg : RequestMessage<ScheduleBlock>
        {
            public LeavedMsg() : base()
            {
            }
        }
    }
}
