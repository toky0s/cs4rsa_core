using Cs4rsa.BaseClasses;

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

namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class MyProgramUC : UserControl, IDialog
    {
        public MyProgramUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway() => true;
    }
}
