using CommunityToolkit.Mvvm.Messaging;

using cs4rsa_core.BaseClasses;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Messages.Publishers.Dialogs;
using cs4rsa_core.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;
using cs4rsa_core.Utils;

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
    public class SubjectImporterViewModel : ViewModelBase
    {
        public ObservableCollection<UserSubject> SubjectInfoDatas { get; set; }
        public bool IsUseCache { get; set; }

        private readonly IKeywordRepository _keywordRepository;
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly ColorGenerator _colorGenerator;

        public SubjectImporterViewModel(
            IKeywordRepository keywordRepository,
            ISubjectCrawler subjectCrawler,
            ColorGenerator colorGenerator)
        {
            _keywordRepository = keywordRepository;
            _subjectCrawler = subjectCrawler;
            _colorGenerator = colorGenerator;

            SubjectInfoDatas = new();
            IsUseCache = true;
        }

        public async Task Run(IEnumerable<UserSubject> userSubjects)
        {
            foreach (UserSubject item in userSubjects)
            {
                SubjectInfoDatas.Add(item);
            }

            IEnumerable<int> courseIds = userSubjects
                                        .Select(item => _keywordRepository.GetCourseId(item.SubjectCode));
            IEnumerable<Task<SubjectModel>> subjectTasks = courseIds.Select(courseId => ToSubjectModel(courseId));
            SubjectModel[] subjectModels = await Task.WhenAll(subjectTasks);

            Tuple<IEnumerable<SubjectModel>, IEnumerable<UserSubject>> tup = new(subjectModels, SubjectInfoDatas);
            SubjectImporterVmMsgs.ExitImportSubjectMsg message = new(tup);
            Messenger.Send(message);

        }

        private async Task<SubjectModel> ToSubjectModel(int courseId)
        {
            Subject subject = await _subjectCrawler.Crawl(courseId, IsUseCache);
            await subject.GetClassGroups();
            SubjectModel subjectModel = await SubjectModel.CreateAsync(subject, _colorGenerator);
            subjectModel.Color = await _colorGenerator.GetColorAsync(subject.CourseId);
            return subjectModel;
        }
    }
}
