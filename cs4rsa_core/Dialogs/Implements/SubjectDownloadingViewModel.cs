using cs4rsa_core.BaseClasses;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace cs4rsa_core.Dialogs.Implements
{
    class SubjectDownloadingViewModel : ViewModelBase
    {
        private string _subjectName;

        public string SubjectName
        {
            get { return _subjectName; }
            set { _subjectName = value; OnPropertyChanged(); }
        }

        private string _subjectCode;

        public string SubjectCode
        {
            get { return _subjectCode; }
            set { _subjectCode = value; OnPropertyChanged(); }
        }

        private readonly IUnitOfWork _unitOfWork;

        public SubjectDownloadingViewModel(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ReEvaluated(int courseId)
        {
            SubjectCode = await _unitOfWork.Keywords.GetSubjectCode(courseId);
            Keyword kw = await _unitOfWork.Keywords.GetKeyword(courseId);
            SubjectName = kw.SubjectName;
        }
    }
}
