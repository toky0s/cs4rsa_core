using cs4rsa.BasicData;
using cs4rsa.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Database;

namespace cs4rsa.Implements
{
    public class StudentSaver : ISaver<StudentInfo>
    {
        public void Save(StudentInfo obj, object[] parameters = null)
        {
            if (!Cs4rsaDataView.IsStudentExists(obj.SpecialString))
            {
                Cs4rsaDataEdit.AddStudent(obj);
            }
            else
            {
                Cs4rsaDataEdit.UpdateStudent(obj);
            }
        }
    }
}
