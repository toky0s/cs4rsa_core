using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa.Dialogs.Extensions
{
    public static class DialogExtension
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogExtension),
                new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null && window.IsVisible)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }

        public static object GetDialogResult(Window target)
        {
            bool? bReturn = null;

            try
            {
                if (target != null)
                {
                    bReturn =
                      target.GetValue(DialogResultProperty) as bool?;
                }
            }
            catch { }

            return bReturn;
        }

        public static void SetDialogResult(Window target, bool? value)
        {
            if (target != null)
            {
                target.SetValue(DialogResultProperty, value);
            }
        }
    }
}
