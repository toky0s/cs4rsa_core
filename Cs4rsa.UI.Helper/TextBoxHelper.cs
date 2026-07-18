using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace Cs4rsa.UI.Helper
{
    public static class TextBoxHelper
    {
        public static string GetPlaceholder(DependencyObject obj) =>
            (string)obj.GetValue(PlaceholderProperty);

        public static void SetPlaceholder(DependencyObject obj, string value) =>
            obj.SetValue(PlaceholderProperty, value);

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(string),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(
                    defaultValue: null,
                    propertyChangedCallback: OnPlaceholderChanged)
                );

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBox textBox)) return;

            textBox.TextChanged -= TextBoxControl_TextChanged;
            textBox.TextChanged += TextBoxControl_TextChanged;

            textBox.Loaded -= TextBoxControl_Loaded;
            textBox.Loaded += TextBoxControl_Loaded;
            textBox.Unloaded -= TextBoxControl_Unloaded;
            textBox.Unloaded += TextBoxControl_Unloaded;

            textBox.IsVisibleChanged -= TextBoxControl_IsVisibleChanged;
            textBox.IsVisibleChanged += TextBoxControl_IsVisibleChanged;

            RefreshAdorner(textBox);
        }

        private static void TextBoxControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBox);

            if (layer == null)
                return;

            var adorners = layer.GetAdorners(textBox);

            if (adorners == null)
                return;

            foreach (var adorner in adorners.OfType<PlaceholderAdorner>())
            {
                layer.Remove(adorner);
            }
        }

        private static void TextBoxControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            if (textBox.IsVisible)
            {
                RefreshAdorner(textBox);
            }
            else
            {
                // TextBox vừa bị ẩn (ví dụ do Expander cha bị collapse) -> ẩn luôn placeholder adorner
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBox);
                if (layer == null) return;

                var adorners = layer.GetAdorners(textBox);
                if (adorners == null) return;

                foreach (var adorner in adorners.OfType<PlaceholderAdorner>())
                {
                    adorner.Visibility = Visibility.Hidden;
                }
            }
        }

        // Helper mới: tạo adorner và set visibility đúng ngay từ đầu
        private static void RefreshAdorner(TextBox textBox)
        {
            if (!textBox.IsLoaded)
                return;

            textBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!GetOrCreateAdorner(textBox, out PlaceholderAdorner adorner))
                    return;

                adorner.Visibility = string.IsNullOrEmpty(textBox.Text)
                    ? Visibility.Visible
                    : Visibility.Hidden;

                adorner.InvalidateVisual();
            }),
            DispatcherPriority.Render);
        }

        private static void TextBoxControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            RefreshAdorner(textBox);
        }

        private static void TextBoxControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            if (!GetOrCreateAdorner(textBox, out PlaceholderAdorner adorner)) return;

            adorner.Visibility = textBox.Text.Length > 0
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        private static bool GetOrCreateAdorner(TextBox textBoxControl, out PlaceholderAdorner adorner)
        {
            // Get the adorner layer
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBoxControl);

            // If null, it doesn't exist or the control's template isn't loaded
            if (layer == null)
            {
                adorner = null;
                return false;
            }

            // Layer exists, try to find the adorner
            adorner = layer.GetAdorners(textBoxControl)?.OfType<PlaceholderAdorner>().FirstOrDefault();

            // Adorner never added to control, so add it
            if (adorner == null)
            {
                adorner = new PlaceholderAdorner(textBoxControl);
                layer.Add(adorner);
            }

            return true;
        }

        public class PlaceholderAdorner : Adorner
        {
            public PlaceholderAdorner(TextBox textBox) : base(textBox)
            {
                IsHitTestVisible = false;    // <--- cho phép click xuyên qua
                Focusable = false;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                TextBox textBoxControl = (TextBox)AdornedElement;

                string placeholderValue = TextBoxHelper.GetPlaceholder(textBoxControl);

                if (string.IsNullOrEmpty(placeholderValue))
                    return;

                // Create the formatted text object
                FormattedText text = new FormattedText(
                                            placeholderValue,
                                            System.Globalization.CultureInfo.CurrentCulture,
                                            textBoxControl.FlowDirection,
                                            new Typeface(textBoxControl.FontFamily,
                                                         textBoxControl.FontStyle,
                                                         textBoxControl.FontWeight,
                                                         textBoxControl.FontStretch),
                                            textBoxControl.FontSize,
                                            SystemColors.InactiveCaptionBrush,
                                            VisualTreeHelper.GetDpi(textBoxControl).PixelsPerDip);

                text.MaxTextWidth = System.Math.Max(textBoxControl.ActualWidth - textBoxControl.Padding.Left - textBoxControl.Padding.Right, 10);
                text.MaxTextHeight = System.Math.Max(textBoxControl.ActualHeight, 10);

                // Render based on padding of the control, to try and match where the textbox places text
                Point renderingOffset = new Point(textBoxControl.Padding.Left, textBoxControl.Padding.Top);

                // Template contains the content part; adjust sizes to try and align the text
                if (textBoxControl.Template.FindName("PART_ContentHost", textBoxControl) is FrameworkElement part)
                {
                    Point partPosition = part.TransformToAncestor(textBoxControl).Transform(new Point(0, 0));
                    renderingOffset.X += partPosition.X;
                    renderingOffset.Y += partPosition.Y;

                    text.MaxTextWidth = System.Math.Max(part.ActualWidth - renderingOffset.X, 10);
                    text.MaxTextHeight = System.Math.Max(part.ActualHeight, 10);
                }

                // Draw the text
                drawingContext.DrawText(text, renderingOffset);
            }
        }
    }
}
