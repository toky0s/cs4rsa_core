﻿using System;
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
using cs4rsa.ViewModels;

namespace cs4rsa.Views
{
    /// <summary>
    /// Interaction logic for ChoicedSession.xaml
    /// </summary>
    public partial class ChoicedSession : UserControl
    {
        private ChoiceSessionViewModel ChoiceSessionViewModel = new ChoiceSessionViewModel();
        public ChoicedSession()
        {
            DataContext = ChoiceSessionViewModel;
            InitializeComponent();
        }
    }
}
