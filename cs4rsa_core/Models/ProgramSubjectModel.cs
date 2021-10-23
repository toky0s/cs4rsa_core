using cs4rsa_core.Models.Bases;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using HelperService;
using ProgramSubjectCrawlerService.DataTypes;
using ProgramSubjectCrawlerService.DataTypes.Enums;
using System.Linq;

namespace cs4rsa_core.Models
{
    /// <summary>
    /// ProgramSubjectModel khác ProgramSubject ở chỗ chúng có các phương thức check trong cây diagram
    /// nhằm xác định việc người dùng có thể chọn subject đó hay không, điều mà ProgramSubject không thể
    /// làm được vì nó chỉ chứa thông tin của chính nó mà không có tương tác nào với các Folder hay Subject
    /// khác trong cây.
    /// </summary>
    public class ProgramSubjectModel: TreeItem
    {
        public ProgramSubjectCrawlerService.DataTypes.ProgramSubject ProgramSubject { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string FolderName { get; set; }
        public string Color { get; set; }

        private int _studyUnit;
        public int StudyUnit
        {
            get => _studyUnit == 0 ? ProgramSubject.StudyUnit : _studyUnit;
            set => _studyUnit = value;
        }
        public StudyState StudyState { get; set; }

        public bool IsDone => ProgramSubject.IsDone();
        public bool IsAvaiable => IsAvaiableInThisSemester();
        public string CourseId => ProgramSubject.CourseId;
        public string ChildOfNode => ProgramSubject.ChildOfNode;

        private Cs4rsaDbContext _cs4rsaDbContext;
        public ProgramSubjectModel(ProgramSubjectCrawlerService.DataTypes.ProgramSubject programSubject, ColorGenerator colorGenerator, Cs4rsaDbContext cs4rsaDbContext) : base(programSubject.SubjectName, programSubject.Id)
        {
            _cs4rsaDbContext = cs4rsaDbContext;
            ProgramSubject = programSubject;
            SubjectCode = programSubject.SubjectCode;
            SubjectName = programSubject.SubjectName;
            FolderName = programSubject.ParrentNodeName;
            StudyState = programSubject.StudyState;
            NodeType = programSubject.GetNodeType();
            Color = colorGenerator.GetColor(int.Parse(programSubject.CourseId));
        }

        /// <summary>
        /// Kiểm tra xem ProgramSubjectModel này có sẵn trong học kỳ này hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsAvaiableInThisSemester()
        {
            string[] subjectCodeSlices = ProgramSubject.SubjectCode.Split(new char[] { ' ' });
            string discipline = subjectCodeSlices[0];
            string keyword1 = subjectCodeSlices[1];
            var query = from ds in _cs4rsaDbContext.Disciplines
                        join kw in _cs4rsaDbContext.Keywords
                        on ds.DisciplineId equals kw.DisciplineId
                        where ds.Name == kw.Keyword1
                        select kw;
            return query.Count() > 0;

            //return Cs4rsaDataView.IsExistsSubjectInThisSemester(ProgramSubject);
        }
    }
}
