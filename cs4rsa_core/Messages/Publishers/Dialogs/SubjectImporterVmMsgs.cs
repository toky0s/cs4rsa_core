using CommunityToolkit.Mvvm.Messaging.Messages;

using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;

namespace cs4rsa_core.Messages.Publishers.Dialogs
{
    internal sealed class SubjectImporterVmMsgs
    {
        internal sealed class ExitImportSubjectMsg : ValueChangedMessage<Tuple<IEnumerable<SubjectModel>, IEnumerable<UserSubject>>>
        {
            /// <summary>
            /// Thoát import subject vào SearchSessionViewModel
            /// </summary>
            public ExitImportSubjectMsg(Tuple<IEnumerable<SubjectModel>, IEnumerable<UserSubject>> value) : base(value)
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
