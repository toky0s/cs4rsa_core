using cs4rsa_core.BaseClasses;
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

namespace cs4rsa_core.Dialogs.DialogViews
{
    public partial class DonateUC : UserControl, IDialog
    {
        public DonateUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return true;
        }
    }
}
