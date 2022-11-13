using CommunityToolkit.Mvvm.Messaging.Messages;

using cs4rsa_core.Services.ConflictSvc.Models;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

using System.Collections.Generic;

namespace cs4rsa_core.Messages.Publishers
{
    internal sealed class ChoicedSessionVmMsgs
    {
        /// <summary>
        /// Choice Changed Message
        /// 
        /// Có sự thay đổi danh sách các lớp đã chọn. 
        /// Điều này sẽ gọi qua ScheduleTableViewModel để cập nhật lại mô phỏng.
        /// </summary>
        internal sealed class ChoiceChangedMsg : ValueChangedMessage<IEnumerable<ClassGroupModel>>
        {
            public ChoiceChangedMsg(IEnumerable<ClassGroupModel> value) : base(value)
            {
            }
        }

        /// <summary>
        /// Conflict Collection Changed Message
        /// 
        /// Có sự thay đổi giữa danh sách xung đột thời gian.
        /// </summary>
        internal sealed class ConflictCollChangedMsg : ValueChangedMessage<IEnumerable<ConflictModel>>
        {
            public ConflictCollChangedMsg(IEnumerable<ConflictModel> value) : base(value)
            {
            }
        }

        /// <summary>
        /// Place Conflict Collection Changed Message
        /// 
        /// Có sự thay đổi giữa danh sách xung đột vị trí.
        /// </summary>
        internal sealed class PlaceConflictCollChangedMsg : ValueChangedMessage<IEnumerable<PlaceConflictFinderModel>>
        {
            public PlaceConflictCollChangedMsg(IEnumerable<PlaceConflictFinderModel> value) : base(value)
            {
            }
        }

        /// <summary>
        /// Delete Class Group Choice Message
        /// 
        /// Xoá Class Group đã chọn.
        /// </summary>
        internal sealed class DelClassGroupChoiceMsg : ValueChangedMessage<IEnumerable<ClassGroupModel>>
        {
            public DelClassGroupChoiceMsg(IEnumerable<ClassGroupModel> value) : base(value)
            {
            }
        }
    }
}
