using CommunityToolkit.Mvvm.Input;

using Cs4rsa.Constants;
using Cs4rsa.ViewModels;

using MaterialDesignThemes.Wpf;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cs4rsa.Views
{
    public partial class MainWindow
    {
        private const int HeightAndWidth = 24;
        private int _currentMenuItemIndex = -1;
        private MainWindowViewModel Vm;

        public MainWindow()
        {
            InitializeComponent();
            Vm = (MainWindowViewModel)DataContext;
            RenderScreen();
            RenderMainMenu();
            RenderCompactMenu();
            Vm.LoadInfor();
            Goto(Vm.StoredScreenIdx);
        }

        private void RenderScreen()
        {
            foreach (var item in ViewConstants.CredizMenu)
            {
                CredizTransitioner.Items.Add(item.Screen);
            }
        }

        private void RenderMainMenu()
        {
            Thickness textBlockMargin = new(30, 0, 0, 0);
            Thickness panelMargin = new(10, 0, 0, 0);
            foreach (CredizMenuItem item in ViewConstants.CredizMenu)
            {
                ListBoxItem listBoxItem = new();

                PackIcon icon = new()
                {
                    Kind = item.IconKind,
                    Width = HeightAndWidth,
                    Height = HeightAndWidth
                };

                TextBlock textBlock = new()
                {
                    Text = item.MenuName,
                    Margin = textBlockMargin,
                    VerticalAlignment = VerticalAlignment.Center
                };

                StackPanel stackPanel = new()
                {
                    Margin = panelMargin,
                    Orientation = Orientation.Horizontal
                };

                stackPanel.Children.Add(icon);
                stackPanel.Children.Add(textBlock);

                listBoxItem.Content = stackPanel;
                ListViewMenu.Items.Add(listBoxItem);
            }
        }

        private void RenderCompactMenu()
        {
            for (int i = 0; i < ViewConstants.CredizMenu.Length; i++)
            {
                CredizMenuItem item = ViewConstants.CredizMenu[i];
                PackIcon icon = new()
                {
                    Kind = item.IconKind,
                    Width = HeightAndWidth,
                    Height = HeightAndWidth
                };
                int idx = i;
                Button button = new()
                {
                    Margin = new Thickness(16, 0, 0, 0),
                    Content = icon,
                    ToolTip = item.MenuName,
                    Foreground = Brushes.White,
                    Style = (Style)FindResource("MaterialDesignToolButton"),
                    Command = new RelayCommand(() => Goto(idx))
                };

                CompactMenu.Children.Add(button);
            }
        }

        /// <summary>
        /// Di chuyển tới một màn hình trong App.
        /// 
        /// **Cập nhật lại Document này mỗi khi 
        /// thứ tự màn hình có sự thay đổi**
        /// 
        /// <list type="bullet">
        /// <item> 0 - <see cref="ViewConstants.Screen01"/> Trang chủ</item>
        /// <item> 1 - <see cref="ViewConstants.Screen02"/> Xếp lịch thủ công</item>
        /// <item> 2 - <see cref="ViewConstants.Screen03"/> Xếp lịch tự động</item>
        /// <item> 3 - <see cref="ViewConstants.Screen04"/> Hồ sơ</item>
        /// <item> 4 - <see cref="ViewConstants.Screen05"/> Cơ sở dữ liệu</item>
        /// <item> 5 - <see cref="ViewConstants.Screen06"/> Thông tin ứng dụng</item>
        /// </list>
        /// </summary>
        /// <param name="index">Index màn hình.</param>
        public void Goto(int index)
        {
            PackIcon icon;
            Button button;

            // Trường hợp chuyển màn hình, đưa icon trước đó về dạng unselect.
            if (index != _currentMenuItemIndex && _currentMenuItemIndex != -1)
            {
                icon = new()
                {
                    Kind = ViewConstants.CredizMenu[_currentMenuItemIndex].IconKind,
                    Width = HeightAndWidth,
                    Height = HeightAndWidth
                };
                button = CompactMenu.Children[_currentMenuItemIndex] as Button;
                button.Content = icon;
            }

            // Đi tới một màn hình, đưa icon về dạng select.
            icon = new()
            {
                Kind = ViewConstants.CredizMenu[index].IconKindOnSelected,
                Width = HeightAndWidth,
                Height = HeightAndWidth
            };
            button = CompactMenu.Children[index] as Button;
            button.Content = icon;

            ViewConstants.CredizMenu[index].LoadSelfData();
            ViewConstants.CredizMenu[index].Screen.LoadComponentsData();

            CredizTransitioner.SelectedIndex = index;
            _currentMenuItemIndex = index;
            Vm.CurrentScreenName = ViewConstants.CredizMenu[index].MenuName;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listView = sender as ListBox;
            Goto(listView.SelectedIndex);
            // Close Drawer
            DrawerHost.CloseDrawerCommand.Execute(null, null);
        }

        private void MainWd_Closed(object sender, System.EventArgs e)
        {
            Vm.SaveScreenIdx(_currentMenuItemIndex.ToString());
        }
    }
}
