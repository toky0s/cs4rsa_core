using Cs4rsa.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cs4rsa.BaseClasses
{
    /// <summary>
    /// Interaction logic for CredizDialog.xaml
    /// </summary>
    public partial class CredizDialog : UserControl, IDialog
    {
        public CredizDialog()
        {
            InitializeComponent();
            KeyDown += CredizUserControl_KeyDown;
        }

        /// <summary>
        /// True: Có thể Click ra ngoài để đóng Dialog
        /// False: Chặn việc Click ra ngoài để đóng Dialog
        /// </summary>
        public bool IsCloseOnClickAway
        {
            get { return (bool)GetValue(IsCloseOnClickAwayProperty); }
            set { SetValue(IsCloseOnClickAwayProperty, value); }
        }

        public static readonly DependencyProperty IsCloseOnClickAwayProperty = DependencyProperty
            .Register(
                "IsCloseOnClickAway",
                typeof(bool),
                typeof(CredizDialog),
                new PropertyMetadata(false)
            );


        /// <summary>
        /// Tiêu đề hộp thoại
        /// </summary>
        public string DialogTitle
        {
            get { return (string)GetValue(DialogTitleProperty); }
            set { SetValue(DialogTitleProperty, value); }
        }

        public static readonly DependencyProperty DialogTitleProperty =
            DependencyProperty.Register("DialogTitle", typeof(string), typeof(CredizDialog), new PropertyMetadata(""));



        private void CredizUserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (IsCloseOnClickAway)
                    {
                        (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseModal();
                    }
                    break;

                default:
                    break;
            }
        }

        bool IDialog.IsCloseOnClickAway()
        {
            return IsCloseOnClickAway;
        }
    }
}
