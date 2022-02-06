using cs4rsa_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs4rsa_core.Controls
{
    public partial class ScheduleBlock : UserControl
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeBlock TimeBlock { get; set; }
        public ScheduleBlock()
        {
            InitializeComponent();
        }

        #region propdp BlockColor
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

        #region propdp Description
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

        #region propdp StartIndex
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

        #region propdp EndIndex
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
    }
}
