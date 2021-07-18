using cs4rsa.Interfaces;
using System;
using System.Collections.Generic;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho toàn bộ chương trình học của một sinh viên nào đó.
    /// </summary>
    public class ProgramDiagram
    {
        private List<ProgramFolder> _programFolders = new List<ProgramFolder>();

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
        /// Trả về tên Folder chứa của một ProgramSubject.
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public string GetFolderName(ProgramSubject subject)
        {
            string name;
            foreach (ProgramFolder folder in _programFolders)
            {
                name = folder.GetNameOfFolderContainsThisSubject(subject);
                if (name != null)
                    return name;
            }
            return null;
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
    }
}
