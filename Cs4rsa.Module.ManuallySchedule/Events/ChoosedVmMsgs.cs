using Prism.Events;

namespace Cs4rsa.Module.ManuallySchedule.Events
{
    internal sealed class ChoosedVmMsgs
    {

        /// <summary>
        /// Xoá tất cả Class Group đang hiển thị trên lịch
        /// </summary>
        internal sealed class DelAllClassGroupChoiceMsg : PubSubEvent { }
    }
}
