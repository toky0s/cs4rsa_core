using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Models.Bases;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Utils;

using System.Threading.Tasks;

namespace Cs4rsa.Models
{
    /// <summary>
    /// ProgramSubjectModel khác ProgramSubject ở chỗ chúng có các phương thức check trong cây diagram
    /// nhằm xác định việc người dùng có thể chọn subject đó hay không, điều mà ProgramSubject không thể
    /// làm được vì nó chỉ chứa thông tin của chính nó mà không có tương tác nào với các Folder hay Subject
    /// khác trong cây.
    /// </summary>
    public class ProgramSubjectModel : TreeItem
    {
        public ProgramSubject ProgramSubject { get; set; }
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
        public bool IsAvaiable { get; set; }
        public string CourseId => ProgramSubject.CourseId;
        public string ChildOfNode => ProgramSubject.ChildOfNode;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ColorGenerator _colorGenerator;
        private ProgramSubjectModel(ProgramSubject programSubject,
            ColorGenerator colorGenerator, IUnitOfWork unitOfWork) : base(programSubject.SubjectName, programSubject.Id)
        {
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;
            ProgramSubject = programSubject;
            SubjectCode = programSubject.SubjectCode;
            SubjectName = programSubject.SubjectName;
            FolderName = programSubject.ParentNodeName;
            StudyState = programSubject.StudyState;
            NodeType = programSubject.GetNodeType();
        }

        /// <summary>
        /// Kiểm tra xem ProgramSubjectModel này có sẵn trong học kỳ này hay không.
        /// </summary>
        /// <returns></returns>
        private async Task IsAvaiableInThisSemester()
        {
            string[] subjectCodeSlices = ProgramSubject.SubjectCode.Split(new char[] { VMConstants.CHAR_SPACE });
            string discipline = subjectCodeSlices[0];
            string keyword1 = subjectCodeSlices[1];
            int count = await _unitOfWork.Keywords.CountAsync(discipline, keyword1);
            IsAvaiable = count > 0;
        }

        private async Task<ProgramSubjectModel> InitializeAsync()
        {
            Color = await _colorGenerator.GetColorAsync(int.Parse(ProgramSubject.CourseId));
            await IsAvaiableInThisSemester();
            return this;
        }

        public static Task<ProgramSubjectModel> CreateAsync(ProgramSubject programSubject,
            ColorGenerator colorGenerator, IUnitOfWork unitOfWork)
        {
            ProgramSubjectModel ret = new(programSubject, colorGenerator, unitOfWork);
            return ret.InitializeAsync();
        }
    }
}
