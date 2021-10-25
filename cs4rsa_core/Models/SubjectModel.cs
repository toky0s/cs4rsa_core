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
        private readonly Subject subject;

        public List<Teacher> Teachers => subject.Teachers;
        public List<string> TempTeachers => subject.TempTeachers;
        public List<ClassGroupModel> ClassGroupModels { get; set; }

        public string SubjectName => subject.Name;
        public string SubjectCode => subject.SubjectCode;
        public int StudyUnit { get; set; }
        public int CourseId => subject.CourseId;
        public string Color { get; set; }
        private ColorGenerator _colorGenerator;
        private SubjectModel(Subject subject, ColorGenerator colorGenerator)
        {
            _colorGenerator = colorGenerator;
            this.subject = subject;
            StudyUnit = subject.StudyUnit;
            ClassGroupModels = subject.ClassGroups.Select(item => new ClassGroupModel(item, colorGenerator)).ToList();
        }

        private async Task<SubjectModel> InitializeAsync()
        {
            Color = await _colorGenerator.GetColorAsync(subject.CourseId);
            return this;
        }

        public static Task<SubjectModel> CreateAsync(Subject subject, ColorGenerator colorGenerator)
        {
            SubjectModel ret = new(subject, colorGenerator);
            return ret.InitializeAsync();
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
