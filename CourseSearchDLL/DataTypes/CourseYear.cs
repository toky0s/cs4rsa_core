using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseSearchDLL.DataTypes
{
    /// <summary>
    /// Class này đại diện cho thông tin của một năm học, đóng vai trò
    /// quan trọng trong việc xác định được course cần thiết trong các
    /// crawler liên quan tới cào thông tin môn học và bảng điểm sinh viên.
    /// </summary>
    public class CourseYear
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public IEnumerable<CourseSemester> CourseSemesters { get; set; }
    }
}
