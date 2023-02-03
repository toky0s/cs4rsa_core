using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Utils.Interfaces;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.ViewModels.Database
{
    internal partial class DclTabViewModel : ViewModelBase
    {
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<Keyword> Keywords { get; set; }

        [ObservableProperty]
        private Discipline _sltDiscipline;

        public AsyncRelayCommand<int> ViewCacheCommand { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenInBrowser _openInBrowser;
        public DclTabViewModel(
            IUnitOfWork unitOfWork,
            IOpenInBrowser openInBrowser
        )
        {
            _unitOfWork = unitOfWork;
            _openInBrowser = openInBrowser;

            Disciplines = new();
            Keywords = new();
            ViewCacheCommand = new(OnViewCache);

            Messenger.Register<DbVmMsgs.RefreshMsg>(this, (r, m) =>
            {
                Task.Run(LoadInf);
            });

            LoadInf();
        }

        /// <summary>
        /// Khởi tạo infor màn hình.
        /// </summary>
        private void LoadInf()
        {
            LoadDisciplines();
        }

        partial void OnSltDisciplineChanged(Discipline value)
        {
            if (value != null)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Keywords.Clear();
                    foreach (Keyword kw in value.Keywords)
                    {
                        Keywords.Add(kw);
                    }
                });
            }
        }

        private void LoadDisciplines()
        {
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                Disciplines.Clear();
                IAsyncEnumerable<Discipline> disciplines = _unitOfWork.Disciplines.GetAll();
                await foreach (Discipline dcl in disciplines)
                {
                    Disciplines.Add(dcl);
                }
                if (Disciplines.Any())
                {
                    SltDiscipline = Disciplines.FirstOrDefault();
                }
            });
        }

        /// <summary>
        /// Tạo và mở file Cache trong Browser.
        /// </summary>
        /// <param name="courseId">Course ID</param>
        private async Task OnViewCache(int courseId)
        {
            string filePath = CredizText.PathHtmlCacheFile(courseId);
            if (!File.Exists(filePath))
            {
                Keyword kw = Keywords.Where(kw => kw.CourseId == courseId).First();
                await File.WriteAllTextAsync(filePath, kw.Cache);
            }
            _openInBrowser.Open(filePath);
        }
    }
}
