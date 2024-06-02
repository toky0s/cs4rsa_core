using Cs4rsa.UI.ScheduleTable.Models;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.UI.ScheduleTable.CustomControls
{
    public partial class STable : UserControl
    {
        public STable()
        {
            InitializeComponent();
        }

        public ObservableCollection<string> ItemsSource
        {
            get { return (ObservableCollection<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                name: "ItemsSource",
                propertyType: typeof(ObservableCollection<string>),
                ownerType: typeof(STable),
                typeMetadata: new FrameworkPropertyMetadata(null));

        public ObservableCollection<ObservableCollection<TimeBlock>> Week
        {
            get { return (ObservableCollection<ObservableCollection<TimeBlock>>)GetValue(WeekProperty); }
            set { SetValue(WeekProperty, value); }
        }

        public static readonly DependencyProperty WeekProperty =
            DependencyProperty.Register(
                name: "Week", 
                propertyType: typeof(ObservableCollection<ObservableCollection<TimeBlock>>), 
                ownerType: typeof(STable),
                typeMetadata: new FrameworkPropertyMetadata(null));
    }
}
