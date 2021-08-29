using cs4rsa.Database;
using cs4rsa.Implements;
using cs4rsa.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho toàn bộ chương trình học của một sinh viên nào đó.
    /// </summary>
    public class ProgramDiagram
    {
        private List<ProgramFolder> _programFolders = new List<ProgramFolder>();
        public List<ProgramFolder> ProgramFolders
        {
            get { return _programFolders; }
            set { _programFolders = value; }
        }

        public ProgramDiagram(ProgramFolder outlineRoot, ProgramFolder physicalEducationRoot,
                              ProgramFolder industryOutlineRoot, ProgramFolder specializedRoot)
        {
            _programFolders.Add(outlineRoot);
            _programFolders.Add(physicalEducationRoot);
            _programFolders.Add(industryOutlineRoot);
            _programFolders.Add(specializedRoot);
        }

        public List<ProgramSubject> GetAllProSubject()
        {
            List<ProgramSubject> programSubjects = new List<ProgramSubject>();
            foreach (ProgramFolder folder in _programFolders)
            {
                programSubjects.AddRange(folder.GetProgramSubjects());
            }
            return programSubjects;
        }

        /// <summary>
        /// Lấy ra một folder trong cây folder này dựa theo tên.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public ProgramFolder GetFolder(string folderName)
        {
            foreach (ProgramFolder folder in _programFolders)
            {
                ProgramFolder result = folder.FindProgramFolder(folderName);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Lấy ra một Program Subject có trong diagram này.
        /// </summary>
        /// <param name="subjectCode"></param>
        /// <returns></returns>
        public ProgramSubject GetProgramSubject(string subjectCode)
        {
            foreach (ProgramFolder folder in _programFolders)
            {
                ProgramSubject result = folder.GetProgramSubject(subjectCode);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Lấy ra danh sách các môn tiên quyết của một ProgramSubject.
        /// </summary>
        /// <param name="programSubject">ProgramSubject</param>
        /// <returns></returns>
        public List<ProgramSubject> GetPreProgramSubject(ProgramSubject programSubject)
        {
            List<string> subjectCodes = programSubject.PrerequisiteSubjects;
            return subjectCodes.Select(item => GetProgramSubject(item)).ToList();
        }

        /// <summary>
        /// Lấy ra danh sách các môn song hành của một ProgramSubject.
        /// </summary>
        /// <param name="programSubject">ProgramSubject</param>
        /// <returns></returns>
        public List<ProgramSubject> GetParProgramSubject(ProgramSubject programSubject)
        {
            List<string> subjectCodes = Cs4rsaDataView.GetParSubjects(programSubject.CourseId);
            return subjectCodes.Select(item => GetProgramSubject(item)).ToList();
        }
    }
}
