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
using cs4rsa.Helpers;
using cs4rsa.Messages;
using cs4rsa.Models;
using cs4rsa.ViewModels;
using LightMessageBus;
using LightMessageBus.Interfaces;

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
            ClassGroupModel classGroupModel = obj as ClassGroupModel;
            return CheckDayOfWeek(classGroupModel) && 
                CheckSession(classGroupModel) &&
                CheckTeacher(classGroupModel) &&
                CheckPhase(classGroupModel) &&
                CheckPlace(classGroupModel);
        }

        private bool CheckTeacher(ClassGroupModel classGroupModel)
        {
            TeacherModel currentTeacher = (TeacherModel)ComboxBox_Teachers.SelectedItem;
            if (currentTeacher == null || currentTeacher.Id == "0")
                return true;
            return classGroupModel.GetTeacherModels().Contains(currentTeacher);
        }

        private bool CheckDayOfWeek(ClassGroupModel classGroupModel)
        {
            //DateCheck
            bool datesCheck = CheckBox_Monday.IsChecked == false &&
                CheckBox_Tuseday.IsChecked == false &&
                CheckBox_Wednessday.IsChecked == false &&
                CheckBox_Thursday.IsChecked == false &&
                CheckBox_Friday.IsChecked == false &&
                CheckBox_Saturday.IsChecked == false &&
                CheckBox_Sunday.IsChecked == false;
            if (datesCheck)
            {
                return true;
            }

            Dictionary<CheckBox, DayOfWeek> checkedDates = new Dictionary<CheckBox, DayOfWeek>
            {
                { CheckBox_Monday, DayOfWeek.Monday },
                { CheckBox_Tuseday, DayOfWeek.Tuesday },
                { CheckBox_Wednessday, DayOfWeek.Wednesday },
                { CheckBox_Thursday, DayOfWeek.Thursday },
                { CheckBox_Friday, DayOfWeek.Friday },
                { CheckBox_Saturday, DayOfWeek.Saturday },
                { CheckBox_Sunday, DayOfWeek.Sunday }
            };

            checkedDates = checkedDates.Where(pair => (bool)pair.Key.IsChecked)
                .ToDictionary(p => p.Key, p => p.Value);

            foreach (DayOfWeek day in checkedDates.Values)
            {
                if (!classGroupModel.Schedule.GetSchoolDays().Contains(day))
                    return false;
            }
            return true;
        }

        private bool CheckSession(ClassGroupModel classGroupModel)
        {
            bool sessionCheck = CheckBox_Afternoon.IsChecked == false &&
                CheckBox_Morning.IsChecked == false &&
                CheckBox_Night.IsChecked == false;
            if (sessionCheck)
                return true;

            Dictionary<CheckBox, Session> checkSessions = new Dictionary<CheckBox, Session>
            {
                { CheckBox_Morning, Session.MORNING },
                { CheckBox_Afternoon, Session.AFTERNOON },
                { CheckBox_Night, Session.NIGHT }
            };

            checkSessions = checkSessions.Where(pair => pair.Key.IsChecked.Value)
                .ToDictionary(p => p.Key, p => p.Value);

            foreach (Session session in checkSessions.Values)
            {
                if (!classGroupModel.Schedule.GetSessions().Contains(session))
                    return false;
            }
            return true;
        }

        private bool CheckPhase(ClassGroupModel classGroupModel)
        {
            RadioButton checkedRadioButton = RadioButtonContainer_Phase.Children
                .OfType<RadioButton>()
                .FirstOrDefault(r => r.IsChecked.Value);
            string value = checkedRadioButton.Name;
            if (value == "all") return true;
            Phase phase = BasicDataConverter.ToPhase(value);
            return classGroupModel.Phase == phase;
        }

        private bool CheckPlace(ClassGroupModel classGroupModel)
        {
            List<CheckBox> checkedPlaces = CheckBoxContainer_Place.Children.OfType<CheckBox>()
                .Where(c => c.IsChecked.Value)
                .ToList();
            if (checkedPlaces.Count == 0) return true;
            Dictionary<CheckBox, Place> checkboxAndPlace = new Dictionary<CheckBox, Place>()
            {
                {CheckBox_quantrung, Place.QUANGTRUNG },
                {CheckBox_hoakhanh, Place.HOAKHANH },
                {CheckBox_phanthanh, Place.PHANTHANH },
                {CheckBox_viettin, Place.VIETTIN },
                {CheckBox_137_nvl, Place.NVL_137 }
            };
            checkboxAndPlace = checkboxAndPlace.Where(pair => pair.Key.IsChecked.Value)
                .ToDictionary(p => p.Key, p => p.Value);
            foreach (Place place in checkboxAndPlace.Values)
            {
                if (!classGroupModel.Places.Contains(place))
                    return false;
            }
            return true;
        }

        private void ReloadClassGroupCollection(object sender, RoutedEventArgs e)
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