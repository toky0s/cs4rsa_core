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
        /// Xoá hết tất cả các môn đã tải
        /// </summary>
        internal class DelAllSubjectMsg : PubSubEvent { }
    }
}
