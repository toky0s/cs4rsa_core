using System;
using System.Collections.Generic;
using System.Globalization;
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
using cs4rsa.BasicData;
using cs4rsa.Models;
using cs4rsa.ViewModels;

namespace cs4rsa.Views
{
    /// <summary>
    /// Interaction logic for ClassGroupSession.xaml
    /// </summary>
    public partial class ClassGroupSession : UserControl
    {
        private ClassGroupViewModel classGroupViewModel = new ClassGroupViewModel();
        public ClassGroupSession()
        {
            InitializeComponent();
            DataContext = classGroupViewModel;

            CollectionView view = (CollectionView) CollectionViewSource.GetDefaultView(classGroupViewModel.ClassGroupModels);
            view.Filter = ClassGroupFilter;
        }

        private bool ClassGroupFilter(object obj)
        {
            if (Monday.IsChecked == false)
                return true;
            return (obj as ClassGroupModel).Schedule.GetSchoolDays().Contains(DayOfWeek.Monday);
        }

        private void Monday_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(classGroupViewModel.ClassGroupModels).Refresh();
        }
    }
}

namespace cs4rsa.Converters
{
    public class PhaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Phase phase = (Phase)value;
            switch (phase)
            {
                case Phase.FIRST:
                    return "Giai đoạn 1";
                case Phase.SECOND:
                    return "Giai đoạn 2";
                case Phase.NON:
                    break;
                case Phase.ALL:
                    return "Hai giai đoạn";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}