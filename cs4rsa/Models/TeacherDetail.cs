using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models
{
    public class TeacherDetail
    {
        public int TeacherDetailId { get; set; }
        public Teacher Teacher { get; set; }
        public string SubjectName { get; set; }
    }
}
