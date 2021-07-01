using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Interfaces;

namespace cs4rsa_test
{
    public class FakeSubjectSaver : ISaver<ProgramSubject>
    {
        public void Save(ProgramSubject obj, object[] parameters = null)
        {
            Console.WriteLine("Save " + obj.SubjectName);
        }
    }
}
