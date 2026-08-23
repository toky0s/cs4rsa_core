using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;

using Prism.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Events
{
    public class ExitImportSubjectMsg : PubSubEvent<IEnumerable<UserSubject>>
    {
    }
}
