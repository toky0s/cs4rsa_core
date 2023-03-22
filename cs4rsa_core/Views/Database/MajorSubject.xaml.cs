using Cs4rsa.BaseClasses;
using Cs4rsa.Models.Database;
using Cs4rsa.ViewModels.Database;

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;

using System.Windows;
using System.Windows.Input;

namespace Cs4rsa.Views.Database
{
    public partial class MajorSubject
    {
        private MajorSubjectViewModel viewModel;
        public MajorSubject()
        {
            InitializeComponent();
        }

        private void BaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = (MajorSubjectViewModel)DataContext;
            viewModel.LoadPlanJsonFile();
        }

        private void Card_GotoDetals_Clicked(object sender, MouseButtonEventArgs e)
        {
            Card c = (Card)sender;
            Transitioner.MoveNextCommand.Execute(null, c);
            viewModel.CurrentMajorSubject = (MajorSubjectModel)c.DataContext;
        }
    }
}
