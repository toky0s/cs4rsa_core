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
        /// <summary>
        /// Tên bộ lịch
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        /// <summary>
        /// Ngày lưu
        /// </summary>
        public DateTime SaveDate { get; set; }
        /// <summary>
        /// Giá trị học kỳ hiện tại, đại diện bằng một số
        /// </summary>
        public string SemesterValue { get; set; }
        /// <summary>
        /// Giá trị năm học hiện tại, đại diện bằng một số
        /// </summary>
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
        /// <summary>
        /// Cờ kiểm tra bộ lịch đã vượt quá học kỳ hiện tại hay chưa
        /// </summary>
        public bool IsExpired { get; set; }
    }
}
