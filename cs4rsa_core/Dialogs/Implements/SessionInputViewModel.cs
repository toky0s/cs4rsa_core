﻿using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.MessageBoxService;
using System.Windows;
using LightMessageBus;
using cs4rsa_core.Messages;
using cs4rsa_core.Dialogs.DialogServices;
using StudentCrawlerService.Crawlers;
using Cs4rsaDatabaseService.Models;
using System.Threading.Tasks;
using cs4rsa_core.ViewModels;
using StudentCrawlerService.Crawlers.Interfaces;

namespace cs4rsa_core.Dialogs.Implements
{
    /// <summary>
    /// ViewModel của dialog nhập session id
    /// view model này là sử dụng chính DtuStudentInfoCrawler để cào thông tin sinh viên.
    /// Ngoài ra không có bất cứ viewmodel nào được sử dụng crawler này.
    /// </summary>
    public class SessionInputViewModel : DialogViewModelBase<StudentResult>
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
        private readonly IDtuStudentInfoCrawler _dtuStudentInfoCrawler;
        public SessionInputViewModel(IDtuStudentInfoCrawler dtuStudentInfoCrawler)
        {
            _dtuStudentInfoCrawler = dtuStudentInfoCrawler;
        }

        public async Task Find()
        {
            string specialString = await SpecialStringCrawler.GetSpecialString(_sessionId);
            if (specialString == null)
            {
                string message = "Hãy chắc chắn bạn đã đăng nhập vào MyDTU trước khi lấy Session ID, " +
                    "và đảm bảo lúc này server DTU không bảo trì. Hãy thử lại sau.";
                MessageBoxResult _ = MessageBox.ShowMessage(message,
                                        "Thông báo",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation);
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            }
            else
            {
                Student student = await _dtuStudentInfoCrawler.Crawl(specialString);
                string message = $"Xin chào {student.Name}";
                MessageBus.Default.Publish(new Cs4rsaSnackbarMessage(message));
                StudentResult result = new() { Student = student };
                MessageBus.Default.Publish(new ExitSessionInputMessage(result));
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            }
        }
    }
}