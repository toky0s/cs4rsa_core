using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Interfaces;

namespace cs4rsa_test
{
    class FakeStudentSaver : ISaver<StudentInfo>
    {
        public void Save(StudentInfo obj, object[] parameters = null)
        {
            Console.WriteLine("Save " + obj.Name);
        }
    }
}
