using Prism.Mvvm;

namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    public class MainSchedulingViewModel : BindableBase
    {
        private bool isSummerSemester;
        public bool IsSummerSemester
        {
            get { return isSummerSemester; }
            set { SetProperty(ref isSummerSemester, value); }
        }

        public MainSchedulingViewModel()
        {
        }
    }
}
