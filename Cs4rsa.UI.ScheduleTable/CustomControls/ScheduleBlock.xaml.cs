using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cs4rsa.UI.ScheduleTable.CustomControls
{
    public partial class ScheduleBlock: UserControl
    {
        public ScheduleBlock(): base()
        {
            InitializeComponent();
        }

        public TimeBlock TimeBlock
        {
            get { return (TimeBlock)GetValue(TimeBlockProperty); }
            set { SetValue(TimeBlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeBlock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeBlockProperty =
            DependencyProperty.Register(
                "TimeBlock",
                typeof(TimeBlock),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata(null)
            );

        // Thank Claude Code =))
        private bool _suppressNextOpen = false;

        private void Border_ScheduleBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var tooltip = (ToolTip)border.ToolTip;
            if (tooltip == null) return;

            if (_suppressNextOpen)
            {
                _suppressNextOpen = false;
                return;
            }

            tooltip.DataContext = DataContext;
            tooltip.PlacementTarget = border;
            tooltip.StaysOpen = true;
            tooltip.IsOpen = true;

            Mouse.Capture(tooltip, CaptureMode.SubTree);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(tooltip, OnClickOutsideTooltip);
        }

        private void OnClickOutsideTooltip(object sender, MouseButtonEventArgs e)
        {
            var tooltip = (ToolTip)sender;

            // Kiểm tra nếu click nằm trong tooltip → không đóng
            var hitResult = VisualTreeHelper.HitTest(tooltip, e.GetPosition(tooltip));
            if (hitResult != null)
            {
                // Nếu click vào Button thì release capture để Button nhận được Click event
                var clickedButton = FindVisualParent<Button>(hitResult.VisualHit);
                if (clickedButton != null)
                {
                    Mouse.Capture(clickedButton);
                    Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(tooltip, OnClickOutsideTooltip);
                    tooltip.IsOpen = false;
                }
                return;
            }

            tooltip.IsOpen = false;
            Mouse.Capture(null);
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(tooltip, OnClickOutsideTooltip);

            // Kiểm tra nếu click nằm trên chính Border chủ sở hữu
            if (tooltip.PlacementTarget is Border border)
            {
                var hitOnBorder = VisualTreeHelper.HitTest(border, e.GetPosition(border));
                if (hitOnBorder != null)
                {
                    _suppressNextOpen = true;
                }
            }
        }

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var current = child;
            while (current != null)
            {
                if (current is T target) return target;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private void Unselect_First(object sender, RoutedEventArgs e)
        {
            var classGroupName = this.TimeBlock.FirstCfClass.ClassGroupName;
            OnUnselectClassGroup(classGroupName);
        }

        private void Unselect_Second(object sender, RoutedEventArgs e)
        {
            var classGroupName = this.TimeBlock.SecondCfClass.ClassGroupName;
            OnUnselectClassGroup(classGroupName);
        }

        // Routed Event
        /// <summary>
        /// Event này được raise mỗi khi người dùng unselect một class group 
        /// (bằng cách click vào nút "Unselect" trong tooltip của ScheduleBlock).
        /// </summary>
        public static readonly RoutedEvent UnselectClassGroupEvent =
            EventManager.RegisterRoutedEvent(
                nameof(UnselectClassGroup),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ScheduleView_v2));

        public event RoutedEventHandler UnselectClassGroup
        {
            add => AddHandler(UnselectClassGroupEvent, value);
            remove => RemoveHandler(UnselectClassGroupEvent, value);
        }

        // Raise helper
        protected virtual void OnUnselectClassGroup(string classGroupName)
        {
            var args = new UnselectClassGroupEventArgs(UnselectClassGroupEvent, classGroupName);
            RaiseEvent(args);
        }

        public class UnselectClassGroupEventArgs : RoutedEventArgs
        {
            public string ClassGroupName { get; }

            public UnselectClassGroupEventArgs(RoutedEvent routedEvent, string classGroupName)
                : base(routedEvent)
            {
                ClassGroupName = classGroupName;
            }
        }
    }
}

