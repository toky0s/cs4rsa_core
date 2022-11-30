using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Dialogs.Implements
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
            Messenger.Send(new SubjectImporterVmMsgs.ExitImportSubjectMsg(tup));
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
