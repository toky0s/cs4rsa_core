using cs4rsa_core.Dialogs.DialogResults;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace cs4rsa_core.Messages.Publishers.Dialogs
{
    internal sealed class SubjectImporterVmMsgs
    {
        internal sealed class ExitImportSubjectMsg : ValueChangedMessage<Tuple<ImportResult, SessionManagerResult>>
        {
            /// <summary>
            /// Thoát import subject vào SearchSessionViewModel
            /// </summary>
            public ExitImportSubjectMsg(Tuple<ImportResult, SessionManagerResult> value) : base(value)
            {
            }
        }
    }
}
