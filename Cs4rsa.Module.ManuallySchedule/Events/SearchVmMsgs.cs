using System.Collections.Generic;

using Cs4rsa.Module.ManuallySchedule.Models;

using Prism.Events;

namespace Cs4rsa.Module.ManuallySchedule.Events
{
    /// <summary>
    /// Danh sách các Message được sử dụng trong SearchViewModel
    /// </summary>
    internal class SearchVmMsgs
    {
        /// <summary>
        /// Xoá một môn học
        /// </summary>
        internal class DelSubjectMsg : PubSubEvent<SubjectModel>{ }

        /// <summary>
        /// Xoá hết tất cả các môn đã tải
        /// </summary>
        internal class DelAllSubjectMsg : PubSubEvent { }

        /// <summary>
        /// Thay đổi môn học đã lựa chọn hiện tại
        /// </summary>
        internal class SelectedSubjectChangedMsg : PubSubEvent<SubjectModel> { }
        /// <summary>
        /// Select danh sách các ClassGroupModel
        /// </summary>
        internal class SelectCgmsMsg : PubSubEvent<IEnumerable<ClassGroupModel>> { }

        /// <summary>
        /// Hoàn tác một Subject đã xoá.
        /// </summary>
        internal class UndoDelMsg : PubSubEvent<SearchValues.UndoDelValue> { }

        internal class UndoDeleteAllMsg : PubSubEvent<SearchValues.UndoDelAllValue> { }
    }
}
