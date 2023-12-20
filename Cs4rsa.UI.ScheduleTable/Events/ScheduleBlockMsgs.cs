using Cs4rsa.UI.ScheduleTable.CustomControls;
using Cs4rsa.UI.ScheduleTable.Models;

using Prism.Events;

namespace Cs4rsa.Messages.Publishers.UIs
{
    public class ScheduleBlockMsgs
    {
        /// <summary>
        /// ScheduleBlockSelectedMsg
        /// 
        /// Khi người dùng thực hiện chọn một ScheduleBlock
        /// </typeparam>
        public class SelectedMsg : PubSubEvent<object> { }

        /// <summary>
        /// ScheduleBlock HoveredMsg
        /// </summary>
        /// <remarks>
        /// Xảy ra khi người dùng thực hiện Hover lên một ScheduleBlock.
        /// Một giải pháp thay thế rõ ràng hơn Tooltip để hiển thị thông
        /// tin của block.
        /// </remarks>
        public class HoveredMsg : PubSubEvent<TimeBlock> { }
        /// <summary>
        /// ScheduleBlock LeavedMsg
        /// </summary>
        /// <remarks>
        /// Tắt hiển thị thông tin của ScheduleBlock.
        /// </remarks>
        public class LeavedMsg : PubSubEvent<ScheduleBlock> { }
    }
}
