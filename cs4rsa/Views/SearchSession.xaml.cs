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
using cs4rsa.ViewModels;
using System.ComponentModel;
using cs4rsa.Dialogs.MessageBoxService;

namespace cs4rsa.Views
{
    /// <summary>
    /// Interaction logic for SearchSession.xaml
    /// </summary>
    public partial class SearchSession : UserControl
    {
        private SearchViewModel searchViewModel = new SearchViewModel();
        
        public SearchSession()
        {
            InitializeComponent();
            Cs4rsaMessageBox cs4RsaMessageBox = new Cs4rsaMessageBox();
            searchViewModel.MessageBox = cs4RsaMessageBox;
            DataContext = searchViewModel;
        }

        private void DisciplineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchViewModel.LoadDisciplineKeyword(DisciplineComboBox.SelectedValue.ToString());
        }
    }
}
