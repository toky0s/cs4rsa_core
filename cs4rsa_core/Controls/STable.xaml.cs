using Cs4rsa.BaseClasses;
using Cs4rsa.Models;

using System.Collections.ObjectModel;
using System.Windows;

namespace Cs4rsa.Controls
{
    public partial class STable : BaseUserControl
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
