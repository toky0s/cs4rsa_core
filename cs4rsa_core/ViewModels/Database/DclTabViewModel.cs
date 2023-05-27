using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Messages.Publishers;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cs4rsa.ViewModels.Database
{
    public partial class DclTabViewModel : ViewModelBase
    {
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<Keyword> Keywords { get; set; }

        [ObservableProperty]
        private Discipline _sltDiscipline;

        private readonly IUnitOfWork _unitOfWork;
        public DclTabViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Disciplines = new();
            Keywords = new();

            Messenger.Register<DbVmMsgs.RefreshMsg>(this, (r, m) =>
            {
                LoadInf();
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
            if (value == null) return;
            IEnumerable<Keyword> keywords = _unitOfWork.Keywords.GetKeywordsByDisciplineId(value.DisciplineId);
            Keywords.Clear();
            foreach (Keyword kw in keywords)
            {
                Keywords.Add(kw);
            }
        }

        private void LoadDisciplines()
        {
            Disciplines.Clear();
            IEnumerable<Discipline> disciplines = _unitOfWork.Disciplines.GetAllDiscipline();
            foreach (Discipline dcl in disciplines)
            {
                Disciplines.Add(dcl);
            }

            if (!Disciplines.Any()) return;
            SltDiscipline = Disciplines.First();
        }
    }
}
