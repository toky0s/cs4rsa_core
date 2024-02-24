using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Models
{
    public class ScheduleBagModel: BindableBase
    {
        public int UserScheduleId { get; set; }
        public string Name { get; set; }
        public DateTime SaveDate { get; set; }
        public string SemesterValue { get; set; }
        public string YearValue { get; set; }
        public ObservableCollection<ScheduleBagItemModel> ScheduleBagItemModels { get; set; }
    }
}
