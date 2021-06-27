using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Class này đại diện cho thông tin của một sinh viên.
    /// </summary>
    public class StudentInfo
    {
        public string Name { get; set; }
        public string StudentId { get; set; }
        public string SpecialString { get; set; }
        public string Birthday { get; set; }
        public string CMND { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
    }
}
