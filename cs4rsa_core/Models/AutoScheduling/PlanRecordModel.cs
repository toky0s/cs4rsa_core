using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;

namespace Cs4rsa.Models.AutoScheduling
{
    public partial class PlanRecordModel : ObservableObject
    {
        [ObservableProperty]
        private string _subjectCode;

        [ObservableProperty]
        private string _subjectName;

        [ObservableProperty]
        private string _url;

        [ObservableProperty]
        private int _studyUnit;

        /// <summary>
        /// Kiểm tra tồn tại trong học kỳ này
        /// </summary>
        [ObservableProperty]
        private bool _isAvailable;

        [ObservableProperty]
        private StudyState _studyState;

        [ObservableProperty]
        private bool _isSelected;
    }
}
