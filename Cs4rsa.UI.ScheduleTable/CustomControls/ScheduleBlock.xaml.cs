using System;
using System.Windows;
using System.Windows.Controls;
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
    }
}

