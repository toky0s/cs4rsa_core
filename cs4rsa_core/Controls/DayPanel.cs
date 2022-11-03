using cs4rsa_core.Commons.Models;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace cs4rsa_core.Controls
{
    public class DayPanel: Panel
    {
        private static double _unitHeight;
        private static double _startPoint;
        
        protected override Size MeasureOverride(Size availableSize)
        {
            _unitHeight = availableSize.Height / Utils.TIME_LINES.Length;
            _startPoint = _unitHeight / 2;
            foreach (ContentPresenter child in Children)
            {
                TimeBlock block = (TimeBlock)child.Content;
                int start = Utils.GetTimeIndex(block.Start);
                int end = Utils.GetTimeIndex(block.End);
                availableSize.Height = (end - start) * _unitHeight;
                child.Measure(availableSize);
            }

            return availableSize;
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
