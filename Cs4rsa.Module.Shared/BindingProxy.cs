using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.Module.Shared
{
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy));

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
}
