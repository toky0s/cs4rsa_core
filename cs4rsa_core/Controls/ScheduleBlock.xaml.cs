using Cs4rsa.Interfaces;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cs4rsa.Controls
{
    public partial class ScheduleBlock : UserControl
    {
        public ScheduleBlock()
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
                new FrameworkPropertyMetadata(ScheduleTableItemType.SchoolClass, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
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
                new FrameworkPropertyMetadata("Nội dung hiển thị", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        public string BlockColor
        {
            get { return (string)GetValue(BlockColorProperty); }
            set { SetValue(BlockColorProperty, value); }
        }

        public static readonly DependencyProperty BlockColorProperty =
            DependencyProperty.Register(
                "BlockColor",
                typeof(string),
                typeof(ScheduleBlock),
                new PropertyMetadata(OnBlockColorChanged));

        private static void OnBlockColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleBlock sender = (ScheduleBlock)d;
            sender.OnBlockColorChanged(e);
        }

        private void OnBlockColorChanged(DependencyPropertyChangedEventArgs e)
        {
            string strBrush = (string)e.NewValue;
            BrushConverter brushConverter = new();
            SolidColorBrush brushColor = (SolidColorBrush)brushConverter.ConvertFromString(strBrush);
            Border_ScheduleBlock.Background = brushColor;
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(string.Empty +
                "Description",
                typeof(string),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata("Mô tả buổi học", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks on SchoolClass Block")]
        public event MouseButtonEventHandler ScheduleBlockClicked;

        private void Border_ScheduleBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ScheduleBlockClicked?.Invoke(this, e);
            }
        }
    }
}

