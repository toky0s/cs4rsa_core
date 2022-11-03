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

        private static int GetTimeIndex(DateTime dateTime)
        {
            string[] timelines = new string[16]
            {
                "07:00",
                "09:00",
                "09:15",
                "10:15",
                "11:15",
                "12:00",
                "13:00",
                "14:00",
                "15:00",
                "15:15",
                "16:15",
                "17:15",
                "17:45",
                "18:45",
                "19:45", // SPM 300 Chăm sóc sức khoẻ cộng đồng
                "21:00"
            };

            string time = dateTime.ToString("HH:mm", CultureInfo.CurrentCulture);
            return Array.IndexOf(timelines, time);
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
                StartIndex = GetTimeIndex(timeBlock.Start),
                EndIndex = GetTimeIndex(timeBlock.End),
            };

            if (timeBlock.BlockType == BlockType.SchoolClass)
            {
                scheduleBlock.Code = timeBlock.Code;
            }
            else
            {
                scheduleBlock.Class1 = timeBlock.Class1;
                scheduleBlock.Class2 = timeBlock.Class2;
            }

            #region widthBinding | Tính toán chiều rộng
            Binding widthBinding = new()
            {
                RelativeSource = new(RelativeSourceMode.FindAncestor, typeof(Canvas), 1),
                Path = new PropertyPath(ActualWidthProperty),
                Converter = new ScheduleBlockWidthConverter()
            };
            #endregion

            #region heightMultiBinding | Tính toán chiều cao
            MultiBinding heightMultiBinding = new();
            heightMultiBinding.Converter = new ScheduleBlockHeightConverter();

            Binding bindingCnvTimelineActualHeight = new()
            {
                ElementName = "Canvas_Timelines",
                Path = new PropertyPath("ActualHeight")
            };

            Binding bindingStartIndex2 = new()
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                Path = new PropertyPath(ScheduleBlock.StartIndexProperty)
            };

            Binding bindingEndIndex = new()
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                Path = new PropertyPath(ScheduleBlock.EndIndexProperty)
            };

            heightMultiBinding.Bindings.Add(bindingCnvTimelineActualHeight);
            heightMultiBinding.Bindings.Add(bindingStartIndex2);
            heightMultiBinding.Bindings.Add(bindingEndIndex);
            #endregion

            #region canvasTopMultiBinding | Tính toán khoảng cách phía trên so với Canvas
            MultiBinding canvasTopMultiBinding = new()
            {
                Converter = new TopRangeConverter()
            };

            Binding bindingTimelineCanvasHeight = new()
            {
                ElementName = "Canvas_Timelines",
                Path = new PropertyPath("ActualHeight")
            };

            Binding bindingStartIndex = new()
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                Path = new PropertyPath(ScheduleBlock.StartIndexProperty)
            };

            canvasTopMultiBinding.Bindings.Add(bindingTimelineCanvasHeight);
            canvasTopMultiBinding.Bindings.Add(bindingStartIndex);
            #endregion

            #region Set Binding
            scheduleBlock.SetBinding(WidthProperty, widthBinding);
            scheduleBlock.SetBinding(HeightProperty, heightMultiBinding);
            scheduleBlock.SetBinding(Canvas.TopProperty, canvasTopMultiBinding);
            #endregion

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

        private void CleanCanvas(Canvas canvas)
        {
            canvas.Children.Clear();
            canvas.UpdateLayout();
        }
    }
}
