using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Interfaces;

namespace cs4rsa.Implements
{
    /// <summary>
    /// Implement này thực hiện việc lưu một Subject thuộc chương trình học
    /// của một sinh viên vào database bao gồm các môn tiên quyết và môn song hành.
    /// </summary>
    class ProgramSubjectSaver : ISaver<ProgramSubject>
    {
        public void Save(ProgramSubject obj)
        {
            if (!Cs4rsaDataView.IsExistsProgramSubject(obj.SubjectCode))
                Cs4rsaDataEdit.AddProgramSubject(obj);
            foreach (string preSubject in obj.PrerequisiteSubjects)
            {
                if (!Cs4rsaDataView.IsExistsPreParSubject(preSubject))
                    Cs4rsaDataEdit.AddPreParSubject(preSubject);
                if (!Cs4rsaDataView.IsExistsPreDetail(obj.SubjectCode, preSubject))
                    Cs4rsaDataEdit.AddPreSubjectDetail(obj.SubjectCode, preSubject);
            }
            foreach (string parSubject in obj.ParallelSubjects)
            {
                if (!Cs4rsaDataView.IsExistsPreParSubject(parSubject))
                    Cs4rsaDataEdit.AddPreParSubject(parSubject);
                if (!Cs4rsaDataView.IsExistsParDetail(obj.SubjectCode, parSubject))
                    Cs4rsaDataEdit.AddParSubjectDetail(obj.SubjectCode, parSubject);
            }
        }
    }
}
