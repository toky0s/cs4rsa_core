using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;

using Prism.Events;

using System.Collections.Generic;

namespace Cs4rsa.Module.ManuallySchedule.Events
{
    internal sealed class ImportSessionVmMsgs
    {
        /// <summary>
        /// Hoàn thành và thoát việc import subject.
        /// 
        /// Chuyển xử lý đến SearchSessionViewModel
        /// </summary>
        internal sealed class ExitImportSubjectMsg : PubSubEvent<IEnumerable<UserSubject>> { }
    }
}
