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

        internal sealed class UndoDelAllMsg : ValueChangedMessage<IEnumerable<ClassGroupModel>>
        {
            public UndoDelAllMsg(IEnumerable<ClassGroupModel> value) : base(value)
            {

            }
        }
    }
}
