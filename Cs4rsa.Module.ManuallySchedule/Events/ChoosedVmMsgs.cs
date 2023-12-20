using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.UI.ScheduleTable.Models;
using Prism.Events;

namespace Cs4rsa.Module.ManuallySchedule.Events
{
    internal sealed class ChoosedVmMsgs
    {
        /// <summary>
        /// Conflict Collection Changed Message
        /// 
        /// Có sự thay đổi giữa danh sách xung đột thời gian.
        /// </summary>
        internal sealed class ConflictCollChangedMsg : PubSubEvent<IEnumerable<ConflictModel>> { }

        /// <summary>
        /// Place Conflict Collection Changed Message
        /// 
        /// Có sự thay đổi giữa danh sách xung đột vị trí.
        /// </summary>
        internal sealed class PlaceConflictCollChangedMsg : PubSubEvent<IEnumerable<PlaceConflictFinderModel>> { }

        /// <summary>
        /// Delete Class Group Choice Message
        /// 
        /// Xoá Class Group đã chọn.
        /// </summary>
        internal sealed class DelClassGroupChoiceMsg : PubSubEvent<ClassGroupModel> { }

        /// <summary>
        /// Delete All Class Group Choice Message
        /// 
        /// Xoá tất cả Class Group đã chọn.
        /// </summary>
        internal sealed class DelAllClassGroupChoiceMsg : PubSubEvent<DBNull> { }

        internal sealed class UndoDelAllMsg : PubSubEvent<
            Tuple<
                IEnumerable<ClassGroupModel>, 
                ObservableCollection<ConflictModel>, 
                ObservableCollection<PlaceConflictFinderModel>
            >> { }

        internal sealed class UndoDelMsg : PubSubEvent<ClassGroupModel> { }

        internal sealed class ClassGroupAddedMsg : PubSubEvent<ClassGroupModel> { }
    }
}
