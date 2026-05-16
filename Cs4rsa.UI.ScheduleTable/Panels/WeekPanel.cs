using Cs4rsa.UI.ScheduleTable.Models;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cs4rsa.UI.ScheduleTable.Panels
{
    /// <summary>
    /// WeekPanel tự quản lý 7 DayPanel cố định (T2–CN).
    /// Nhận dữ liệu qua DP <see cref="Week"/> thay vì ItemsControl bên ngoài.
    /// Khi Week thay đổi, tự phân phối TimeBlock vào đúng DayPanel theo DayOfWeek.
    /// </summary>
    public class WeekPanel : Panel
    {
        // ── Constants ────────────────────────────────────────────────────────

        private const int DayCount = 7;

        // DayOfWeek → index cột (Monday=0 … Sunday=6)
        private static readonly DayOfWeek[] DayOrder = new[]
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday,
        };

        // ── Fields ───────────────────────────────────────────────────────────

        private static readonly SolidColorBrush DefaultGridLineBrush;
        private readonly DayPanel[] _dayPanels = new DayPanel[DayCount];

        // ── Static ctor ──────────────────────────────────────────────────────

        static WeekPanel()
        {
            DefaultGridLineBrush = new SolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
            DefaultGridLineBrush.Freeze();
        }

        // ── Ctor ─────────────────────────────────────────────────────────────

        public WeekPanel()
        {
            for (int i = 0; i < DayCount; i++)
            {
                _dayPanels[i] = new DayPanel();
                Children.Add(_dayPanels[i]);
            }
        }

        // ── Dependency Properties ─────────────────────────────────────────────

        public static readonly DependencyProperty WeekProperty =
            DependencyProperty.Register(
                nameof(Week),
                typeof(ObservableCollection<TimeBlock>),
                typeof(WeekPanel),
                new FrameworkPropertyMetadata(null, OnWeekChanged));

        public ObservableCollection<TimeBlock> Week
        {
            get => (ObservableCollection<TimeBlock>)GetValue(WeekProperty);
            set => SetValue(WeekProperty, value);
        }

        public static readonly DependencyProperty BlockItemTemplateProperty =
            DependencyProperty.Register(
                nameof(BlockItemTemplate),
                typeof(DataTemplate),
                typeof(WeekPanel),
                new FrameworkPropertyMetadata(null, OnBlockItemTemplateChanged));

        /// <summary>
        /// DataTemplate dùng để render từng TimeBlock.
        /// Khi null, WeekPanel sẽ dùng DefaultBlockTemplate nội bộ.
        /// </summary>
        public DataTemplate BlockItemTemplate
        {
            get => (DataTemplate)GetValue(BlockItemTemplateProperty);
            set => SetValue(BlockItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register(
                nameof(ShowGridLines),
                typeof(bool),
                typeof(WeekPanel),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ShowGridLines
        {
            get => (bool)GetValue(ShowGridLinesProperty);
            set => SetValue(ShowGridLinesProperty, value);
        }

        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register(
                nameof(GridLineBrush),
                typeof(Brush),
                typeof(WeekPanel),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush GridLineBrush
        {
            get => (Brush)GetValue(GridLineBrushProperty);
            set => SetValue(GridLineBrushProperty, value);
        }

        // ── Callbacks ────────────────────────────────────────────────────────

        private static void OnWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (WeekPanel)d;

            if (e.OldValue is ObservableCollection<TimeBlock> old)
                old.CollectionChanged -= panel.OnWeekCollectionChanged;

            if (e.NewValue is ObservableCollection<TimeBlock> newWeek)
                newWeek.CollectionChanged += panel.OnWeekCollectionChanged;

            panel.RebuildDayPanels();
        }

        private static void OnBlockItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Template thay đổi → rebuild để áp dụng template mới
            ((WeekPanel)d).RebuildDayPanels();
        }

        private void OnWeekCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildDayPanels();
        }

        // ── Core logic ───────────────────────────────────────────────────────

        /// <summary>
        /// Xóa toàn bộ ContentPresenter trong 7 DayPanel rồi
        /// phân phối lại TimeBlock theo DayOfWeek.
        /// </summary>
        private void RebuildDayPanels()
        {
            foreach (var dp in _dayPanels)
                dp.Children.Clear();

            if (Week == null) return;

            DataTemplate template = BlockItemTemplate;

            foreach (var block in Week)  // flat list, không cần loop lồng
            {
                int colIndex = GetDayIndex(block.DayOfWeek);
                if (colIndex < 0) continue;

                _dayPanels[colIndex].Children.Add(new ContentPresenter
                {
                    Content = block,
                    ContentTemplate = template,
                });
            }

            InvalidateMeasure();
        }

        /// <summary>
        /// Lấy index cột (0–6) từ DayOfWeek. Trả về -1 nếu không hợp lệ.
        /// </summary>
        private static int GetDayIndex(DayOfWeek day)
        {
            for (int i = 0; i < DayOrder.Length; i++)
            {
                if (DayOrder[i] == day) return i;
            }
            return -1;
        }

        // ── Layout ───────────────────────────────────────────────────────────

        protected override Size MeasureOverride(Size availableSize)
        {
            double colW = availableSize.Width / DayCount;
            double maxH = 0;

            var colSize = new Size(colW, availableSize.Height);
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(colSize);
                maxH = Math.Max(maxH, child.DesiredSize.Height);
            }

            double w = double.IsPositiveInfinity(availableSize.Width) ? colW * DayCount : availableSize.Width;
            double h = double.IsPositiveInfinity(availableSize.Height) ? maxH : availableSize.Height;
            return new Size(w, h);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double colW = finalSize.Width / DayCount;
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                InternalChildren[i].Arrange(new Rect(i * colW, 0, colW, finalSize.Height));
            }
            return finalSize;
        }

        // ── Render ───────────────────────────────────────────────────────────

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (!ShowGridLines) return;

            double w = ActualWidth;
            double h = ActualHeight;
            if (w <= 1 || h <= 1) return;

            Brush brush = GridLineBrush ?? DefaultGridLineBrush;
            var pen = new Pen(brush, 1d);
            if (pen.CanFreeze) pen.Freeze();

            double colW = w / DayCount;
            for (int i = 1; i < DayCount; i++)
            {
                double x = Math.Floor(i * colW) + 0.5;
                dc.DrawLine(pen, new Point(x, 0), new Point(x, h));
            }
        }
    }
}