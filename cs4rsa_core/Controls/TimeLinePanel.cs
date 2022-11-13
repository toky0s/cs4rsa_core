using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Controls
{
    internal class TimeLinePanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double _unitHeight = finalSize.Height / Utils.TIME_LINES.Length;
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
