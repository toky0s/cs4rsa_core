using System;
using System.Windows.Controls;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Views
{
    /// <summary>
    /// Interaction logic for YesNoUc
    /// </summary>
    public partial class YesNoUc : UserControl
    {
        public Action LeftAction { get; set; }
        public Action RightAction { get; set; }

        public YesNoUc(string leftBtnContent="THỰC HIỆN", string rightBtnContent="THOÁT")
        {
            InitializeComponent();
            Caption.Text = "Xác nhận hành động?";
            Description.Text = "Mô tả hành động";
            Cancel.Content = rightBtnContent;
            Do.Content = leftBtnContent;
        }

        public YesNoUc(string caption, string leftBtnContent = "THỰC HIỆN", string rightBtnContent = "THOÁT")
        {
            InitializeComponent();
            Caption.Text = caption ?? "Xác nhận hành động?";
            Description.Visibility = System.Windows.Visibility.Collapsed;
            Cancel.Content = rightBtnContent;
            Do.Content = leftBtnContent;
        }

        public YesNoUc(string caption, string description, string leftBtnContent = "THỰC HIỆN", string rightBtnContent = "THOÁT")
        {
            InitializeComponent();
            Caption.Text = caption ?? "Xác nhận hành động?";
            Description.Text = description ?? "Mô tả hành động";
            Cancel.Content = rightBtnContent;
            Do.Content = leftBtnContent;
        }

        private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RightAction?.Invoke();
        }

        private void Do_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LeftAction?.Invoke();
        }
    }
}
