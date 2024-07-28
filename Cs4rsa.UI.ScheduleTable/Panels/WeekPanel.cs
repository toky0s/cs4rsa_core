using System;
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
        protected override Size MeasureOverride(Size availableSize)
        {
            double totalWidth = 0;
            double totalHeight = 0;

            foreach (UIElement child in InternalChildren)
            {
                // Measure each child with available size
                child.Measure(availableSize);

                // Get the desired size of the child
                Size desiredSize = child.DesiredSize;

                // Accumulate the total width and height
                totalWidth += desiredSize.Width;
                totalHeight = Math.Max(totalHeight, desiredSize.Height);
            }

            // Ensure that the returned size is not infinite
            totalWidth = double.IsPositiveInfinity(totalWidth) ? availableSize.Width : totalWidth;
            totalHeight = double.IsPositiveInfinity(totalHeight) ? availableSize.Height : totalHeight;

            // Return the total desired size
            return new Size(totalWidth, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double childWidth = finalSize.Width / 7; // Divide the total width by 7 for 7 children
            double childHeight = finalSize.Height; // Use the full height

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                UIElement child = InternalChildren[i];
                if (child != null)
                {
                    double x = i * childWidth;
                    double y = 0;
                    Rect rect = new Rect(x, y, childWidth, childHeight);
                    child.Arrange(rect);
                }
            }

            // Return the final arranged size
            return finalSize;
        }
    }
}
