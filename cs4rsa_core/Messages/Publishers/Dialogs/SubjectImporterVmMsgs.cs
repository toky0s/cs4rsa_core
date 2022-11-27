using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;

namespace Cs4rsa.Messages.Publishers.Dialogs
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
