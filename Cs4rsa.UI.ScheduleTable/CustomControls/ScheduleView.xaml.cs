using Cs4rsa.UI.ScheduleTable.Models;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.UI.ScheduleTable.CustomControls
{
    /// <summary>
    /// MVVM-friendly schedule grid with two-way bindable collections and vertical scroll offset.
    /// Bind <see cref="ItemsSource"/> (timeline labels), <see cref="Week"/> (per-day blocks), and optionally
    /// </summary>
    public partial class ScheduleView : UserControl
    {
        private const string GlyphScrollDown = "\u25BC";
        private const string GlyphScrollUp = "\u25B2";

        private bool _isNavigateByClick;
        private bool _scrollDownIntent = true;
        private double _previousVerticalOffset;

        public ScheduleView()
        {
            InitializeComponent();
            SetCurrentValue(
                ItemsSourceProperty,
                new ObservableCollection<string>(Utils.Utils.TimeLines));
            SetCurrentValue(
                DayHeadersProperty,
                new ObservableCollection<string>(new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" }));
        }

        public ObservableCollection<string> ItemsSource
        {
            get => (ObservableCollection<string>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(ObservableCollection<string>),
                typeof(ScheduleView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<ObservableCollection<TimeBlock>> Week
        {
            get => (ObservableCollection<ObservableCollection<TimeBlock>>)GetValue(WeekProperty);
            set => SetValue(WeekProperty, value);
        }

        public static readonly DependencyProperty WeekProperty =
            DependencyProperty.Register(
                nameof(Week),
                typeof(ObservableCollection<ObservableCollection<TimeBlock>>),
                typeof(ScheduleView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<string> DayHeaders
        {
            get => (ObservableCollection<string>)GetValue(DayHeadersProperty);
            set => SetValue(DayHeadersProperty, value);
        }

        public static readonly DependencyProperty DayHeadersProperty =
            DependencyProperty.Register(
                nameof(DayHeaders),
                typeof(ObservableCollection<string>),
                typeof(ScheduleView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Optional template for each <see cref="TimeBlock"/>; when null, the default <see cref="ScheduleBlock"/> template is used.
        /// </summary>
        public DataTemplate BlockItemTemplate
        {
            get => (DataTemplate)GetValue(BlockItemTemplateProperty);
            set => SetValue(BlockItemTemplateProperty, value);
        }

        public static readonly DependencyProperty BlockItemTemplateProperty =
            DependencyProperty.Register(
                nameof(BlockItemTemplate),
                typeof(DataTemplate),
                typeof(ScheduleView),
                new PropertyMetadata(null));

        /// <summary>
        /// Vertical scroll position in pixels; two-way bindable for restoring or syncing scroll state.
        /// </summary>
        public double VerticalScrollOffset
        {
            get => (double)GetValue(VerticalScrollOffsetProperty);
            set => SetValue(VerticalScrollOffsetProperty, value);
        }

        public static readonly DependencyProperty VerticalScrollOffsetProperty =
            DependencyProperty.Register(
                nameof(VerticalScrollOffset),
                typeof(double),
                typeof(ScheduleView),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnVerticalScrollOffsetChanged));

        /// <summary>
        /// Total height of the schedule body (timeline + week columns). Default matches the classic fixed grid height.
        /// </summary>
        public double ScheduleBodyHeight
        {
            get => (double)GetValue(ScheduleBodyHeightProperty);
            set => SetValue(ScheduleBodyHeightProperty, value);
        }

        public static readonly DependencyProperty ScheduleBodyHeightProperty =
            DependencyProperty.Register(
                nameof(ScheduleBodyHeight),
                typeof(double),
                typeof(ScheduleView),
                new FrameworkPropertyMetadata(400d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Minimum height for the schedule body when stretching inside the parent.
        /// </summary>
        public double ScheduleBodyMinHeight
        {
            get => (double)GetValue(ScheduleBodyMinHeightProperty);
            set => SetValue(ScheduleBodyMinHeightProperty, value);
        }

        public static readonly DependencyProperty ScheduleBodyMinHeightProperty =
            DependencyProperty.Register(
                nameof(ScheduleBodyMinHeight),
                typeof(double),
                typeof(ScheduleView),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool ShowScrollNavigation
        {
            get => (bool)GetValue(ShowScrollNavigationProperty);
            set => SetValue(ShowScrollNavigationProperty, value);
        }

        public static readonly DependencyProperty ShowScrollNavigationProperty =
            DependencyProperty.Register(
                nameof(ShowScrollNavigation),
                typeof(bool),
                typeof(ScheduleView),
                new PropertyMetadata(true));

        private bool _suppressScrollOffsetPush;

        private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ScheduleView)d;
            if (view._suppressScrollOffsetPush || view.PART_ScrollViewer == null)
            {
                return;
            }

            double v = (double)e.NewValue;
            if (!double.IsNaN(v) && !double.IsInfinity(v))
            {
                view.PART_ScrollViewer.ScrollToVerticalOffset(v);
            }
        }
    }
}
