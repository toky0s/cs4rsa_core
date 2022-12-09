using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.Controls;
using Cs4rsa.Messages.Publishers.UIs;
using Cs4rsa.Models;

using System.Windows.Controls;

namespace Cs4rsa.Views
{
    public partial class Scheduler : UserControl
    {
        public Scheduler()
        {
            InitializeComponent();
        }

        private void ScheduleBlock_ScheduleBlockClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ScheduleBlock scheduleBlock = (ScheduleBlock)sender;
            TimeBlock timeBlock = (TimeBlock)scheduleBlock.DataContext;
            StrongReferenceMessenger.Default.Send(new ScheduleBlockMsgs.SelectedMsg(timeBlock));
        }
    }
}