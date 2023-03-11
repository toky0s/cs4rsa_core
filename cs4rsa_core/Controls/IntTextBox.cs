using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cs4rsa.Controls
{
    public class IntTextBox : TextBox
    {
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            // Chặn trường hợp xoá mà chỉ còn một ký tự.
            if (e.Key == Key.Back && Text.Length == 1)
            {
                e.Handled = true;
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            e.Handled = !IsTextAllowed(e.Text);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            bool isValidNumber = int.TryParse(Text, out int value);
            // Trường hợp rỗng thì lấy Min, nếu Min chưa được set thì lấy 0
            if (
                string.IsNullOrEmpty(Text.Trim())
                || !isValidNumber
                || (value < Min && Min != 0)
            )
            {
                Text = Min.ToString() ?? "0";
            }

            // Trường hợp vượt Max.
            else if (value > Max && Max != 0)
            {
                Text = Max.ToString() ?? "0";
            }
        }

        /// <summary>
        /// Regex that matches disallowed text.
        /// </summary>
        private static readonly Regex _regex = new("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(int), typeof(IntTextBox), new PropertyMetadata(0));


        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(IntTextBox), new PropertyMetadata(0));

    }
}
