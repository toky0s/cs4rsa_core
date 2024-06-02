using Cs4rsa.UI.ScheduleTable.Models;

using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.UI.ScheduleTable.Panels
{
    public class DayPanel : Panel
    {
        private double _unitHeight;
        private double _startPoint;

        protected override Size MeasureOverride(Size availableSize)
        {
            double height = availableSize.Height;
            double width = availableSize.Width / 7;

            _unitHeight = availableSize.Height / Utils.Utils.TimeLines.Length;
            _startPoint = _unitHeight / 2;
            foreach (ContentPresenter child in Children)
            {
                TimeBlock block = (TimeBlock)child.Content;
                int start = Utils.Utils.GetTimeIndex(block.Start);
                int end = Utils.Utils.GetTimeIndex(block.End);
                availableSize.Height = (end - start) * _unitHeight;
                child.Measure(availableSize);
            }

            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (ContentPresenter child in Children)
            {
                TimeBlock block = (TimeBlock)child.Content;
                int start = Utils.Utils.GetTimeIndex(block.Start);
                int end = Utils.Utils.GetTimeIndex(block.End);
                double y = start * _unitHeight + _startPoint;
                double height = (end - start) * _unitHeight;
                child.Arrange(new Rect(0, y, finalSize.Width - 2, height));
            }
            return finalSize;
        }
    }
}
