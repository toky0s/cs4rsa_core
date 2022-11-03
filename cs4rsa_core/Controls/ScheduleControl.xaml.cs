using cs4rsa_core.Commons.Enums;
using cs4rsa_core.Commons.Interfaces;
using cs4rsa_core.Commons.Models;
using cs4rsa_core.Converters.Controls;
using cs4rsa_core.Services.ConflictSvc.Models;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace cs4rsa_core.Controls
{
    /// Control này đại diện cho Lịch mô phỏng, bạn cần lưu ý các ưu tố sau khi custom trên Control này.
    /// - Timelines của control này bao gồm 15 hàng tương ứng với 15 mốc thời gian
    ///   thu thập được từ các buổi học DTU, nếu có thay đổi thời gian (thêm) hãy
    ///   lưu ý sửa thêm các phần code liên quan sau:
    ///   + Các Converter liên quan đến control này.
    ///   + Các giá trị truyền vào set width height
    ///     ScheduleBlock binding với các ConverterParamters <summary>
    /// Control này đại diện cho Lịch mô phỏng, bạn cần lưu ý các ưu tố sau khi custom trên Control này.
    /// https://stackoverflow.com/questions/5913792/wpf-canvas-how-to-add-children-dynamically-with-mvvm-code-behind
    /// </summary>
    public partial class ScheduleControl : UserControl
    {
        public ScheduleControl()
        {
            InitializeComponent();
        }

        public ObservableCollection<IScheduleTableItem> BlocksSource
        {
            get { return (ObservableCollection<IScheduleTableItem>)GetValue(BlocksSourceProperty); }
            set { SetValue(BlocksSourceProperty, value); }
        }

        public static readonly DependencyProperty BlocksSourceProperty =
            DependencyProperty.Register(
                "BlocksSource",
                typeof(ObservableCollection<IScheduleTableItem>),
                typeof(ScheduleControl),
                new PropertyMetadata(OnBlocksSourceChanged)
                );

        private static void OnBlocksSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var action = new NotifyCollectionChangedEventHandler(
            (o, args) =>
            {
                ScheduleControl scheduleControl = (ScheduleControl)d;
                if (args.Action == NotifyCollectionChangedAction.Reset || args.Action == NotifyCollectionChangedAction.Remove)
                {
                    scheduleControl.CleanCanvas(scheduleControl.cnvCN);
                    scheduleControl.CleanCanvas(scheduleControl.cnvT2);
                    scheduleControl.CleanCanvas(scheduleControl.cnvT3);
                    scheduleControl.CleanCanvas(scheduleControl.cnvT4);
                    scheduleControl.CleanCanvas(scheduleControl.cnvT5);
                    scheduleControl.CleanCanvas(scheduleControl.cnvT6);
                    scheduleControl.CleanCanvas(scheduleControl.cnvT7);
                }
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    if (args.NewItems != null)
                    {
                        foreach (var item in args.NewItems)
                        {
                            IEnumerable<ScheduleBlock> scheduleBlocks;
                            if (item.GetType() == typeof(SchoolClassModel))
                            {
                                scheduleBlocks = scheduleControl.RenderScheduleBlock((SchoolClassModel)item);
                            }
                            else if (item.GetType() == typeof(ConflictModel))
                            {
                                scheduleBlocks = scheduleControl.RenderScheduleBlock((ConflictModel)item);
                            }
                            else
                            {
                                scheduleBlocks = scheduleControl.RenderScheduleBlock((PlaceConflictFinderModel)item);
                            }
                            foreach (ScheduleBlock scheduleBlock in scheduleBlocks)
                            {
                                scheduleControl.SetScheduleBlockAt(scheduleBlock, scheduleBlock.DayOfWeek);
                            }
                        }
                    }
                }
            });
            if (e.OldValue != null)
            {
                var collection = (INotifyCollectionChanged)e.OldValue;
                collection.CollectionChanged += action;
            }
            if (e.NewValue != null)
            {
                var collection = (ObservableCollection<IScheduleTableItem>)e.NewValue;
                collection.CollectionChanged += action;
            }
        }

        private void SetScheduleBlockAt(ScheduleBlock scheduleBlock, DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    cnvCN.Children.Add(scheduleBlock);
                    cnvCN.UpdateLayout();
                    break;
                case DayOfWeek.Monday:
                    cnvT2.Children.Add(scheduleBlock);
                    cnvT2.UpdateLayout();
                    break;
                case DayOfWeek.Tuesday:
                    cnvT3.Children.Add(scheduleBlock);
                    cnvT3.UpdateLayout();
                    break;
                case DayOfWeek.Wednesday:
                    cnvT4.Children.Add(scheduleBlock);
                    cnvT4.UpdateLayout();
                    break;
                case DayOfWeek.Thursday:
                    cnvT5.Children.Add(scheduleBlock);
                    cnvT5.UpdateLayout();
                    break;
                case DayOfWeek.Friday:
                    cnvT6.Children.Add(scheduleBlock);
                    cnvT6.UpdateLayout();
                    break;
                case DayOfWeek.Saturday:
                    cnvT7.Children.Add(scheduleBlock);
                    cnvT7.UpdateLayout();
                    break;
            }
        }

        private IEnumerable<ScheduleBlock> RenderScheduleBlock(IScheduleTableItem scheduleTableItem)
        {
            foreach (TimeBlock timeBlock in scheduleTableItem.GetBlocks())
            {
                yield return GetScheduleBlock(timeBlock);
            }
        }

        private ScheduleBlock GetScheduleBlock(TimeBlock timeBlock)
        {
            ScheduleBlock scheduleBlock = new()
            {
                DayOfWeek = timeBlock.DayOfWeek,
                BlockType = timeBlock.BlockType,
                BlockName = timeBlock.Content,
                BlockColor = timeBlock.Background,
                Description = timeBlock.Description,
                StartIndex = Utils.GetTimeIndex(timeBlock.Start),
                EndIndex = Utils.GetTimeIndex(timeBlock.End),
            };

            if (timeBlock.BlockType == BlockType.SchoolClass)
            {
                scheduleBlock.Code = timeBlock.ClassGroupName;
            }
            else
            {
                scheduleBlock.Class1 = timeBlock.Class1;
                scheduleBlock.Class2 = timeBlock.Class2;
            }

            #region Events | Đặt hai sự kiện rê chuột
            scheduleBlock.MouseEnter += ScheduleBlock_MouseEnter;
            scheduleBlock.MouseLeave += ScheduleBlock_MouseLeave;
            #endregion

            return scheduleBlock;
        }

        private void ScheduleBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            /** 
             * Set mấy line vừa tô về màu cũ khi hover out
             * Hai line dư lúc vẽ bảng
             */
            int UNEXPECTED_LINE_QTY = 2;

            ScheduleBlock scheduleBlock = (ScheduleBlock)sender;

            List<Line> lines = Canvas_Timelines.Children.OfType<Line>().ToList();
            Line startLine = lines[scheduleBlock.StartIndex + UNEXPECTED_LINE_QTY];
            Line endLine = lines[scheduleBlock.EndIndex + UNEXPECTED_LINE_QTY];

            var converter = new BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#dfe6e9");
            startLine.Stroke = brush;
            endLine.Stroke = brush;

            List<TextBlock> textBlocks = Canvas_Timelines.Children.OfType<TextBlock>().ToList();

            TextBlock startTextBlock = textBlocks[scheduleBlock.StartIndex];
            TextBlock endTextBlock = textBlocks[scheduleBlock.EndIndex];

            startTextBlock.FontWeight = FontWeights.Regular;
            endTextBlock.FontWeight = FontWeights.Regular;
            startTextBlock.FontSize = 10;
            endTextBlock.FontSize = 10;

            // Khôi phục line về stroke ban đầu
            for (int i = 2; i < lines.Count; i++)
            {
                if (i != scheduleBlock.StartIndex + UNEXPECTED_LINE_QTY || i != scheduleBlock.EndIndex + UNEXPECTED_LINE_QTY)
                {
                    lines[i].Stroke = (Brush)converter.ConvertFromString("#f1f2f6");
                }
            }
            // tương tự với textblock
            for (int i = 0; i < textBlocks.Count; i++)
            {
                if (i != scheduleBlock.StartIndex || i != scheduleBlock.EndIndex)
                {
                    textBlocks[i].Foreground = (Brush)converter.ConvertFromString("Black");
                }
            }
        }

        private void ScheduleBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            // Hai line dư lúc vẽ bảng
            int UNEXPECTED_LINE_QTY = 2;

            ScheduleBlock scheduleBlock = (ScheduleBlock)sender;

            List<Line> lines = Canvas_Timelines.Children.OfType<Line>().ToList();
            Line startLine = lines[scheduleBlock.StartIndex + UNEXPECTED_LINE_QTY];
            Line endLine = lines[scheduleBlock.EndIndex + UNEXPECTED_LINE_QTY];


            var converter = new BrushConverter();
            /**
             * Hạ tone màu của các line còn lại để làm nổi bật
             * line của block đang hover
             */
            for (int i = 2; i < lines.Count; i++)
            {
                if (i != scheduleBlock.StartIndex + UNEXPECTED_LINE_QTY || i != scheduleBlock.EndIndex + UNEXPECTED_LINE_QTY)
                {
                    lines[i].Stroke = (Brush)converter.ConvertFromString("#f7f8fc");
                }
            }

            var brush = (Brush)converter.ConvertFromString("#a5abad");
            startLine.Stroke = brush;
            endLine.Stroke = brush;

            List<TextBlock> textBlocks = Canvas_Timelines.Children.OfType<TextBlock>().ToList();

            // Tương tự line, các textblock cũng sẽ được hạ tone màu.
            for (int i = 0; i < textBlocks.Count; i++)
            {
                if (i != scheduleBlock.StartIndex || i != scheduleBlock.EndIndex)
                {
                    textBlocks[i].Foreground = (Brush)converter.ConvertFromString("#b7bec7");
                }
            }
            TextBlock startTextBlock = textBlocks[scheduleBlock.StartIndex];
            TextBlock endTextBlock = textBlocks[scheduleBlock.EndIndex];
            startTextBlock.Foreground = (Brush)converter.ConvertFromString("Black");
            endTextBlock.Foreground = (Brush)converter.ConvertFromString("Black");
        }

        private void CleanCanvas(Panel canvas)
        {
            canvas.Children.Clear();
            canvas.UpdateLayout();
        }
    }
}
