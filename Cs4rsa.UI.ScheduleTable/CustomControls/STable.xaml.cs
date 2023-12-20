using Cs4rsa.UI.ScheduleTable.Models;

using MaterialDesignThemes.Wpf;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.UI.ScheduleTable.CustomControls
{
    public partial class STable : UserControl
    {
        private bool _isGotoByClick = false;
        private double _previousVerticalOffset = 0;

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

        private void STable_ScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            double verticalOffset = STable_ScrollViewer.VerticalOffset;
            double maxVerticalOffset = STable_ScrollViewer.ScrollableHeight;
            // verticalOffset tăng dần khi người dùng cuộc xuống
            // verticalOffset giảm dần khi người dùng cuộn lên
            
            if ((verticalOffset == maxVerticalOffset || verticalOffset == 0) && _isGotoByClick)
            {
                STable_BtnGoto.Visibility = Visibility.Collapsed;
                return;
            }
            _isGotoByClick = false;

            if (verticalOffset > _previousVerticalOffset || verticalOffset == 0)
            {
                STable_BtnGoto.Click -= STable_BtnGoto_GoUp;
                STable_BtnGoto.Click += STable_BtnGoto_GoDown;
                STable_BtnGoto.ToolTip = "Xuống dưới";
                STable_BtnGoto.Content = new PackIcon { Kind = PackIconKind.ArrowDown };
            }

            if (verticalOffset < _previousVerticalOffset || verticalOffset == maxVerticalOffset)
            {
                STable_BtnGoto.Click -= STable_BtnGoto_GoDown;
                STable_BtnGoto.Click += STable_BtnGoto_GoUp;
                STable_BtnGoto.ToolTip = "Lên trên";
                STable_BtnGoto.Content = new PackIcon { Kind = PackIconKind.ArrowUp };
            }

            _previousVerticalOffset = verticalOffset;
            STable_BtnGoto.Visibility = Visibility.Visible;
        }

        private void STable_BtnGoto_GoUp(object sender, RoutedEventArgs e)
        {
            STable_ScrollViewer.ScrollToTop();
            _isGotoByClick = true;
        }

        private void STable_BtnGoto_GoDown(object sender, RoutedEventArgs e)
        {
            STable_ScrollViewer.ScrollToEnd();
            _isGotoByClick = true;
        }
    }
}
