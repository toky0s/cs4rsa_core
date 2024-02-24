using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Database.Models;

using System.Windows.Controls;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Views
{
    /// <summary>
    /// Interaction logic for ScheduleBag
    /// </summary>
    public partial class ScheduleBag : UserControl
    {
        private ScheduleBagViewModel _dataContext;
        public ScheduleBag()
        {
            InitializeComponent();
            _dataContext = (ScheduleBagViewModel)DataContext;
        }

        private void ScheduleBagItem_Expanded(object sender, System.Windows.RoutedEventArgs e)
        {
            ScheduleBagModel scheduleBagModel = (ScheduleBagModel)((Expander)sender).DataContext;
            _dataContext.GetScheduleDetails(scheduleBagModel);
        }
    }
}
