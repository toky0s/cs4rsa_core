using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Dialogs.DialogResults;

using System.Collections.Generic;

namespace Cs4rsa.Messages.Publishers.Dialogs
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
