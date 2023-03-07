using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.TeacherCrawlerSvc.Models;
using Cs4rsa.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Models
{
    public partial class SubjectModel : ObservableObject
    {
        [ObservableProperty]
        public Subject _subject;

        public readonly UserSubject UserSubject;
        public List<TeacherModel> Teachers => Subject.Teachers;
        public List<string> TempTeachers => Subject.TempTeachers;
        public List<ClassGroupModel> ClassGroupModels { get; private set; }
        public string SubjectName { get; private set; }
        public string SubjectCode { get; private set; }
        public int CourseId { get; private set; }
        public string StudyUnitType => Subject.StudyUnitType;
        public string StudyType => Subject.StudyType;
        public string Semester => Subject.Semester;
        public string Desciption => Subject.Desciption;

        [ObservableProperty]
        public string _prerequisiteSubjectAsString;

        [ObservableProperty]
        public string _parallelSubjectAsString;

        [ObservableProperty]
        private bool _isDownloading;

        [ObservableProperty]
        private int _studyUnit;

        [ObservableProperty]
        public bool _isSpecialSubject;

        [ObservableProperty]
        private bool _isError;

        [ObservableProperty]
        private string _errorMessage;

        public string Color { get; set; }

        #region Services
        private readonly ColorGenerator _colorGenerator;
        #endregion

        public SubjectModel() { }

        private SubjectModel(Subject subject, ColorGenerator colorGenerator)
        {
            _colorGenerator = colorGenerator;
            Subject = subject;
            SubjectName = subject.Name;
            SubjectCode = subject.SubjectCode;
            CourseId = subject.CourseId;
            StudyUnit = subject.StudyUnit;
            PrerequisiteSubjectAsString = GetMustStudySubjects();
            ParallelSubjectAsString = GetParallelSubjects();
        }

        private SubjectModel(
            string subjectName,
            string subjectCode,
            string color,
            int courseId)
        {
            SubjectName = subjectName;
            SubjectCode = subjectCode;
            CourseId = courseId;
            Color = color;
            IsDownloading = true;
            IsError = false;
        }

        private SubjectModel(
            string subjectName,
            string subjectCode,
            string color,
            int courseId,
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

        public static Task<SubjectModel> CreateAsync(Subject subject, ColorGenerator colorGenerator)
        {
            SubjectModel subjectModel = new(subject, colorGenerator);
            return subjectModel.InitializeAsync();
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

        /// <summary>
        /// Tạo Pseudo Subject.
        /// </summary>
        /// <param name="subjectName">Tên môn học</param>
        /// <param name="subjectCode">Mã môn</param>
        /// <param name="color">Màu sắc</param>
        /// <param name="courseId">Course ID</param>
        /// <returns></returns>
        public static SubjectModel CreatePseudo(
             string subjectName
            , string subjectCode
            , string color
            , int courseId)
        {
            return new SubjectModel(
                  subjectName
                , subjectCode
                , color
                , courseId
            );
        }

        /// <summary>
        /// Tạo Pseudo Subject với UserSubject được import bởi người dùng.
        /// </summary>
        /// <param name="subjectName">Tên môn học</param>
        /// <param name="subjectCode">Mã môn</param>
        /// <param name="color">Màu sắc</param>
        /// <param name="courseId">Course ID</param>
        /// <param name="userSubject">UserSubject</param>
        /// <returns></returns>
        public static SubjectModel CreatePseudo(
             string subjectName
            , string subjectCode
            , string color
            , int courseId
            , UserSubject userSubject)
        {
            return new SubjectModel(
                  subjectName
                , subjectCode
                , color
                , courseId
                , userSubject
            );
        }

        private async Task<SubjectModel> InitializeAsync()
        {
            Color = await _colorGenerator.GetColorAsync(Subject.CourseId);
            IsSpecialSubject = Subject.IsSpecialSubject();
            ClassGroupModels = Subject.ClassGroups
                .Select(item => new ClassGroupModel(item, IsSpecialSubject, Color))
                .ToList();
            return this;
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
