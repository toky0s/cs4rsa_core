using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using HelperService;
using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cs4rsa_core.Dialogs.Implements
{
    public class AutoSortSubjectLoadViewModel : ViewModelBase
    {
        public List<ProgramSubjectModel> ProgramSubjectModels { get; set; } = new List<ProgramSubjectModel>();
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly ColorGenerator _colorGenerator;
        public AutoSortSubjectLoadViewModel(ISubjectCrawler subjectCrawler, ColorGenerator colorGenerator)
        {
            _subjectCrawler = subjectCrawler;
            _colorGenerator = colorGenerator;
        }
        public async Task<List<SubjectModel>> Download()
        {
            List<string> courseIds = ProgramSubjectModels.Select(item => item.CourseId).ToList();
            List<Subject> subjects = new();
            foreach (string courseId in courseIds)
            {
                Subject subject = await _subjectCrawler.Crawl(ushort.Parse(courseId));
                subjects.Add(subject);
            }
            List<SubjectModel> subjectModels = new();
            foreach (Subject item in subjects)
            {
                SubjectModel subjectModel = await SubjectModel.CreateAsync(item, _colorGenerator);
                subjectModels.Add(subjectModel);
            }
            return subjectModels;
        }
    }
}
