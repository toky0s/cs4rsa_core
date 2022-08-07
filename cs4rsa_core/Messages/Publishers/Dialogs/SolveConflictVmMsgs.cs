using CommunityToolkit.Mvvm.Messaging.Messages;

namespace cs4rsa_core.Messages.Publishers.Dialogs
{
    internal sealed class SolveConflictVmMsgs
    {
        /// <summary>
        /// Remove Choiced Class Message
        /// 
        /// Loại bỏ lớp đã chọn.
        /// </summary>
        internal sealed class RemoveChoicedClassMsg : ValueChangedMessage<string>
        {
            /// <summary>
            /// value: Tên lớp muốn loại bỏ
            /// </summary>
            /// <param name="value">Tên lớp muốn loại bỏ</param>
            public RemoveChoicedClassMsg(string value) : base(value)
            {
            }
        }
    }
}
