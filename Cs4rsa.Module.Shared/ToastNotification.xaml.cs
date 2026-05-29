using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Cs4rsa.Module.Shared
{
    public enum ToastType { Info, Success, Warning, Error }

    public partial class ToastNotification : Window
    {
        public ToastNotification(string title, string message, ToastType type = ToastType.Info)
        {
            InitializeComponent();
            TitleText.Text = title;
            MessageText.Text = message;

            MessageText.Visibility = string.IsNullOrWhiteSpace(message)
                ? Visibility.Collapsed
                : Visibility.Visible;

            string icon;
            string accentHex;

            switch (type)
            {
                case ToastType.Success:
                    icon = "✅"; accentHex = "#4CAF50"; break;
                case ToastType.Warning:
                    icon = "⚠️"; accentHex = "#FF9800"; break;
                case ToastType.Error:
                    icon = "❌"; accentHex = "#F44336"; break;
                default:
                    icon = "ℹ️"; accentHex = "#2196F3"; break;
            }

            IconText.Text = icon;

            var border = (System.Windows.Controls.Border)Content;
            border.BorderBrush = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(accentHex));
            border.BorderThickness = new Thickness(3, 0, 0, 0);
        }

        public void BeginShowSequence(int displayMs = 2000)
        {
            var fadeIn = (Storyboard)Resources["FadeIn"];
            var fadeOut = (Storyboard)Resources["FadeOut"];

            fadeOut.Completed += (s, e) => Close();

            fadeIn.Completed += (s, e) =>
            {
                var hold = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(displayMs)
                };
                hold.Tick += (ts, te) =>
                {
                    hold.Stop();
                    fadeOut.Begin(this);
                };
                hold.Start();
            };

            Show();
            fadeIn.Begin(this);
        }
    }
}