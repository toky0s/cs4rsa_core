using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Models
{
    public class ScheduleBagModel : BindableBase
    {
        public int UserScheduleId { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        public DateTime SaveDate { get; set; }
        public string SemesterValue { get; set; }
        public string YearValue { get; set; }
        /// <summary>
        /// Thông tin học kỳ, ví dụ: Học kỳ I
        /// </summary>
        public string Semester { get; set; }
        /// <summary>
        /// Thông tin năm học, ví dụ: Năm học 2024-2025
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// Số phần tử của danh sách này lớn hơn 0 đồng nghĩa nó đã được load từ database
        /// </summary>
        public ObservableCollection<ScheduleBagItemModel> ScheduleBagItemModels { get; set; }
    }
}
