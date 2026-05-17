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
            int lineCount = Utils.TimeLines.Length;

            // Clamp Infinity về giá trị tính toán được
            double minRowHeight = 20d; // chiều cao tối thiểu mỗi slot giờ
            double resolvedHeight = double.IsPositiveInfinity(availableSize.Height)
                ? lineCount * minRowHeight
                : availableSize.Height;

            double resolvedWidth = double.IsPositiveInfinity(availableSize.Width)
                ? 0d  // DayPanel không biết width của mình, để WeekPanel quyết định
                : availableSize.Width;

            _unitHeight = resolvedHeight / lineCount;
            _startPoint = _unitHeight / 2;

            foreach (ContentPresenter child in Children)
            {
                TimeBlock block = (TimeBlock)child.Content;
                int start = Utils.GetTimeIndex(block.Start);
                int end = Utils.GetTimeIndex(block.End);

                double blockHeight = (end - start) * _unitHeight;
                child.Measure(new Size(resolvedWidth, blockHeight));
            }

            return new Size(resolvedWidth, resolvedHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (ContentPresenter child in Children)
            {
                TimeBlock block = (TimeBlock)child.Content;
                int start = Utils.GetTimeIndex(block.Start);
                int end = Utils.GetTimeIndex(block.End);
                double y = start * _unitHeight + _startPoint;
                double height = (end - start) * _unitHeight;
                child.Arrange(new Rect(0, y, finalSize.Width - 2, height));
            }
            return finalSize;
        }
    }
}
