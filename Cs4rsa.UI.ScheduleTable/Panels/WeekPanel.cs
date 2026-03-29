using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cs4rsa.UI.ScheduleTable.Panels
{
    /// <summary>
    /// Một WeekPanel sẽ chứa 7 DayPanel tương ứng.
    /// MeasureOverride: Tính toán Width expected cho từng DayPanel.
    /// ArrangeOverride: Sắp xếp và tính toán lại Width của DayPanel theo hàng ngang.
    /// </summary>
    public class WeekPanel : Panel
    {
        private static readonly SolidColorBrush DefaultGridLineBrush;

        static WeekPanel()
        {
            DefaultGridLineBrush = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
            DefaultGridLineBrush.Freeze();
        }

        public bool ShowGridLines
        {
            get => (bool)GetValue(ShowGridLinesProperty);
            set => SetValue(ShowGridLinesProperty, value);
        }

        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register(
                nameof(ShowGridLines),
                typeof(bool),
                typeof(WeekPanel),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>Kẻ dọc giữa các cột thứ. Kẻ ngang theo giờ được vẽ ở lớp timeline.</summary>
        public Brush GridLineBrush
        {
            get => (Brush)GetValue(GridLineBrushProperty);
            set => SetValue(GridLineBrushProperty, value);
        }

        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register(
                nameof(GridLineBrush),
                typeof(Brush),
                typeof(WeekPanel),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (!ShowGridLines)
            {
                return;
            }

            double w = ActualWidth;
            double h = ActualHeight;
            if (w <= 1 || h <= 1)
            {
                return;
            }

            Brush brush = GridLineBrush ?? DefaultGridLineBrush;
            var pen = new Pen(brush, 1);
            if (pen.CanFreeze)
            {
                pen.Freeze();
            }

            const int dayCount = 7;
            double colW = w / dayCount;

            for (int i = 1; i < dayCount; i++)
            {
                double x = Math.Floor(i * colW) + 0.5;
                dc.DrawLine(pen, new Point(x, 0), new Point(x, h));
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double totalWidth = 0;
            double totalHeight = 0;

            foreach (UIElement child in InternalChildren)
            {
                // Measure each child with available size
                child.Measure(availableSize);

                // Get the desired size of the child
                Size desiredSize = child.DesiredSize;

                // Accumulate the total width and height
                totalWidth += desiredSize.Width;
                totalHeight = Math.Max(totalHeight, desiredSize.Height);
            }

            // Ensure that the returned size is not infinite
            totalWidth = double.IsPositiveInfinity(totalWidth) ? availableSize.Width : totalWidth;
            totalHeight = double.IsPositiveInfinity(totalHeight) ? availableSize.Height : totalHeight;

            // Return the total desired size
            return new Size(totalWidth, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double childWidth = finalSize.Width / 7; // Divide the total width by 7 for 7 children
            double childHeight = finalSize.Height; // Use the full height

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                if (child != null)
                {
                    double x = i * childWidth;
                    double y = 0;
                    Rect rect = new Rect(x, y, childWidth, childHeight);
                    child.Arrange(rect);
                }
            }

            // Return the final arranged size
            return finalSize;
        }
    }
}
