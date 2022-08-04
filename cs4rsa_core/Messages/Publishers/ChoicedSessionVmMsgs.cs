using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using SubjectCrawlService1.Models;
using System.Collections.Generic;

namespace cs4rsa_core.Messages.Publishers
{
    internal sealed class ChoicedSessionVmMsgs
    {
        /// <summary>
        /// Có sự thay đổi danh sách các lớp đã chọn. 
        /// Điều này sẽ gọi qua ScheduleTableViewModel để cập nhật lại mô phỏng
        /// </summary>
        internal sealed class ChoicesChangedMsg : ValueChangedMessage<IEnumerable<ClassGroupModel>>
        {
            public ChoicesChangedMsg(IEnumerable<ClassGroupModel> value) : base(value)
            {
            }
        }
    }
}
