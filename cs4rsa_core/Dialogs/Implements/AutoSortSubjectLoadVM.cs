using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using HelperService;
using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cs4rsa_core.Dialogs.Implements
{
    public class AutoSortSubjectLoadVM : ViewModelBase
    {
        public List<ProgramSubjectModel> ProgramSubjectModels { get; set; } = new List<ProgramSubjectModel>();
        public Action<List<SubjectModel>> CloseDialogCallback { get; set; }
        public bool IsRemoveClassGroupInvalid { get; set; }
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly ColorGenerator _colorGenerator;
        public AutoSortSubjectLoadVM(ISubjectCrawler subjectCrawler, ColorGenerator colorGenerator)
        {
            _subjectCrawler = subjectCrawler;
            _colorGenerator = colorGenerator;
        }
        public async Task Download()
        {
            List<string> courseIds = ProgramSubjectModels.Select(item => item.CourseId).ToList();
            List<Subject> subjects = new List<Subject>();
            foreach (string courseId in courseIds)
            {
                Subject subject = await _subjectCrawler.Crawl(int.Parse(courseId));
                subjects.Add(subject);
            }
            List<SubjectModel> subjectModels = subjects.Select(item => new SubjectModel(item, _colorGenerator)).ToList();
            CloseDialogCallback.Invoke(subjectModels);
        }
    }
}
