using Prism.Events;

namespace Cs4rsa.Module.ManuallySchedule.Events
{
    public sealed class SolveConflictVmMsgs
    {
        /// <summary>
        /// Remove Choiced Class Message
        /// 
        /// Loại bỏ lớp đã chọn.
        /// </summary>
        internal sealed class RemoveChoicedClassMsg : PubSubEvent<string> { }
    }
}
