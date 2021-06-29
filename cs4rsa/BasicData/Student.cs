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
        private ProgramDiagram _diagram;

        public StudentInfo Info => _info;
        public ProgramDiagram ProgramDiagram => _diagram;

        public Student(StudentInfo info, ProgramDiagram diagram)
        {
            _info = info;
            _diagram = diagram;
        }
    }
}
