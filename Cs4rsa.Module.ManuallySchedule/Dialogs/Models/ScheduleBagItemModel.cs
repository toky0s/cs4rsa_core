using Cs4rsa.Module.ManuallySchedule.Utils;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Models
{
    public class ScheduleBagItemModel: BindableBase
    {
        public int ScheduleDetailId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string ClassGroup { get; set; }
        public string RegisterCode { get; set; }
        public string SelectedSchoolClass { get; set; }
    }
}
