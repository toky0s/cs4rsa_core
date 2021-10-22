using Cs4rsaDatabaseService.DataProviders;
using System.Collections.Generic;
using System.Linq;

namespace ProgramSubjectCrawlerService.DataTypes
{
    /// <summary>
    /// Đại diện cho toàn bộ chương trình học của một sinh viên nào đó.
    /// </summary>
    public class ProgramDiagram
    {
        public List<ProgramFolder> ProgramFolders { get; set; } = new List<ProgramFolder>();
        private readonly Cs4rsaDbContext _cs4rsaDbContext;

        public ProgramDiagram(ProgramFolder outlineRoot, ProgramFolder physicalEducationRoot,
                              ProgramFolder industryOutlineRoot, ProgramFolder specializedRoot, Cs4rsaDbContext cs4rsaDbContext)
        {
            ProgramFolders.Add(outlineRoot);
            ProgramFolders.Add(physicalEducationRoot);
            ProgramFolders.Add(industryOutlineRoot);
            ProgramFolders.Add(specializedRoot);
            _cs4rsaDbContext = cs4rsaDbContext;
        }

        public List<ProgramSubject> GetAllProSubject()
        {
            List<ProgramSubject> programSubjects = new List<ProgramSubject>();
            foreach (ProgramFolder folder in ProgramFolders)
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
            foreach (ProgramFolder folder in ProgramFolders)
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
            foreach (ProgramFolder folder in ProgramFolders)
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
            Cs4rsaDatabaseService.Models.ProgramSubject programSubjectModel = _cs4rsaDbContext.ProgramSubjects
                .Where(p => p.CourseId == programSubject.CourseId)
                .FirstOrDefault();
            List<Cs4rsaDatabaseService.Models.PreParSubject> preParSubjects = programSubjectModel.ParProDetails
                .Select(parProDetail => parProDetail.PreParSubject)
                .ToList();

            List<string> subjectCodes = preParSubjects.Select(preParSubject => preParSubject.SubjectCode).ToList();
            return subjectCodes.Select(item => GetProgramSubject(item)).ToList();
        }
    }
}
