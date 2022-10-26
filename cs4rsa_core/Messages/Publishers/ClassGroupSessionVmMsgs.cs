using CommunityToolkit.Mvvm.Messaging.Messages;

using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

namespace cs4rsa_core.Messages.Publishers
{
    internal sealed class ClassGroupSessionVmMsgs
    {
        /// <summary>
        /// Xử lý sự kiện thêm Class Group
        /// </summary>
        internal sealed class ClassGroupAddedMsg : ValueChangedMessage<ClassGroupModel>
        {
            public ClassGroupAddedMsg(ClassGroupModel value) : base(value)
            {
            }
        }
    }
}
