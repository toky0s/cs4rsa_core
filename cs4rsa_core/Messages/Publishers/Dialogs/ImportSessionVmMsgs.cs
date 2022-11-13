using CommunityToolkit.Mvvm.Messaging.Messages;

using cs4rsa_core.Dialogs.DialogResults;

using System.Collections.Generic;

namespace cs4rsa_core.Messages.Publishers.Dialogs
{
    internal sealed class ImportSessionVmMsgs
    {
        /// <summary>
        /// Hoàn thành và thoát việc import subject.
        /// </summary>
        internal sealed class ExitImportSubjectMsg : ValueChangedMessage<IEnumerable<UserSubject>>
        {
            public ExitImportSubjectMsg(IEnumerable<UserSubject> value) : base(value)
            {
            }
        }
    }
}
