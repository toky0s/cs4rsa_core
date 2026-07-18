using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Cs4rsa.Module.ManuallySchedule.UC
{
    /// <summary>
    /// Interaction logic for MultiSelection.xaml
    /// </summary>
    public partial class MultiSelection : UserControl
    {
        public MultiSelection()
        {
            InitializeComponent();
        }

        public ObservableCollection<MultiSelectionItem> ItemsSource
        {
            get { return (ObservableCollection<MultiSelectionItem>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(ObservableCollection<MultiSelectionItem>),
                typeof(MultiSelection),
                new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MultiSelection)d;
            if (e.NewValue is ObservableCollection<MultiSelectionItem> newItems)
            {
                control.FilteredItems = CollectionViewSource.GetDefaultView(newItems);
                control.FilteredItems.Filter = (item) =>
                {
                    if (item is MultiSelectionItem multiSelectionItem)
                    {
                        var searchText = control.TextBlock_Search.Text.Trim();
                        if (string.IsNullOrWhiteSpace(searchText))
                        {
                            return true; // Show all items if search text is empty
                        }
                        return multiSelectionItem.Label.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                    return false;
                };
            }
        }

        public ObservableCollection<MultiSelectionItem> SelectedItems
        {
            get { return (ObservableCollection<MultiSelectionItem>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<MultiSelectionItem>), typeof(MultiSelection), new PropertyMetadata(null));


        public ICollectionView FilteredItems { get; set; }

        private void TextBlock_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilteredItems.Refresh();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListBox)) return;

            // Nếu SelectedItems chưa được khởi tạo thì tạo mới
            if (SelectedItems == null)
            {
                SelectedItems = new ObservableCollection<MultiSelectionItem>();
            }

            // Xóa các item bị bỏ chọn
            foreach (MultiSelectionItem removed in e.RemovedItems)
            {
                SelectedItems.Remove(removed);
            }

            // Thêm các item mới được chọn
            foreach (MultiSelectionItem added in e.AddedItems)
            {
                if (!SelectedItems.Contains(added))
                {
                    SelectedItems.Add(added);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is MultiSelectionItem mulItem)
            {
                int index = (int)button.Tag;
                // Xóa item khỏi SelectedItems
                SelectedItems.Remove(mulItem);
                ListBox_Items.SelectedItems.RemoveAt(index);
            }
        }

    }
}
