using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cs4rsa.Controls
{
    public class IntTextBox : TextBox
    {
        private const string DefaultValue = "0";
        public IntTextBox() : base()
        {
            Loaded += IntTextBox_Loaded;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            int crrVal = string.IsNullOrWhiteSpace(Text) ? 0 : int.Parse(Text);
            int nextVal = 0;
            if (e.Delta >= 120)
            {
                nextVal = crrVal + 1;
            }
            else if (e.Delta < 0 && crrVal > 1)
            {
                nextVal = crrVal - 1;
            }
            Text = nextVal.ToString();
            e.Handled = true;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            // Trường hợp còn một ký tự, người dùng nhấn nút BackSpace
            // tự động set về Min hoặc Default.
            if (e.Key == Key.Back && Text.Length == 1)
            {
                Text = Min.ToString() ?? DefaultValue;
                // Đưa con trỏ về cuối
                CaretIndex = Text.Length;
            }
        }

        private void IntTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            bool isValidNumber = int.TryParse(Text, out int value);
            // Trường hợp rỗng thì lấy Min, nếu Min chưa được set thì lấy 0
            if (
                string.IsNullOrEmpty(Text.Trim())
                || !isValidNumber
                || (value < Min && Min != 0)
            )
            {
                Text = Min.ToString() ?? DefaultValue;
            }

            // Trường hợp vượt Max.
            else if (value > Max && Max != 0)
            {
                Text = Max.ToString() ?? DefaultValue;
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            if (string.IsNullOrWhiteSpace(e.Text)) Text = Min.ToString() ?? DefaultValue;
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
                Text = Min.ToString() ?? DefaultValue;
            }

            // Trường hợp vượt Max.
            else if (value > Max && Max != 0)
            {
                Text = Max.ToString() ?? DefaultValue;
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

        /// <summary>
        /// Giá trị tối thiểu
        /// </summary>
        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(int), typeof(IntTextBox), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        /// <summary>
        /// Giá trị tối đa
        /// </summary>
        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(IntTextBox), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    }
}
