using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.UI.Helper
{
    public static class ListBoxExtensions
    {
        public static readonly DependencyProperty AutoScrollToSelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToSelectedItem",
                typeof(bool),
                typeof(ListBoxExtensions),
                new PropertyMetadata(false, OnAutoScrollToSelectedItemChanged));

        public static bool GetAutoScrollToSelectedItem(DependencyObject obj) =>
            (bool)obj.GetValue(AutoScrollToSelectedItemProperty);

        public static void SetAutoScrollToSelectedItem(DependencyObject obj, bool value) =>
            obj.SetValue(AutoScrollToSelectedItemProperty, value);

        private static void OnAutoScrollToSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox && (bool)e.NewValue)
            {
                listBox.SelectionChanged += (s, args) =>
                {
                    if (listBox.SelectedItem != null)
                    {
                        // Dùng Dispatcher để đảm bảo container đã được generate
                        listBox.Dispatcher.InvokeAsync(() =>
                        {
                            listBox.ScrollIntoView(listBox.SelectedItem);
                        }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                };
            }
        }
    }
}
