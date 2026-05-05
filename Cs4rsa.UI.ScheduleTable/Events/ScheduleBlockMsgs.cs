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
    }
}
