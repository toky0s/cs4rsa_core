using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Cs4rsa.Messages.States
{
    /// <summary>
    /// PhaseStore Messages :)))
    /// Những đoạn code cuối cùng trước khi đi nghĩa vụ quân sự.
    /// 
    /// BetweenPointChangedMsg
    ///     Thông báo rằng PhaseStore Between Point vừa được thay đổi.
    ///     Các ViewModel tương ứng sẽ thực hiện đánh giá lại bao gồm:
    ///     ChoosedSessionViewModel, ScheduleTableViewModel để thực hiện
    ///     đánh giá lại xung đột và vẽ lại bộ mô phỏng lịch.
    /// </summary>
    internal sealed class PhaseStoreMsgs
    {
        internal sealed class BetweenPointChangedMsg : ValueChangedMessage<int>
        {
            public BetweenPointChangedMsg(int value) : base(value)
            {
            }
        }
    }
}
