using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.MessageBoxService;
using System;
using System.ComponentModel;
using System.Windows;
using LightMessageBus;
using cs4rsa_core.Messages;
using cs4rsa_core.Dialogs.DialogServices;
using StudentCrawlerService.Crawlers;
using Cs4rsaDatabaseService.Models;
using StudentCrawlerService.Interfaces;
using System.Threading.Tasks;
using CurriculumCrawlerService.Crawlers;
using Cs4rsaDatabaseService.Interfaces;

namespace cs4rsa_core.Dialogs.Implements
{
    /// <summary>
    /// ViewModel của dialog nhập session id
    /// view model này là sử dụng chính DtuStudentInfoCrawler để cào thông tin sinh viên.
    /// Ngoài ra không có bất cứ viewmodel nào được sử dụng crawler này.
    /// </summary>
    class SessionInputViewModel : DialogViewModelBase<StudentResult>
    {
        private string _sessionId;
        public string SessionId
        {
            get => _sessionId;
            set
            {
                _sessionId = value;
                OnPropertyChanged();
            }
        }

        public IMessageBox MessageBox { get; set; }

        public Action<StudentResult> CloseDialogCallback { get; set; }
        private readonly IDtuStudentInfoCrawler _dtuStudentInfoCrawler;
        private readonly CurriculumCrawler _curriculumCrawler;
        private readonly IUnitOfWork _unitOfWork;
        public SessionInputViewModel(IDtuStudentInfoCrawler dtuStudentInfoCrawler, CurriculumCrawler curriculumCrawler,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dtuStudentInfoCrawler = dtuStudentInfoCrawler;
            _curriculumCrawler = curriculumCrawler;
        }

        public async Task Find()
        {
            string specialString = await SpecialStringCrawler.GetSpecialString(_sessionId);
            if (specialString == null)
            {
                // check special String hop le
                // THOAT
                string message = "Hãy chắc chắn bạn đã đăng nhập vào MyDTU trước khi lấy Session ID, " +
                    "và đảm bảo lúc này server DTU không bảo trì. Hãy thử lại sau.";
                MessageBoxResult messageBoxResult = MessageBox.ShowMessage(message,
                                        "Thông báo",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation);
                CloseDialogCallback.Invoke(null);
            }
            else
            {
                Curriculum curriculum = CurriculumCrawler.GetCurriculum(specialString);

                Student student = _dtuStudentInfoCrawler.Crawl(specialString);
                _unitOfWork.Students.Add(student);
                _unitOfWork.Curriculums.Add(curriculum);
                _unitOfWork.Complete();

                string message = $"Xin chào {student.Name}";
                MessageBus.Default.Publish<Cs4rsaSnackbarMessage>(new Cs4rsaSnackbarMessage(message));
                StudentResult result = new StudentResult { Student = student };
                CloseDialogCallback.Invoke(result);
            }
        }
    }
}
