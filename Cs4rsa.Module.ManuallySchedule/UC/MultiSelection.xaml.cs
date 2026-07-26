using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cs4rsa.Module.ManuallySchedule.UC
{
    public partial class MultiSelection : UserControl
    {
        public MultiSelection()
        {
            InitializeComponent();
            Loaded += MultiSelection_Loaded;
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
                            return true;
                        }
                        return multiSelectionItem.Label.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                    return false;
                };

                // ItemsSource vừa đổi -> đồng bộ lại checkbox theo SelectedItems hiện có
                control.SyncListBoxSelectionFromModel();
            }
        }

        public ObservableCollection<MultiSelectionItem> SelectedItems
        {
            get { return (ObservableCollection<MultiSelectionItem>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                "SelectedItems",
                typeof(ObservableCollection<MultiSelectionItem>),
                typeof(MultiSelection),
                new PropertyMetadata(null, SelectedItemsChanged)
            );

        private static void SelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MultiSelection)d;

            // Gỡ theo dõi collection cũ (nếu có)
            if (e.OldValue is ObservableCollection<MultiSelectionItem> oldItems)
            {
                oldItems.CollectionChanged -= control.SelectedItems_CollectionChanged;
            }

            if (e.NewValue is ObservableCollection<MultiSelectionItem> newItems)
            {
                // Theo dõi khi bên ngoài Add/Remove trực tiếp vào SelectedItems (không qua UI)
                newItems.CollectionChanged += control.SelectedItems_CollectionChanged;

                // SelectedItems vừa được binding -> đồng bộ checkbox ngay
                control.SyncListBoxSelectionFromModel();
            }
        }

        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SyncListBoxSelectionFromModel();
        }

        private void MultiSelection_Loaded(object sender, RoutedEventArgs e)
        {
            // Đảm bảo đồng bộ 1 lần nữa sau khi control đã load xong (phòng trường hợp
            // ItemsSource/SelectedItems set trước khi ListBox render container)
            SyncListBoxSelectionFromModel();
        }

        /// <summary>
        /// Đồng bộ lại ListBox_Items.SelectedItems dựa theo dữ liệu hiện có trong SelectedItems,
        /// để checkbox tự động check nếu item đã tồn tại sẵn trong SelectedItems.
        /// </summary>
        private void SyncListBoxSelectionFromModel()
        {
            if (SelectedItems == null || ListBox_Items == null || ItemsSource == null)
            {
                return;
            }

            ListBox_Items.SelectionChanged -= ListBox_SelectionChanged;
            try
            {
                ListBox_Items.SelectedItems.Clear();

                foreach (var selected in SelectedItems)
                {
                    // Tìm đúng instance trong ItemsSource có cùng ID
                    var matched = ItemsSource.FirstOrDefault(x => x.ID == selected.ID);
                    if (matched != null && !ListBox_Items.SelectedItems.Contains(matched))
                    {
                        ListBox_Items.SelectedItems.Add(matched);
                    }
                }
            }
            finally
            {
                ListBox_Items.SelectionChanged += ListBox_SelectionChanged;
            }
        }

        public ICollectionView FilteredItems { get; set; }

        private void TextBlock_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilteredItems.Refresh();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListBox)) return;

            if (SelectedItems == null)
            {
                throw new NullReferenceException("SelectedItems of MultiSelection is null, please initialize before bind");
            }

            foreach (MultiSelectionItem removed in e.RemovedItems)
            {
                SelectedItems.Remove(removed);
            }

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
                SelectedItems.Remove(mulItem);
                ListBox_Items.SelectedItems.Remove(mulItem);
            }
        }
    }
}