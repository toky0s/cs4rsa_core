using Cs4rsa.BaseClasses;
using Cs4rsa.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Dialogs.Implements
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
