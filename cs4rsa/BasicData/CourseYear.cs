using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
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

        public CourseYear(string name, string value, IEnumerable<CourseSemester> courseSemesters)
        {
            Name = name;
            Value = value;
            CourseSemesters = courseSemesters;
        }
    }
}
