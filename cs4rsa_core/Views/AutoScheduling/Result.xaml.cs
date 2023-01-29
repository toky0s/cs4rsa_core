using System.Text.RegularExpressions;
using System.Windows.Controls;

using static System.Net.Mime.MediaTypeNames;

namespace Cs4rsa.Views.AutoScheduling
{
    public partial class Result : UserControl
    {
        public Result()
        {
            InitializeComponent();
        }

        private void TextBox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int crrVal = string.IsNullOrEmpty(tb.Text) ? 0 : int.Parse(tb.Text);
            int nextVal = 0;
            if (e.Delta >= 120)
            {
                nextVal = crrVal + 1;
            }
            else if (e.Delta < 0 && crrVal > 1)
            {
                nextVal = crrVal - 1;
            }
            tb.Text = nextVal.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = Regex.Replace(((TextBox)sender).Text, @"[^\d.]", "");
            ((TextBox)sender).Text = s;
        }
    }
}
