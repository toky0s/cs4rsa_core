using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    public class Student
    {
        private StudentInfo _info;

        public StudentInfo Info => _info;

        public Student(StudentInfo info)
        {
            _info = info;
        }
    }
}
