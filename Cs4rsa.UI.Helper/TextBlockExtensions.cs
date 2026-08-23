using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cs4rsa.UI.Helper
{
    public static class TextBlockExtensions
    {
        public static readonly DependencyProperty HighlightedTextProperty =
            DependencyProperty.RegisterAttached(
                "HighlightedText",
                typeof(string),
                typeof(TextBlockExtensions),
                new PropertyMetadata(null, OnHighlightedTextChanged));

        public static string GetHighlightedText(DependencyObject obj) =>
            (string)obj.GetValue(HighlightedTextProperty);

        public static void SetHighlightedText(DependencyObject obj, string value) =>
            obj.SetValue(HighlightedTextProperty, value);

        private static void OnHighlightedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d.GetType() != typeof(TextBlock)) return;
            var textBlock = (TextBlock)d;

            textBlock.Inlines.Clear();

            var text = e.NewValue as string;
            if (string.IsNullOrEmpty(text)) return;

            string[] parts = text.Split(new[] { "<b>", "</b>" }, StringSplitOptions.None);
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1)
                    textBlock.Inlines.Add(new Run(parts[i]) { FontWeight = FontWeights.Bold });
                else
                    textBlock.Inlines.Add(new Run(parts[i]));
            }
        }
    }
}