using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes;

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

        public ClassTeacher[] ClassTeachers { get; set; }
        public List<string> TempTeachers { get; set; }
        public List<ClassGroupModel> ClassGroupModels { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public string CourseId { get; set; }
        public string StudyUnitType { get; set; }
        public string StudyType { get; set; }
        public string Semester { get; set; }
        public string Desciption { get; set; }

        private string _prerequisiteSubjects;
        public string PrerequisiteSubjects
        {
            get { return _prerequisiteSubjects; }
            set { SetProperty(ref _prerequisiteSubjects, value); }
        }

        private string _parallelSubjects;
        public string ParallelSubjects
        {
            get { return _parallelSubjects; }
            set { SetProperty(ref _parallelSubjects, value); }
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
            ClassTeacher[] classTeachers, 
            string color)
        {
            Color = color;
            Subject = subject;
            SubjectName = subject.Name;
            SubjectCode = subject.SubjectCode;
            CourseId = subject.CourseId;
            StudyUnit = subject.StudyUnit;
            PrerequisiteSubjects = GetMustStudySubjects();
            ParallelSubjects = GetParallelSubjects();
            IsSpecialSubject = Subject.IsSpecialSubject;
            ClassGroupModels = Subject.ClassGroups.Select(item => new ClassGroupModel(item, IsSpecialSubject, Color)).ToList();
            ClassTeachers = classTeachers;
            TempTeachers = subject.TempTeachers;
            StudyUnitType = subject.StudyUnitType;
            StudyType = subject.StudyType;
            Semester = subject.Semester;
            Desciption = subject.Description;
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

            IsDownloading = false;
            IsError = false;
            ErrorMessage = null;

            Subject = subjectModel.Subject;
            StudyUnit = subjectModel.Subject.StudyUnit;
            IsSpecialSubject = subjectModel.IsSpecialSubject;
            ClassGroupModels = subjectModel.ClassGroupModels;
            ClassTeachers = subjectModel.ClassTeachers;
            StudyUnit = subjectModel.StudyUnit;
            PrerequisiteSubjects = subjectModel.PrerequisiteSubjects;
            ParallelSubjects = subjectModel.ParallelSubjects;
            TempTeachers = subjectModel.TempTeachers;
            StudyUnitType = subjectModel.StudyUnitType;
            StudyType = subjectModel.StudyType;
            Semester = subjectModel.Semester;
            Desciption = subjectModel.Desciption;
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
