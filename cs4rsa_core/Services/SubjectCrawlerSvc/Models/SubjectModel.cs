using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.TeacherCrawlerSvc.Models;
using cs4rsa_core.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.Models
{
    public class SubjectModel
    {
        private readonly Subject _subject;

        public List<TeacherModel> Teachers => _subject.Teachers;
        public List<string> TempTeachers => _subject.TempTeachers;
        public List<ClassGroupModel> ClassGroupModels { get; set; }
        public string SubjectName => _subject.Name;
        public string SubjectCode => _subject.SubjectCode;
        public int StudyUnit { get; set; }
        public int CourseId => _subject.CourseId;
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
            StudyUnit = subject.StudyUnit;
        }

        public static Task<SubjectModel> CreateAsync(Subject subject, ColorGenerator colorGenerator)
        {
            SubjectModel subjectModel = new(subject, colorGenerator);
            return subjectModel.InitializeAsync();
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
            if (_subject.MustStudySubject.Count() == 0)
            {
                return "Không có môn tiên quyết";
            }
            return string.Join(", ", _subject.MustStudySubject);
        }

        public string GetParallelSubjectsAsString()
        {
            if (_subject.ParallelSubject.Count() == 0)
            {
                return "Không có môn song hành";
            }
            return string.Join(", ", _subject.ParallelSubject);
        }
    }
}
