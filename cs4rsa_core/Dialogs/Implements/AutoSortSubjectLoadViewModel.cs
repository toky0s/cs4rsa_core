using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using cs4rsa_core.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;
using cs4rsa_core.Utils;

using System.Collections.Generic;
using System.Linq;

namespace cs4rsa_core.Dialogs.Implements
{
    public class AutoSortSubjectLoadViewModel : ViewModelBase
    {
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly ColorGenerator _colorGenerator;
        public AutoSortSubjectLoadViewModel(ISubjectCrawler subjectCrawler, ColorGenerator colorGenerator)
        {
            _subjectCrawler = subjectCrawler;
            _colorGenerator = colorGenerator;
        }
        public async IAsyncEnumerable<SubjectModel> Download(IEnumerable<ProgramSubjectModel> programSubjectModels)
        {
            IEnumerable<string> courseIds = programSubjectModels.Select(item => item.CourseId);
            foreach (string courseId in courseIds)
            {
                Subject subject = await _subjectCrawler.Crawl(int.Parse(courseId), true);
                yield return await SubjectModel.CreateAsync(subject, _colorGenerator);
            }
        }
    }
}
