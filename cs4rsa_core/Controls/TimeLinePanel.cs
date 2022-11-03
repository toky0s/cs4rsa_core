using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace cs4rsa_core.Controls
{
    internal class TimeLinePanel: Panel
    {
        private static double _unitHeight = 0d;

        protected override Size MeasureOverride(Size availableSize)
        {
            _unitHeight = availableSize.Height / Utils.TIME_LINES.Length;
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
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
