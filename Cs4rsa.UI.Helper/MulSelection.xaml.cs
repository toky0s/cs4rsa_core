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

using System.Collections.ObjectModel; 
namespace Cs4rsa.UI.Helper
{
    public class ItemModel
    {
        public string ID { get; set; }
        public string Label { get; set; }
    }

    public partial class MulSelection : UserControl
    {
        public MulSelection()
        {
            InitializeComponent();
            SelectedItems = new ObservableCollection<ItemModel>();
        }

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(MulSelection), new PropertyMetadata("Select item..."));

        public ObservableCollection<ItemModel> ItemsSource
        {
            get { return (ObservableCollection<ItemModel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<ItemModel>), typeof(MulSelection), new PropertyMetadata(null));

        public ObservableCollection<ItemModel> SelectedItems
        {
            get { return (ObservableCollection<ItemModel>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<ItemModel>), typeof(MulSelection), new PropertyMetadata(null));

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(MulSelection), new PropertyMetadata(false));

        private void InputBox_GotFocus(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = true;
        }

        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is ItemModel item)
            {
                if (!SelectedItems.Any(x => x.ID == item.ID))
                {
                    SelectedItems.Add(item);
                }
            }
        }

        private void RemoveChip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ItemModel item)
            {
                SelectedItems.Remove(item);
            }
        }
    }
}
