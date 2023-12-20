using Cs4rsa.UI.ScheduleTable.Interfaces;
using Cs4rsa.UI.ScheduleTable.Models;

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

        public ScheduleTableItemType ScheduleTableItemType
        {
            get { return (ScheduleTableItemType)GetValue(ScheduleTableItemTypeProperty); }
            set { SetValue(ScheduleTableItemTypeProperty, value); }
        }

        public static readonly DependencyProperty ScheduleTableItemTypeProperty =
            DependencyProperty.Register(
                "ScheduleTableItemType",
                typeof(ScheduleTableItemType),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata(
                    ScheduleTableItemType.SchoolClass
                  , FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );

        public string BlockName
        {
            get { return (string)GetValue(BlockNameProperty); }
            set { SetValue(BlockNameProperty, value); }
        }

        public static readonly DependencyProperty BlockNameProperty =
            DependencyProperty.Register(
                "BlockName",
                typeof(string),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata(
                    "Nội dung hiển thị"
                  , FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );

        public Brush BlockColor
        {
            get { return (Brush)GetValue(BlockColorProperty); }
            set { SetValue(BlockColorProperty, value); }
        }

        public static readonly DependencyProperty BlockColorProperty =
            DependencyProperty.Register(
                "BlockColor",
                typeof(Brush),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata());

        public TimeBlock TimeBlock
        {
            get { return (TimeBlock)GetValue(TimeBlockProperty); }
            set { SetValue(TimeBlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeBlock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeBlockProperty =
            DependencyProperty.Register(
                "TimeBlock"
                , typeof(TimeBlock)
                , typeof(ScheduleBlock)
                , new FrameworkPropertyMetadata(null)
            );

        private void SchoolClassBlock_Border_ToolTip__Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

