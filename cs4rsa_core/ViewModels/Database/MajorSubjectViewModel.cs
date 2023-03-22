using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Models.Database;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cs4rsa.ViewModels.Database
{
    public partial class MajorSubjectViewModel : ViewModelBase
    {
        public ObservableCollection<MajorSubjectModel> MajorSubjectModels { get; set; }

        [ObservableProperty]
        public MajorSubjectModel _currentMajorSubject;

        private readonly IUnitOfWork _unitOfWork;
        public MajorSubjectViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            MajorSubjectModels = new();
        }

        partial void OnCurrentMajorSubjectChanged(MajorSubjectModel value)
        {
            Messenger.Send(new DbVmMsgs.SelectMajorMsg(value));
        }

        public void LoadPlanJsonFile()
        {
            MajorSubjectModels.Clear();
            List<Curriculum> curriculums = _unitOfWork.Curriculums.GetAllCurr();
            foreach (Curriculum curr in curriculums)
            {
                MajorSubjectModel mj = new()
                {
                    Curriculum = curr,
                    TotalSubject = _unitOfWork.Curriculums.GetCountMajorSubjectByCurrId(curr.CurriculumId)
                };
                MajorSubjectModels.Add(mj);
            }
        }
    }
}
