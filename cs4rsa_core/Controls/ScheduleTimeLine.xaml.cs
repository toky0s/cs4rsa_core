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
    public partial class ScheduleTimeLine : UserControl
    {
        public ScheduleTimeLine()
        {
            InitializeComponent();
        }

        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(string), typeof(ScheduleTimeLine));



        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register(
                "Index", 
                typeof(int), 
                typeof(ScheduleTimeLine));

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            LineControl.X1 = 0;
            LineControl.X2 = arrangeBounds.Width;
            return arrangeBounds;
        }
    }
}
