
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Enums;
using cs4rsa.Models.Base;

namespace cs4rsa.Models
{
    /// <summary>
    /// ProgramSubjectModel khác ProgramSubject ở chỗ chúng có các phương thức check trong cây diagram
    /// nhằm xác định việc người dùng có thể chọn subject đó hay không, điều mà ProgramSubject không thể
    /// làm được vì nó chỉ chứa thông tin của chính nó mà không có tương tác nào với các Folder hay Subject
    /// khác trong cây.
    /// </summary>
    public class ProgramSubjectModel: TreeItem
    {
        public ProgramSubject ProgramSubject { get; set; }

        private string _subjectCode;
        public string SubjectCode
        {
            get
            {
                return _subjectCode;
            }
            set
            {
                _subjectCode = value;
            }
        }

        private string _subjectName;
        public string SubjectName
        {
            get
            {
                return _subjectName;
            }
            set
            {
                _subjectName = value;
            }
        }

        private string _folderName;
        public string FolderName
        {
            get
            {
                return _folderName;
            }
            set
            {
                _folderName = value;

            }
        }

        private string _color;
        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        private int _studyUnit = 0;
        public int StudyUnit
        {
            get
            {
                return _studyUnit == 0 ? ProgramSubject.StudyUnit : _studyUnit;
            }
            set
            {
                _studyUnit = value;
            }
        }

        private StudyState _studyState;
        public StudyState StudyState
        {
            get { return _studyState; }
            set { _studyState = value; }
        }

        public bool IsDone => ProgramSubject.IsDone();
        public bool IsAvaiable => IsAvaiableInThisSemester();
        public string CourseId => ProgramSubject.CourseId;
        public string ChildOfNode => ProgramSubject.ChildOfNode;

        public ProgramSubjectModel(ProgramSubject programSubject):base(programSubject.SubjectName, programSubject.Id)
        {
            ProgramSubject = programSubject;
            _subjectCode = programSubject.SubjectCode;
            _subjectName = programSubject.SubjectName;
            _folderName = programSubject.ParrentNodeName;
            _studyState = programSubject.StudyState;
            NodeType = programSubject.GetNodeType();
            _color = ColorGenerator.GetColor(programSubject.CourseId);
        }

        /// <summary>
        /// Kiểm tra xem ProgramSubjectModel này có sẵn trong học kỳ này hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsAvaiableInThisSemester()
        {
            return Cs4rsaDataView.IsExistsSubjectInThisSemester(ProgramSubject);
        }
    }
}
