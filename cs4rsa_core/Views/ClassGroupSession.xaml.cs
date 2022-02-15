using cs4rsa_core.Models;
using cs4rsa_core.ViewModels;
using Cs4rsaDatabaseService.Models;
using SubjectCrawlService1.DataTypes.Enums;
using SubjectCrawlService1.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cs4rsa_core.Views
{
    public partial class ClassGroupSession : UserControl
    {
        public ClassGroupSession()
        {
            InitializeComponent();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }
    }
}