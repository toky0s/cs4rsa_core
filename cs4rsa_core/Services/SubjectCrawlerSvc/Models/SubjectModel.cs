using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.TeacherCrawlerSvc.Models;
using Cs4rsa.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Models
{
    public partial class SubjectModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isDownloading;

        public Subject Subject { get; private set; }

        public List<TeacherModel> Teachers => Subject.Teachers;
        public List<string> TempTeachers => Subject.TempTeachers;
        public List<ClassGroupModel> ClassGroupModels { get; set; }
        public string SubjectName { get; private set; }
        public string SubjectCode { get; private set; }

        [ObservableProperty]
        private int _studyUnit;

        public int CourseId { get; private set; }
        public string StudyUnitType => Subject.StudyUnitType;
        public string StudyType => Subject.StudyType;
        public string Semester => Subject.Semester;
        public string Desciption => Subject.Desciption;
        public string PrerequisiteSubjectAsString => GetMustStudySubjectsAsString();
        public string ParallelSubjectAsString => GetParallelSubjectsAsString();

        public bool IsSpecialSubject { get; set; }
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
        }

        private SubjectModel(
            string subjectName,
            string subjectCode,
            int courseId,
            string color)
        {
            SubjectName = subjectName;
            SubjectCode = subjectCode;
            CourseId = courseId;
            Color = color;
            IsDownloading = true;
        }

        public static Task<SubjectModel> CreateAsync(Subject subject, ColorGenerator colorGenerator)
        {
            SubjectModel subjectModel = new(subject, colorGenerator);
            return subjectModel.InitializeAsync();
        }

        public void AssignData(SubjectModel subjectModel)
        {
            if (!_isDownloading)
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
        }

        public static SubjectModel CreatePseudo(string subjectName, string subjectCode, string color, int courseId)
        {
            Trace.WriteLine("public static SubjectModel CreatePseudo");
            return new SubjectModel(subjectName, subjectCode, courseId, color);
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

        public ClassGroupModel GetClassGroupModelWithName(string name)
        {
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                if (classGroupModel.Name.Equals(name, StringComparison.Ordinal))
                {
                    return classGroupModel;
                }
            }
            return null;
        }

        public string GetMustStudySubjectsAsString()
        {
            if (Subject.MustStudySubject.Any())
            {
                return string.Join(", ", Subject.MustStudySubject);
            }
            return "Không có môn tiên quyết";
        }

        public string GetParallelSubjectsAsString()
        {
            if (Subject.ParallelSubject.Any())
            {
                return string.Join(", ", Subject.ParallelSubject);
            }
            return "Không có môn song hành";
        }
    }
}
