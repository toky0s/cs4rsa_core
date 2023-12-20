using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.UI.ScheduleTable.Panels
{
    public class TimeLinePanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double _unitHeight = finalSize.Height / Utils.Utils.TimeLines.Length;
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                double x = 0;
                double y = i * _unitHeight;

                InternalChildren[i].Arrange(new Rect(x, y, finalSize.Width, _unitHeight));
            }
            return finalSize;
        }
    }
}
