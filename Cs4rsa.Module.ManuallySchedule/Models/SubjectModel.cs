using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.TeacherCrawler.Models;

using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Module.ManuallySchedule.Models
{
    public partial class SubjectModel : BindableBase
    {
        private Subject _subject;
        public Subject Subject
        {
            get { return _subject; }
            set { SetProperty(ref _subject, value); }
        }

        public readonly UserSubject UserSubject;

        public TeacherModel[] _teachers;
        public TeacherModel[] Teachers { get => _teachers; }
        public List<string> TempTeachers => Subject.TempTeachers;
        public List<ClassGroupModel> ClassGroupModels { get; private set; }
        public string SubjectName { get; private set; }
        public string SubjectCode { get; private set; }
        public string CourseId { get; private set; }
        public string StudyUnitType => Subject.StudyUnitType;
        public string StudyType => Subject.StudyType;
        public string Semester => Subject.Semester;
        public string Desciption => Subject.Description;

        private string _prerequisiteSubjectAsString;
        public string PrerequisiteSubjectAsString
        {
            get { return _prerequisiteSubjectAsString; }
            set { SetProperty(ref _prerequisiteSubjectAsString, value); }
        }

        private string _parallelSubjectAsString;
        public string ParallelSubjectAsString
        {
            get { return _parallelSubjectAsString; }
            set { SetProperty(ref _parallelSubjectAsString, value); }
        }

        private bool _isDownloading;
        public bool IsDownloading
        {
            get { return _isDownloading; }
            set { SetProperty(ref _isDownloading, value); }
        }

        private int _studyUnit;
        public int StudyUnit
        {
            get { return _studyUnit; }
            set { SetProperty(ref _studyUnit, value); }
        }

        private bool _isSpecialSubject;
        public bool IsSpecialSubject
        {
            get { return _isSpecialSubject; }
            set { SetProperty(ref _isSpecialSubject, value); }
        }

        private bool _isError;
        public bool IsError
        {
            get { return _isError; }
            set { SetProperty(ref _isError, value); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(ref _errorMessage, value); }
        }

        private string _color;
        public string Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        public SubjectModel() { }

        public SubjectModel(
            Subject subject, 
            TeacherModel[] teacherModels, 
            string color)
        {
            Color = color;
            Subject = subject;
            SubjectName = subject.Name;
            SubjectCode = subject.SubjectCode;
            CourseId = subject.CourseId;
            StudyUnit = subject.StudyUnit;
            PrerequisiteSubjectAsString = GetMustStudySubjects();
            ParallelSubjectAsString = GetParallelSubjects();
            IsSpecialSubject = Subject.IsSpecialSubject();
            ClassGroupModels = Subject.ClassGroups
                .Select(item => new ClassGroupModel(item, IsSpecialSubject, Color))
                .ToList();

            _teachers = teacherModels;
        }

        /// <summary>
        /// Tạo Pseudo Subject.
        /// </summary>
        /// <param name="teacherModels">Danh sách giảng viên</param>
        /// <param name="subjectName">Tên môn học</param>
        /// <param name="subjectCode">Mã môn</param>
        /// <param name="color">Màu sắc</param>
        /// <param name="courseId">Course ID</param>
        /// <returns></returns>
        public SubjectModel(
            string subjectName,
            string subjectCode,
            string color,
            string courseId)
        {
            SubjectName = subjectName;
            SubjectCode = subjectCode;
            CourseId = courseId;
            Color = color;
            IsDownloading = true;
            IsError = false;
        }

        /// <summary>
        /// Tạo Pseudo Subject với UserSubject được import bởi người dùng.
        /// </summary>
        /// <param name="subjectName">Tên môn học</param>
        /// <param name="subjectCode">Mã môn</param>
        /// <param name="color">Màu sắc</param>
        /// <param name="courseId">Course ID</param>
        /// <param name="userSubject">UserSubject</param>
        public SubjectModel(
            string subjectName,
            string subjectCode,
            string color,
            string courseId,
            UserSubject userSubject)
        {
            SubjectName = subjectName;
            SubjectCode = subjectCode;
            CourseId = courseId;
            Color = color;
            IsDownloading = true;
            IsError = false;
            if (!userSubject.SubjectCode.Equals(SubjectCode))
            {
                throw new Exception("Cannot set UserSubject which different subject code.");
            }
            else
            {
                UserSubject = userSubject;
            }
        }

        public void AssignData(SubjectModel subjectModel)
        {
            if (!IsDownloading)
            {
                throw new Exception("Can not assign SubjectModel because this instance is not a pseudo subject model.");
            }
            Subject = subjectModel.Subject;
            StudyUnit = subjectModel.Subject.StudyUnit;
            IsSpecialSubject = subjectModel.Subject.IsSpecialSubject();
            ClassGroupModels = subjectModel.Subject.ClassGroups
                .Select(item => new ClassGroupModel(item, IsSpecialSubject, Color))
                .ToList();

            IsDownloading = false;
            IsError = false;
            ErrorMessage = null;
        }

        public string GetMustStudySubjects()
        {
            if (Subject.MustStudySubject.Any())
            {
                return string.Join(", ", Subject.MustStudySubject);
            }
            return "Không có môn tiên quyết";
        }

        public string GetParallelSubjects()
        {
            if (Subject.ParallelSubject.Any())
            {
                return string.Join(", ", Subject.ParallelSubject);
            }
            return "Không có môn song hành";
        }
    }
}
