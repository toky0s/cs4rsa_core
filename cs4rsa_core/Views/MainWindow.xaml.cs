using CommunityToolkit.Mvvm.Input;

using Cs4rsa.Constants;
using Cs4rsa.ViewModels;

using MaterialDesignThemes.Wpf;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cs4rsa.Views
{
    public partial class MainWindow : Window
    {
        private static readonly int _heightAndWidth = 24;
        private int _currentMenuItemIndex = -1;
        public MainWindow()
        {
            InitializeComponent();
            RenderScreen();
            RenderMainMenu();
            RenderCompactMenu();
            Goto((int)ScreenIndex.HAND);
        }

        private void RenderScreen()
        {
            foreach (var item in ViewConstants.CREDIZ_MENU_ITEMS)
            {
                CredizTransitioner.Items.Add(item.CredizScreen.Screen);
            }
        }

        public void RenderMainMenu()
        {

            Thickness textBlockMargin = new(30, 0, 0, 0);
            Thickness panelMargin = new(10, 0, 0, 0);
            foreach (CredizMenuItem item in ViewConstants.CREDIZ_MENU_ITEMS)
            {
                ListBoxItem listBoxItem = new();

                PackIcon icon = new()
                {
                    Kind = item.IconKind,
                    Width = _heightAndWidth,
                    Height = _heightAndWidth
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

        public void RenderCompactMenu()
        {
            foreach (CredizMenuItem item in ViewConstants.CREDIZ_MENU_ITEMS)
            {
                PackIcon icon = new()
                {
                    Kind = item.IconKind,
                    Width = _heightAndWidth,
                    Height = _heightAndWidth
                };

                Button button = new()
                {
                    Margin = new Thickness(16, 0, 0, 0),
                    Content = icon,
                    ToolTip = item.MenuName,
                    Foreground = Brushes.White,
                    Style = (Style)FindResource("MaterialDesignToolButton"),
                    Command = new RelayCommand(() => Goto((int)item.CredizScreen.Index))
                };

                CompactMenu.Children.Add(button);
            }
        }

        #region Util functions
        private void Goto(int index)
        {
            PackIcon icon;
            Button button;
            if (index != _currentMenuItemIndex && _currentMenuItemIndex != -1)
            {
                icon = new()
                {
                    Kind = ViewConstants.CREDIZ_MENU_ITEMS[_currentMenuItemIndex].IconKind,
                    Width = _heightAndWidth,
                    Height = _heightAndWidth
                };
                button = CompactMenu.Children[_currentMenuItemIndex] as Button;
                button.Content = icon;
            }

            icon = new()
            {
                Kind = ViewConstants.CREDIZ_MENU_ITEMS[index].IconKindOnSelected,
                Width = _heightAndWidth,
                Height = _heightAndWidth
            };
            ListViewMenu.SelectedIndex = index;
            button = CompactMenu.Children[index] as Button;
            button.Content = icon;

            CredizTransitioner.SelectedIndex = index;
            _currentMenuItemIndex = index;
        }
        #endregion

        #region Event handlers
        private async void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listView = sender as ListBox;
            Goto(listView.SelectedIndex);
            if (listView.SelectedIndex == (int)ScreenIndex.TEACHER)
            {
                LectureViewModel lecture = (LectureViewModel)(CredizTransitioner.Items[listView.SelectedIndex] as Lecture).DataContext;
                await lecture.LoadTeachers();
            }
            // Close Drawer
            DrawerHost.CloseDrawerCommand.Execute(null, null);
        }
        #endregion
    }
}
