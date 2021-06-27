using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Interfaces;

namespace cs4rsa_test
{
    class FakeStudentSaver : IStudentSaver
    {
        public int Save(StudentInfo studentInfo)
        {
            Console.WriteLine("Save "+studentInfo.Name);
            return 1;
        }
    }
}
