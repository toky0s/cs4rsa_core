using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Cs4rsa.WPF.Converter
{
    public class Base64ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() != typeof(string) || ((string)value).Length == 0)
            {
                return null;
            }
            try
            {
                var s = (string)value;
                BitmapImage bi = new BitmapImage();
                Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
                s = regex.Replace(s, string.Empty);
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(System.Convert.FromBase64String(s));
                bi.EndInit();
                return bi;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
