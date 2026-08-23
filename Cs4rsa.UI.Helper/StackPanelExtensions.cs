using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.UI.Helper
{
    public static class StackPanelExtensions
    {
        // Attached Property cho Gap
        public static readonly DependencyProperty GapProperty =
            DependencyProperty.RegisterAttached(
                "Gap",
                typeof(double),
                typeof(StackPanelExtensions),
                new FrameworkPropertyMetadata(
                    0.0, 
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange, 
                    OnGapChanged)
                );

        public static void SetGap(DependencyObject element, double value) =>
            element.SetValue(GapProperty, value);

        public static double GetGap(DependencyObject element) =>
            (double)element.GetValue(GapProperty);

        private static void OnGapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StackPanel stackPanel)
            {
                // Gỡ handler cũ nếu có, tránh đăng ký trùng
                stackPanel.LayoutUpdated -= StackPanel_LayoutUpdated;
                stackPanel.LayoutUpdated += StackPanel_LayoutUpdated;

                // Áp dụng ngay lập tức (kể cả ở design-time)
                ApplyGap(stackPanel);
            }
        }

        private static void StackPanel_LayoutUpdated(object sender, EventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                ApplyGap(stackPanel);
            }
        }

        private static void ApplyGap(StackPanel stackPanel)
        {
            double gap = GetGap(stackPanel);
            int count = stackPanel.Children.Count;

            for (int i = 0; i < count; i++)
            {
                if (stackPanel.Children[i] is FrameworkElement child)
                {
                    var posGap = i == count - 1 ? 0 : gap;
                    var newMargin = stackPanel.Orientation == Orientation.Horizontal
                        ? new Thickness(0, 0, posGap, 0)
                        : new Thickness(0, 0, 0, posGap);

                    // Chỉ set khi thực sự khác, tránh vòng invalidation vô tận
                    if (child.Margin != newMargin)
                    {
                        child.Margin = newMargin;
                    }
                }
            }
        }
    }
}
