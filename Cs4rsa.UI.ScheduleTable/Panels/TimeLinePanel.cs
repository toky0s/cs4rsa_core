using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cs4rsa.UI.ScheduleTable.Panels
{
    public class TimelinePanel : Panel
    {
        // ── Dependency Properties ────────────────────────────────────────────

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register(nameof(LabelWidth), typeof(double), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(28d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register(nameof(GridLineBrush), typeof(Brush), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0)),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register(nameof(LabelForeground), typeof(Brush), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(TimelinePanel),
                new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double LabelWidth
        {
            get => (double)GetValue(LabelWidthProperty);
            set => SetValue(LabelWidthProperty, value);
        }

        public Brush GridLineBrush
        {
            get => (Brush)GetValue(GridLineBrushProperty);
            set => SetValue(GridLineBrushProperty, value);
        }

        public Brush LabelForeground
        {
            get => (Brush)GetValue(LabelForegroundProperty);
            set => SetValue(LabelForegroundProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        // ── Layout ───────────────────────────────────────────────────────────

        protected override Size MeasureOverride(Size availableSize)
        {
            var lines = Utils.Utils.TimeLines;
            int count = lines.Length;

            // Chiều cao tối thiểu: mỗi dòng ít nhất FontSize + 4px padding
            double minRowHeight = FontSize + 4d;
            double desiredHeight = double.IsPositiveInfinity(availableSize.Height)
                ? count * minRowHeight
                : availableSize.Height;

            double desiredWidth = double.IsPositiveInfinity(availableSize.Width)
                ? LabelWidth
                : availableSize.Width;

            return new Size(desiredWidth, desiredHeight);
        }

        protected override Size ArrangeOverride(Size finalSize) => finalSize;

        // ── Rendering ────────────────────────────────────────────────────────

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var lines = Utils.Utils.TimeLines;
            int count = lines.Length;
            double h = RenderSize.Height;
            double w = RenderSize.Width;
            double unit = h / count;

            // Pixel-snap pen (align to device pixels như SnapsToDevicePixels)
            var pen = new Pen(GridLineBrush, 1d);
            pen.Freeze();

            var typeface = new Typeface(
                new FontFamily("Segoe UI"),
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal);

            for (int i = 0; i < count; i++)
            {
                double y = SnapToPixel(i * unit);

                // Kẻ đường ngang
                dc.DrawLine(pen, new Point(0, y), new Point(w, y));

                // Label time
                var ft = new FormattedText(
                    lines[i],
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    FontSize,
                    LabelForeground,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                // Căn giữa dọc trong ô, căn phải trong LabelWidth
                double labelY = y + (unit - ft.Height) / 2d;
                double labelX = LabelWidth - ft.Width - 2d; // margin phải 2px
                dc.DrawText(ft, new Point(labelX, labelY));
            }
        }

        private static double SnapToPixel(double value) => System.Math.Round(value) + 0.5;
    }
}