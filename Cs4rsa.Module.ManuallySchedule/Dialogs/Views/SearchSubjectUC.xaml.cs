using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.UI.Helper;

using Prism.Regions.Behaviors;

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

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Views
{
    /// <summary>
    /// Interaction logic for SearchSubjectUC.xaml
    /// </summary>
    public partial class SearchSubjectUC : UserControl
    {
        private Debouncer _debouncer;
        public SearchSubjectUC()
        {
            InitializeComponent();

            #region UI Config
            TextBox_SearchBox.Focus();

            #endregion

            #region Debounce Config
            const int DELAY = 300; // Delay in milliseconds
            _debouncer = new Debouncer(DELAY, () =>
            {
                var searchText = TextBox_SearchBox.Text;
                var viewModel = DataContext as ViewModels.SearchSubjectViewModel;
                viewModel.SearchCommand.Execute(searchText);
            });
            #endregion
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _debouncer.Debounce();
        }

        private void KeyBinding_Changed(object sender, EventArgs e)
        {
            Console.WriteLine("here");
        }

        private void TextBox_SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                var vm = (SearchSubjectViewModel)DataContext;
                if (e.Key == Key.Down)
                {
                    if (vm.KeyDownCommand.CanExecute())
                    {
                        vm.KeyDownCommand.Execute();
                        e.Handled = true;
                    }
                }
                else if (e.Key == Key.Up)
                {
                    if (vm.KeyUpCommand.CanExecute())
                    {
                        vm.KeyUpCommand.Execute();
                        e.Handled = true;
                    }
                }
                else
                {

                }
            }
        }
    }
}
