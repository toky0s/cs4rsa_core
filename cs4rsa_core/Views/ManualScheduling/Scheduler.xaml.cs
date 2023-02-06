using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Controls;
using Cs4rsa.Messages.Publishers.UIs;
using Cs4rsa.Models;

namespace Cs4rsa.Views.ManualScheduling
{
    public partial class Scheduler : BaseUserControl
    {
        public Scheduler(): base()
        {
            InitializeComponent();
        }

        private void ScheduleBlock_ScheduleBlockClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ScheduleBlock scheduleBlock = (ScheduleBlock)sender;
            TimeBlock timeBlock = (TimeBlock)scheduleBlock.DataContext;
            Messenger.Send(new ScheduleBlockMsgs.SelectedMsg(timeBlock));
        }
    }
}