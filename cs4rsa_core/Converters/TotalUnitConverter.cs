using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cs4rsa.Converters
{
    /// <summary>
    /// Tính toán tổng số tín chỉ của một học kỳ trong chương trình học dự kiến.
    /// </summary>
    internal class TotalUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Binding.DoNothing;
            PlanTable planTable = (PlanTable)value;
            int totalUnit = 0;
            planTable.PlanRecords.ForEach(planRecord => totalUnit += planRecord.StudyUnit);
            return totalUnit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
