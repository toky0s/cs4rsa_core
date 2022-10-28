using cs4rsa_core.Dialogs.DialogResults;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using cs4rsa_core.Models;

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

        internal sealed class AddSubjectMsg : ValueChangedMessage<Tuple<string, string, int>>
        {
            /// <summary>
            /// Download and add Suject
            /// </summary>
            public AddSubjectMsg(Tuple<string, string, int> value) : base(value)
            {

            }
        }
    }
}
