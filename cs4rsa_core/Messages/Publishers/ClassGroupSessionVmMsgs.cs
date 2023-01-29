using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Services.SubjectCrawlerSvc.Models;

namespace Cs4rsa.Messages.Publishers
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
        internal sealed class ClassGroupAddedMsg : ValueChangedMessage<ClassGroupModel>
        {
            public ClassGroupAddedMsg(ClassGroupModel value) : base(value)
            {

            }
        }
    }
}
