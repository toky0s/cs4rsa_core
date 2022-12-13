using Cs4rsa.Cs4rsaDatabase.Interfaces;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes
{
    /// <summary>
    /// Đại diện cho toàn bộ chương trình học của một sinh viên nào đó.
    /// </summary>
    public class ProgramDiagram
    {
        public List<ProgramFolder> ProgramFolders { get; set; }
        private readonly IUnitOfWork _unitOfWork;

        public ProgramDiagram(
            ProgramFolder outlineRoot,
            ProgramFolder physicalEducationRoot,
            ProgramFolder industryOutlineRoot,
            ProgramFolder specializedRoot,
            IUnitOfWork unitOfWork
        )
        {
            ProgramFolders = new()
            {
                outlineRoot,
                physicalEducationRoot,
                industryOutlineRoot,
                specializedRoot
            };
            _unitOfWork = unitOfWork;
        }

        public List<ProgramSubject> GetAllProSubject()
        {
            List<ProgramSubject> programSubjects = new();
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
                {
                    return result;
                }
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
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Lấy ra danh sách các môn tiên quyết của một DbProgramSubject.
        /// </summary>
        public IEnumerable<ProgramSubject> GetPreProgramSubjects(ProgramSubject programSubject)
        {
            IEnumerable<string> subjectCodes = programSubject.PrerequisiteSubjects;
            return subjectCodes.Select(item => GetProgramSubject(item));
        }

        /// <summary>
        /// Lấy ra danh sách các môn song hành của một DbProgramSubject.
        /// </summary>
        public async Task<IEnumerable<ProgramSubject>> GetParProgramSubject(ProgramSubject programSubject)
        {
            Cs4rsaDatabase.Models.DbProgramSubject programSubjectModel = await _unitOfWork.ProgramSubjects.GetByCourseIdAsync(programSubject.CourseId);
            IEnumerable<Cs4rsaDatabase.Models.DbPreParSubject> preParSubjects = programSubjectModel.ParProDetails
                .Select(parProDetail => parProDetail.PreParSubject);

            IEnumerable<string> subjectCodes = preParSubjects.Select(preParSubject => preParSubject.SubjectCode);
            return subjectCodes.Select(item => GetProgramSubject(item));
        }
    }
}
