using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogServices;

using Cs4rsaDatabaseService.Interfaces;

using HelperService;

using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace cs4rsa_core.Dialogs.Implements
{
    /// <summary>
    /// <strong>Trình Quản lý Phiên:</strong>
    /// Bộ ra lệnh cho SearchViewModel thực hiện import các Subject được truyền vào.
    /// </summary>
    public class SubjectImporterViewModel : DialogViewModelBase<ImportResult>
    {
        public ObservableCollection<SubjectInfoData> SubjectInfoDatas { get; set; } = new ObservableCollection<SubjectInfoData>();
        public List<ISubjectCrawler> SubjectCrawlers { get; set; }

        private SessionManagerResult _sessionManagerResult;

        public SessionManagerResult SessionManagerResult
        {
            get { return _sessionManagerResult; }
            set
            {
                _sessionManagerResult = value;
                foreach (SubjectInfoData item in _sessionManagerResult.SubjectInfoDatas)
                {
                    SubjectInfoDatas.Add(item);
                }
            }
        }

        public Action<ImportResult, SessionManagerResult> CloseDialogCallback { get; set; }

        private readonly IKeywordRepository _keywordRepository;
        private readonly ISubjectCrawler _subjectCrawler;

        public SubjectImporterViewModel(IKeywordRepository keywordRepository, ISubjectCrawler subjectCrawler)
        {
            _keywordRepository = keywordRepository;
            _subjectCrawler = subjectCrawler;
        }

        public async Task Run()
        {
            ColorGenerator colorGenerator = new(_keywordRepository);
            List<int> courseIds = _sessionManagerResult.SubjectInfoDatas
                                            .Select(item => _keywordRepository.GetCourseId(item.SubjectCode))
                                            .ToList();

            List<SubjectModel> subjectModels = new();
            foreach (ushort courseId in courseIds)
            {
                Subject subject = await _subjectCrawler.Crawl(courseId);
                await subject.GetClassGroups();
                SubjectModel subjectModel = await SubjectModel.CreateAsync(subject, colorGenerator);
                subjectModels.Add(subjectModel);
            }

            foreach (SubjectModel subject in subjectModels)
            {
                subject.Color = await colorGenerator.GetColorAsync(subject.CourseId);
            }
            ImportResult importResult = new() { SubjectModels = subjectModels };
            CloseDialogCallback.Invoke(importResult, _sessionManagerResult);
        }
    }
}
