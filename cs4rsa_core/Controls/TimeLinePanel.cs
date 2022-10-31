using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Controls
{
    internal class TimeLinePanel: Canvas
    {
        private static double _unitHeight = 0d;

        private void RenderTimeLine()
        {
            for (int i = 0; i < Utils.TIME_LINES.Length; ++i)
            {
                ScheduleTimeLine scheduleTimeLine = new();
                scheduleTimeLine.Time = Utils.TIME_LINES[i];
                scheduleTimeLine.Index = i;
                AddLogicalChild(scheduleTimeLine);
            }
        }

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    _unitHeight = availableSize.Height / Utils.TIME_LINES.Length;
        //    return base.MeasureOverride(availableSize);
        //}

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    foreach (UIElement child in InternalChildren)
        //    {
        //        if (child is ScheduleTimeLine line)
        //        {
        //            int index = line.Index;

        //            double x = 0d;
        //            double y = index * _unitHeight;

        //            double height = index * _unitHeight;
        //            Canvas.SetTop(line, height);
        //            line.Arrange(new Rect(x, y, finalSize.Width, height));
        //        }
        //    }
        //    return finalSize;
        //}
    }
}
