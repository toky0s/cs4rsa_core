using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Implements
{
    /// <summary>
    /// Saver này không có thực hiện lưu các ProgramSubject bên trong. Mà chỉ
    /// thực hiện map Student với các ProgramSubject trong database.
    /// </summary>
    class ProgramDiagramSaver : ISaver<ProgramDiagram>
    {
        public void Save(ProgramDiagram obj, object[] parameters = null)
        {
            string studentId = (string)parameters[0];
            List<ProgramSubject> programSubjects = obj.GetAllProSubject();
            foreach(ProgramSubject subject in programSubjects)
            {
                if (!Cs4rsaDataView.IsExistsProSubjectDetail(studentId, subject.SubjectCode))
                    Cs4rsaDataEdit.AddProSubjectDetail(studentId, subject.SubjectCode);
            }
        }
    }
}
