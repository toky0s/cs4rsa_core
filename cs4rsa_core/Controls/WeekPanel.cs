using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cs4rsa.Controls
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

        private double _xPosition;

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

            double width = ActualWidth;
            double height = ActualHeight;
            if (width <= 1 || height <= 1)
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
            double colW = width / dayCount;

            for (int i = 1; i < dayCount; i++)
            {
                double x = Math.Floor(i * colW) + 0.5;
                dc.DrawLine(pen, new Point(x, 0), new Point(x, height));
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize.Width /= 7;
            _xPosition = availableSize.Width;
            foreach (ContentPresenter child in Children)
            {
                child.Measure(availableSize);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double w = finalSize.Width / 7;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Arrange(new Rect(_xPosition * i, 0, w, finalSize.Height));
            }

            return finalSize;
        }
    }
}
