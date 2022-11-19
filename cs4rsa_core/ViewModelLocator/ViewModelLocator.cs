using Microsoft.Extensions.DependencyInjection;

using System;
using System.ComponentModel;
using System.Windows;


namespace Cs4rsa.ViewModelLocator
{
    public static class ViewModelLocator
    {
        public static bool GetAutoHookedUpViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoHookedUpViewModelProperty);
        }
        public static void SetAutoHookedUpViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoHookedUpViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoHookedUpViewModel. 
        //This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoHookedUpViewModelProperty =
           DependencyProperty.RegisterAttached("AutoHookedUpViewModel",
           typeof(bool), typeof(ViewModelLocator), new
           PropertyMetadata(false, AutoHookedUpViewModelChanged));

        private static void AutoHookedUpViewModelChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            Type viewType = d.GetType();

            string str = viewType.FullName;
            if (str.Contains("UC"))
            {
                str = str
                .Replace("UC", "")
                .Replace(".DialogViews.", ".Implements.");
            }
            else
            {
                str = str.Replace(".Views.", ".ViewModels.");
            }

            string viewTypeName;
            string viewModelTypeName;
            viewTypeName = str;
            viewModelTypeName = viewTypeName + "ViewModel";
            Type viewModelType = Type.GetType(viewModelTypeName);
            object viewModel = ActivatorUtilities.GetServiceOrCreateInstance(((App)Application.Current).Container, viewModelType);
            ((FrameworkElement)d).DataContext = viewModel;
        }
    }
}
