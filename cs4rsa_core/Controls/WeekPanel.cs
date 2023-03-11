using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Controls
{
    /// <summary>
    /// Một WeekPanel sẽ chứa 7 DayPanel tương ứng.
    /// MeasureOverride: Tính toán Width expected cho từng DayPanel.
    /// ArrangeOverride: Sắp xếp và tính toán lại Width của DayPanel theo hàng ngang.
    /// </summary>
    public class WeekPanel : Panel
    {
        private const int DAY_OF_WEEK_COUNT = 7;
        private static double xPosition;
        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = double.MaxValue;
            }
            availableSize.Width /= DAY_OF_WEEK_COUNT;
            xPosition = availableSize.Width;
            foreach (ContentPresenter child in Children)
            {
                child.Measure(availableSize);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double w = finalSize.Width / DAY_OF_WEEK_COUNT;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Arrange(new Rect(xPosition * i, 0, w, finalSize.Height));
            }
            return finalSize;
        }
    }
}
