using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.TeacherCrawlerSvc.Models;
using Cs4rsa.Utils;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Models
{
    public class SubjectModel
    {
        private bool _isDownloading;

        public bool IsDownloading
        {
            get { return _isDownloading; }
            set { _isDownloading = value; }
        }

        private readonly Subject _subject;

        public List<TeacherModel> Teachers => _subject.Teachers;
        public List<string> TempTeachers => _subject.TempTeachers;
        public List<ClassGroupModel> ClassGroupModels { get; set; }
        public string SubjectName { get; private set; }
        public string SubjectCode { get; private set; }
        public int StudyUnit { get; set; }
        public int CourseId { get; private set; }
        public string StudyUnitType => _subject.StudyUnitType;
        public string StudyType => _subject.StudyType;
        public string Semester => _subject.Semester;
        public string Desciption => _subject.Desciption;
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
            _subject = subject;
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

        public static SubjectModel CreatePseudo(string subjectName, string subjectCode, string color, int courseId)
        {
            Trace.WriteLine("public static SubjectModel CreatePseudo");
            return new SubjectModel(subjectName, subjectCode, courseId, color);
        }

        private async Task<SubjectModel> InitializeAsync()
        {
            Color = await _colorGenerator.GetColorAsync(_subject.CourseId);
            IsSpecialSubject = _subject.IsSpecialSubject();
            ClassGroupModels = _subject.ClassGroups
                .Select(item => new ClassGroupModel(item, IsSpecialSubject, _colorGenerator))
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
            if (_subject.MustStudySubject.Any())
            {
                return string.Join(", ", _subject.MustStudySubject);
            }
            return "Không có môn tiên quyết";
        }

        public string GetParallelSubjectsAsString()
        {
            if (_subject.ParallelSubject.Any())
            {
                return string.Join(", ", _subject.ParallelSubject);
            }
            return "Không có môn song hành";
        }
    }
}
