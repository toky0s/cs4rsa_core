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
            if (hitResult != null) return;

            tooltip.IsOpen = false;
            Mouse.Capture(null);
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(tooltip, OnClickOutsideTooltip);

            // Kiểm tra nếu click nằm trên chính Border chủ sở hữu
            var border = tooltip.PlacementTarget as Border;
            if (border != null)
            {
                var hitOnBorder = VisualTreeHelper.HitTest(border, e.GetPosition(border));
                if (hitOnBorder != null)
                {
                    _suppressNextOpen = true;
                }
            }
        }
    }
}

