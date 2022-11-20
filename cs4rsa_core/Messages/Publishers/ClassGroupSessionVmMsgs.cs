using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Services.SubjectCrawlerSvc.Models;

namespace Cs4rsa.Messages.Publishers
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
