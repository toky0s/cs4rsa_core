using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.UI.ScheduleTable.Panels
{
    /// <summary>
    /// Một WeekPanel sẽ chứa 7 DayPanel tương ứng.
    /// MeasureOverride: Tính toán Width expected cho từng DayPanel.
    /// ArrangeOverride: Sắp xếp và tính toán lại Width của DayPanel theo hàng ngang.
    /// </summary>
    public class WeekPanel : Panel
    {
        private double _xPosition;
        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize.Width /= 7;
            _xPosition = availableSize.Width;
            foreach (ContentPresenter child in Children)
            {
                child.Measure(availableSize);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double w = finalSize.Width / 7;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Arrange(new Rect(_xPosition * i, 0, w, finalSize.Height));
            }
            return finalSize;
        }
    }
}
