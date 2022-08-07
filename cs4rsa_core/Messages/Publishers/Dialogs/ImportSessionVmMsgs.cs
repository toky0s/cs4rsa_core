using cs4rsa_core.Dialogs.DialogResults;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace cs4rsa_core.Messages.Publishers.Dialogs
{
    internal sealed class ImportSessionVmMsgs
    {
        /// <summary>
        /// Hoàn thành và thoát việc import subject.
        /// </summary>
        internal sealed class ExitImportSubjectMsg : ValueChangedMessage<SessionManagerResult>
        {
            public ExitImportSubjectMsg(SessionManagerResult value) : base(value)
            {
            }
        }
    }
}
