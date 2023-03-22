using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Models.Database
{
    public partial class MajorSubjectModel : ObservableObject
    {
        [ObservableProperty]
        private int _totalSubject;
        [ObservableProperty]
        private Curriculum _curriculum;
    }
}
