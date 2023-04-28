using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

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

namespace Cs4rsa.Controls
{
    public partial class DtuTimeBlock : UserControl
    {
        public DtuTimeBlock()
        {
            InitializeComponent();
        }

        public SchoolClassUnit SchoolClassUnit
        {
            get { return (SchoolClassUnit)GetValue(SchoolClassUnitProperty); }
            set { SetValue(SchoolClassUnitProperty, value); }
        }

        public static readonly DependencyProperty SchoolClassUnitProperty =
            DependencyProperty.Register(
                "SchoolClassUnit"
                , typeof(SchoolClassUnit)
                , typeof(DtuTimeBlock)
                , new FrameworkPropertyMetadata(null));
    }
}
