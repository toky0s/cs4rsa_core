using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;

using Prism.Events;

namespace Cs4rsa.Module.ManuallySchedule.Events
{
    public class ShowDetailsSchoolClassesVmMsgs
    {
        /// <summary>
        /// Thoát lựa chọn lớp học
        /// </summary>
        internal sealed class ExitChooseMsg : PubSubEvent<ClassGroupResult> { }
    }
}
