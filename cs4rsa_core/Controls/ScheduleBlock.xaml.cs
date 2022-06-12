using cs4rsa_core.Messages;

using Cs4rsaCommon.Enums;

using LightMessageBus;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace cs4rsa_core.Controls
{
    public partial class ScheduleBlock : UserControl, INotifyPropertyChanged
    {
        #region IsClass | Cờ lớp học
        private bool _isClass;
        public bool IsClass
        {
            get => _isClass;
            set
            {
                _isClass = value;
                _isNotClass = !value;
                OnPropertyChanged("IsClass");
            }
        }
        #endregion

        #region IsNotClass | Cờ không phải lớp học
        private bool _isNotClass;
        public bool IsNotClass
        {
            get => _isNotClass;
            set
            {
                _isNotClass = value;
                OnPropertyChanged("_isNotClass");
            }
        }
        #endregion

        #region IsTimeConflict | Hiển thị Menu Item giải quyết xung đột vị thời gian
        private bool _isTimeConflict;
        public bool IsTimeConflict
        {
            get => _isTimeConflict;
            set
            {
                _isTimeConflict = value;
                OnPropertyChanged("IsTimeConflict");
            }
        }
        #endregion

        #region IsPlaceConflict | Hiển thị Menu Item giải quyết xung đột vị vị trí
        public bool _isPlaceConflict;
        public bool IsPlaceConflict
        {
            get => _isPlaceConflict;
            set
            {
                _isPlaceConflict = value;
                OnPropertyChanged("IsPlaceConflict");
            }
        }
        #endregion

        public DayOfWeek DayOfWeek { get; set; }
        public BlockType BlockType { get; set; }

        public ScheduleBlock()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }
        #endregion

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
                "Code",
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

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }

        /// <summary>
        /// Sự kiện loại bỏ lớp đã chọn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            MessageBus.Default.Publish(new RemoveChoicedClassMessage(Code));
        }

        private void MenuItem_Remove_Class1_Click(object sender, RoutedEventArgs e)
        {
            MessageBus.Default.Publish(new RemoveChoicedClassMessage(Class1));
        }

        private void MenuItem_Remove_Class2_Click(object sender, RoutedEventArgs e)
        {
            MessageBus.Default.Publish(new RemoveChoicedClassMessage(Class2));
        }
    }
}

