using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Models.Interfaces;

namespace cs4rsa.Models
{
    /// <summary>
    /// Đại diện cho một ô trong ScheduleRow. Là một ô trong bảng mô phỏng hiển thị.
    /// </summary>
    public class TimeBlock
    {
        private readonly string _defaultForeground = "#000000";
        public string Background { get; set; }
        public string Foreground { get; set; }
        public string Text { get; set; }
        public TimeBlock(SchoolClassModel schoolClassModel)
        {
            Background = schoolClassModel.Color;
            Foreground = _defaultForeground;
            Text = schoolClassModel.SchoolClassName;
        }

        public TimeBlock(StudyTimeIntersect studyTimeIntersect)
        {
            Background = "#800000";
            Foreground = "#FFFFFF";
            Text = "Xung đột";
        }
    }
}
