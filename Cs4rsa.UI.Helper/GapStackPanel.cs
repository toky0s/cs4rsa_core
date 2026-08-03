using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.UI.Helper
{
    public class GapStackPanel : StackPanel
    {
        public static readonly DependencyProperty GapProperty =
            DependencyProperty.Register(
                nameof(Gap), typeof(double), typeof(GapStackPanel),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public double Gap
        {
            get => (double)GetValue(GapProperty);
            set => SetValue(GapProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);
            int visibleCount = Children.Cast<UIElement>().Count(c => c.Visibility != Visibility.Collapsed);
            double totalGap = visibleCount > 1 ? Gap * (visibleCount - 1) : 0;

            return Orientation == Orientation.Horizontal
                ? new Size(size.Width + totalGap, size.Height)
                : new Size(size.Width, size.Height + totalGap);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double offset = 0;
            bool horizontal = Orientation == Orientation.Horizontal;

            foreach (UIElement child in Children)
            {
                if (child.Visibility == Visibility.Collapsed) continue;

                var desired = child.DesiredSize;
                var rect = horizontal
                    ? new Rect(offset, 0, desired.Width, arrangeSize.Height)
                    : new Rect(0, offset, arrangeSize.Width, desired.Height);

                child.Arrange(rect);
                offset += (horizontal ? desired.Width : desired.Height) + Gap;
            }

            return arrangeSize;
        }
    }
}
