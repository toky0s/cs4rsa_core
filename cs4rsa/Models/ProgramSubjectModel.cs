
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
        private ProgramSubject _programSubject;

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
                return _studyUnit == 0 ? _programSubject.StudyUnit : _studyUnit;

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


        public bool IsDone => _programSubject.IsDone();
        public bool IsFolderCompleted => false; // FolderContainThisSubjectIsCompleted();
        public bool IsCanChoice => true; // CanChoice();
        public string CourseId => _programSubject.CourseId;
        public string ChildOfNode => _programSubject.ChildOfNode;

        public ProgramSubjectModel(ProgramSubject programSubject):base(programSubject.SubjectName, programSubject.Id)
        {
            _programSubject = programSubject;
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
        public bool IsAvaiableInThisSemester()
        {
            return Cs4rsaDataView.IsExistsSubjectInThisSemester(_programSubject);
        }

        /// <summary>
        /// Kiểm tra xem folder chứa Subject này đã hoàn thành hay chưa.
        /// </summary>
        /// <returns></returns>
        //public bool FolderContainThisSubjectIsCompleted()
        //{
        //    ProgramFolder folder = _diagram.GetFolder(_folderName);
        //    return folder.IsCompleted();
        //}


        /// <summary>
        /// Kiểm tra xem tất cả các môn tiên quyết của môn này đã hoàn thành hay chưa.
        /// </summary>
        /// <returns></returns>
        //public bool IsCompletedPreSubject()
        //{
        //    bool flag = true;
        //    foreach (string subjectCode in _programSubject.PrerequisiteSubjects)
        //    {
        //        ProgramSubject subject = _diagram.GetProgramSubject(subjectCode);
        //        if (subject == null)
        //        {
        //            return false;
        //        }
        //        if (!subject.IsDone())
        //            return false;
        //    }
        //    return flag;
        //}

        /// <summary>
        /// Xác định xem Subject này có thể chọn hay không.
        /// </summary>
        /// <returns></returns>
        //public bool CanChoice()
        //{
        //    return _programSubject.IsUnLearn() &&
        //        !FolderContainThisSubjectIsCompleted() &&
        //        IsCompletedPreSubject() &&
        //        Cs4rsaDataView.IsExistsSubjectInThisSemester(_programSubject);
        //}
    }
}
