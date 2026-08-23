using Cs4rsa.Module.ManuallySchedule.Models;
using Prism.Events;

namespace Cs4rsa.Module.ManuallySchedule.Events
{
    internal sealed class ClassGroupSessionVmMsgs
    {
        /// <summary>
        /// Xử lý sự kiện chọn ClassGroupModel
        /// 
        ///<list type ="table">
        ///    <listheader>
        ///        <term>ViewModel xử lý</term>
        ///        <description>Xử lý liên quan</description>
        ///    </listheader>
        ///    <item>
        ///        <term>ChoosedSessionViewModel</term>
        ///        <description>Thêm ClassGroupModel vào danh sách đã chọn</description>
        ///    </item>
        ///</list>
        /// </summary>
        internal sealed class ClassGroupAddedMsg : PubSubEvent<ClassGroupModel> { }
    }
}
