using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;

using static Cs4rsa.Messages.Values.SearchValues;

namespace Cs4rsa.Messages.Publishers
{
    /// <summary>
    /// Danh sách các Message được sử dụng trong SearchViewModel
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
        internal sealed class DelAllSubjectMsg : RequestMessage<SearchVmMsgs>
        {
            public DelAllSubjectMsg() : base()
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
        internal sealed class SelectCgmsMsg : ValueChangedMessage<IEnumerable<ClassGroupModel>>
        {
            public SelectCgmsMsg(IEnumerable<ClassGroupModel> value) : base(value)
            {
            }
        }

        /// <summary>
        /// Hoàn tác một Subject đã xoá.
        /// </summary>
        internal sealed class UndoDelMsg : ValueChangedMessage<UndoDelValue>
        {
            public UndoDelMsg(UndoDelValue value) : base(value)
            {
            }
        }

        internal sealed class UndoDeleteAllMsg : ValueChangedMessage<UndoDelAllValue>
        {
            public UndoDeleteAllMsg(UndoDelAllValue value) : base(value)
            {
            }
        }
    }
}
