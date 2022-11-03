using cs4rsa_core.Commons.Enums;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cs4rsa_core.Controls
{
    public partial class ScheduleBlock : UserControl
    {
        public DayOfWeek DayOfWeek { get; set; }
        public BlockType BlockType { get; set; }

        public ScheduleBlock()
        {
            InitializeComponent();
        }

        #region propdp BlockName | Nội dung hiển thị rút gọn
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
        #endregion

        #region propdp BlockColor | Màu nền
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
        #endregion

        #region propdp Description | Chi tiết khi rê chuột
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("" +
                "Description",
                typeof(string),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata("Mô tả buổi học", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        #endregion

        #region propdp StartIndex | Index bắt đầu
        public int StartIndex
        {
            get { return (int)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }

        public static readonly DependencyProperty StartIndexProperty =
            DependencyProperty.Register(
                "StartIndex",
                typeof(int),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        #endregion

        #region propdp EndIndex | Index kết thúc
        public int EndIndex
        {
            get { return (int)GetValue(EndIndexProperty); }
            set { SetValue(EndIndexProperty, value); }
        }

        public static readonly DependencyProperty EndIndexProperty =
            DependencyProperty.Register(
                "EndIndex", typeof(int),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        #endregion

        #region propdp Code | Tên class group
        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register(
                "ClassGroupName",
                typeof(string),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        #endregion

        #region prodp Class1 | Lớp xung đột 1
        public string Class1
        {
            get { return (string)GetValue(Class1Property); }
            set { SetValue(Class1Property, value); }
        }

        public static readonly DependencyProperty Class1Property =
            DependencyProperty.Register(
                "Class1",
                typeof(string),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        #endregion

        #region prodp Class2 | Lớp xung đột 2
        public string Class2
        {
            get { return (string)GetValue(Class2Property); }
            set { SetValue(Class2Property, value); }
        }

        public static readonly DependencyProperty Class2Property =
            DependencyProperty.Register(
                "Class2",
                typeof(string),
                typeof(ScheduleBlock),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
                );
        #endregion
    }
}

