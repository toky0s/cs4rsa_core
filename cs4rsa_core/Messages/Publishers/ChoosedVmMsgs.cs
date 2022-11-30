using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;

namespace Cs4rsa.Messages.Publishers
{
    internal sealed class ChoosedVmMsgs
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
        /// ClassGroupSeletedMsg
        /// 
        /// Thêm một <see cref="ClassGroupModel"/> vào bộ mô phỏng.
        /// <br></br>
        /// <inheritdoc cref="ClassGroupModel"/>
        /// <list type="number">
        /// <item>
        /// SchedulerViewModel phân giải và render các ScheduleItem.
        /// </item>
        /// </list>
        /// </summary>
        internal sealed class ClassGroupSeletedMsg : ValueChangedMessage<ClassGroupModel>
        {
            public ClassGroupSeletedMsg(ClassGroupModel value) : base(value)
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
        internal sealed class DelClassGroupChoiceMsg : ValueChangedMessage<ClassGroupModel>
        {
            public DelClassGroupChoiceMsg(ClassGroupModel value) : base(value)
            {
            }
        }

        /// <summary>
        /// Delete All Class Group Choice Message
        /// 
        /// Xoá tất cả Class Group đã chọn.
        /// </summary>
        internal sealed class DelAllClassGroupChoiceMsg : ValueChangedMessage<DBNull>
        {
            public DelAllClassGroupChoiceMsg(DBNull value) : base(value)
            {
            }
        }
    }
}
