using CommunityToolkit.Mvvm.Messaging.Messages;

using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;

namespace cs4rsa_core.Messages.Publishers
{
    /// <summary>
    /// Danh sách các Message được sử dụng trong SearchSessionViewModel
    /// </summary>
    internal sealed class SearchVmMsgs
    {
        /// <summary>
        /// Xoá một môn học
        /// </summary>
        internal sealed class DelSubjectMsg : ValueChangedMessage<SubjectModel>
        {
            /// <summary>
            /// value: Môn học đã xoá
            /// </summary>
            /// <param name="value">Môn học đã xoá</param>
            public DelSubjectMsg(SubjectModel value) : base(value)
            {
            }
        }

        /// <summary>
        /// Chọn một môn học trong danh sách đã tải
        /// </summary>
        internal sealed class SubjectItemChangedMsg : ValueChangedMessage<Tuple<int, int>>
        {
            /// <summary>
            /// Item1: Tổng tín chỉ
            /// Item2: Tổng số môn
            /// </summary>
            public SubjectItemChangedMsg(Tuple<int, int> value) : base(value)
            {
            }
        }

        /// <summary>
        /// Xoá hết tất cả các môn đã tải
        /// </summary>
        internal sealed class DelAllSubjectMsg : ValueChangedMessage<object>
        {
            public DelAllSubjectMsg(object value) : base(value)
            {
            }
        }

        /// <summary>
        /// Thay đổi môn học đã lựa chọn hiện tại
        /// </summary>
        internal sealed class SelectedSubjectChangedMsg : ValueChangedMessage<SubjectModel>
        {
            public SelectedSubjectChangedMsg(SubjectModel value) : base(value)
            {
            }
        }

        /// <summary>
        /// Select danh sách các ClassGroupModel
        /// </summary>
        internal sealed class SelectClassGroupModelsMsg : ValueChangedMessage<IEnumerable<ClassGroupModel>>
        {
            public SelectClassGroupModelsMsg(IEnumerable<ClassGroupModel> value) : base(value)
            {
            }
        }
    }

}
