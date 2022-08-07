using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using HelperService;
using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.Models;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa_core.Dialogs.Implements
{
    public class AutoSortSubjectLoadViewModel : ViewModelBase
    {
        public IEnumerable<ProgramSubjectModel> ProgramSubjectModels { get; set; }
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly ColorGenerator _colorGenerator;
        public AutoSortSubjectLoadViewModel(ISubjectCrawler subjectCrawler, ColorGenerator colorGenerator)
        {
            _subjectCrawler = subjectCrawler;
            _colorGenerator = colorGenerator;
        }
        public async IAsyncEnumerable<SubjectModel> Download()
        {
            IEnumerable<string> courseIds = ProgramSubjectModels.Select(item => item.CourseId);
            foreach (string courseId in courseIds)
            {
                Subject subject = await _subjectCrawler.Crawl(ushort.Parse(courseId));
                yield return await SubjectModel.CreateAsync(subject, _colorGenerator);
            }
        }
    }
}
