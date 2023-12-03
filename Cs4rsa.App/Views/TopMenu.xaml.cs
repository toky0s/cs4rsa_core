using Cs4rsa.App.Events.TopMenuEvents;

using MaterialDesignThemes.Wpf;

using Prism.Events;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cs4rsa.App.Views
{
    public class TopMenuItem
    {
        public string ScreenName { get; set; }
        public PackIconKind PackIconKind { get; set; }
        public PackIconKind PackIconKindSelected { get; set; }
    }

    public partial class TopMenu : UserControl
    {
        private TopMenuItem[] TopMenuItems { get; set; } = new TopMenuItem[] { 
            new TopMenuItem() {ScreenName = "Trang chủ", PackIconKind = PackIconKind.HomeOutline, PackIconKindSelected = PackIconKind.Home},
            new TopMenuItem() {ScreenName = "Xếp lịch thủ công", PackIconKind = PackIconKind.CursorDefaultOutline, PackIconKindSelected = PackIconKind.CursorDefault},
            new TopMenuItem() {ScreenName = "Xếp lịch tự động", PackIconKind = PackIconKind.LightbulbOutline, PackIconKindSelected = PackIconKind.Lightbulb},
            new TopMenuItem() {ScreenName = "Hồ sơ", PackIconKind = PackIconKind.ClipboardAccountOutline, PackIconKindSelected = PackIconKind.ClipboardAccount},
            new TopMenuItem() {ScreenName = "Dữ liệu", PackIconKind = PackIconKind.DatabaseOutline, PackIconKindSelected = PackIconKind.Database},
            new TopMenuItem() {ScreenName = "Thông tin", PackIconKind = PackIconKind.InformationOutline, PackIconKindSelected = PackIconKind.Information},
        };

        private readonly IEventAggregator _eventAggregator;
        private int _currentMenuItemIndex;

        public TopMenu(IEventAggregator ea)
        {
            _eventAggregator = ea;
            InitializeComponent();
            GenerateTopMenuItem();
            Goto(0);
        }

        private void GenerateTopMenuItem()
        {
            for (int i = 0; i < TopMenuItems.Length; i++)
            {
                var item = TopMenuItems[i];
                PackIcon icon = new PackIcon()
                {
                    Kind = item.PackIconKind,
                    Width = 24,
                    Height = 24
                };
                Button button = new Button()
                {
                    Margin = new Thickness(16, 0, 0, 0),
                    Content = icon,
                    ToolTip = item.ScreenName,
                    Foreground = Brushes.White,
                    Style = (Style)FindResource("MaterialDesignToolButton"),
                };

                int idx = i;
                button.Click += (object sender, RoutedEventArgs e) =>
                {
                    Goto(idx);
                };

                PART_CompactMenu.Children.Add(button);
            }
        }

        private void Goto(int index)
        {
            PackIcon icon;
            Button button;

            // Trường hợp chuyển màn hình, đưa icon trước đó về dạng unselect.
            if (index != _currentMenuItemIndex && _currentMenuItemIndex != -1)
            {
                icon = new PackIcon()
                {
                    Kind = TopMenuItems[_currentMenuItemIndex].PackIconKind,
                    Width = 24,
                    Height = 24
                };
                button = PART_CompactMenu.Children[_currentMenuItemIndex] as Button;
                button.Content = icon;
            }

            // Đi tới một màn hình, đưa icon về dạng select.
            icon = new PackIcon()
            {
                Kind = TopMenuItems[index].PackIconKindSelected,
                Width = 24,
                Height = 24
            };
            button = PART_CompactMenu.Children[index] as Button;
            button.Content = icon;

            PART_TopMenuTitle.Text = TopMenuItems[index].ScreenName;
            _currentMenuItemIndex = index;
            _eventAggregator.GetEvent<ScreenChangedEvent>().Publish(index);
        }
    }
}
