using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Controls
{
    public class DayPanel: Canvas
    {
        private static double _unitHeight = 0d;

        protected override Size MeasureOverride(Size availableSize)
        {
            _unitHeight = availableSize.Height / Utils.TIME_LINES.Length;
            return base.MeasureOverride(availableSize);
        }
        
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                if (child is ScheduleBlock block)
                {
                    int start = block.StartIndex;
                    int end = block.EndIndex;

                    double x = 0d;
                    double y = start * _unitHeight;

                    double height = (end - start) * _unitHeight;
                    block.Arrange(new Rect(x, y, finalSize.Width, height));
                }
            }
            return finalSize;
        }
    }
}
