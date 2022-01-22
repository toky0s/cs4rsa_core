using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cs4rsaDatabaseService.Models;
using HelperService;
using SubjectCrawlService1.DataTypes;

namespace cs4rsa_core.Models
{
    public class SubjectModel
    {
        private readonly Subject _subject;

        public List<Teacher> Teachers => _subject.Teachers;
        public List<string> TempTeachers => _subject.TempTeachers;
        public List<ClassGroupModel> ClassGroupModels { get; set; }

        public string SubjectName => _subject.Name;
        public string SubjectCode => _subject.SubjectCode;
        public int StudyUnit { get; set; }
        public int CourseId => _subject.CourseId;
        public bool IsSpecialSubject { get; set; }
        public string Color { get; set; }
        private readonly ColorGenerator _colorGenerator;
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
            IsSpecialSubject = await _subject.IsSpecialSubject();
            ClassGroupModels = _subject.ClassGroups
                .Select(item => new ClassGroupModel(item, IsSpecialSubject, _colorGenerator))
                .ToList();
            return this;
        }

        /// <summary>
        /// Trả về ClassGroupModel theo tên được truyền vào, nếu không tồn tại ClassGroupModel
        /// theo tên đã truyền vào trả về null.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ClassGroupModel GetClassGroupModelWithName(string name)
        {
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                if (classGroupModel.Name.Equals(name, System.StringComparison.Ordinal))
                {
                    return classGroupModel;
                }
            }
            return null;
        }
    }
}
