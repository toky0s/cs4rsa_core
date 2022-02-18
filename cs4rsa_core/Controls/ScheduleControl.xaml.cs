using ConflictService.DataTypes;
using cs4rsa_core.Converters.Controls;
using cs4rsa_core.Models;
using cs4rsa_core.Models.Interfaces;
using SubjectCrawlService1.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    ///     ScheduleBlock binding với các ConverterParamters
    public partial class ScheduleControl : UserControl
    {
        public ScheduleControl()
        {
            InitializeComponent();
        }

        public ObservableCollection<ICanShowOnScheduleTable> BlocksSource
        {
            get { return (ObservableCollection<ICanShowOnScheduleTable>)GetValue(BlocksSourceProperty); }
            set { SetValue(BlocksSourceProperty, value); }
        }

        public static readonly DependencyProperty BlocksSourceProperty =
            DependencyProperty.Register(
                "BlocksSource",
                typeof(ObservableCollection<ICanShowOnScheduleTable>),
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
                            List<ScheduleBlock> scheduleBlocks;
                            if (item.GetType() == typeof(SchoolClassModel))
                            {
                                scheduleBlocks = scheduleControl.RenderScheduleBlock((SchoolClassModel)item);
                            }
                            else
                            {
                                scheduleBlocks = scheduleControl.RenderScheduleBlock((ConflictModel)item);
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
                var collection = (ObservableCollection<ICanShowOnScheduleTable>)e.NewValue;
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

        private List<ScheduleBlock> RenderScheduleBlock(ConflictModel conflictModel)
        {
            string CONFLICT_BLOCK_COLOR = "#e74c3c";
            List<ScheduleBlock> scheduleBlocks = new();
            foreach (KeyValuePair<DayOfWeek, List<StudyTimeIntersect>> item in conflictModel.ConflictTime.ConflictTimes)
            {
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    string description = conflictModel.GetFullConflictInfo();
                    int startIndex = GetTimeIndex(studyTimeIntersect.Start);
                    int endIndex = GetTimeIndex(studyTimeIntersect.End);
                    ScheduleBlock scheduleBlock = GetScheduleBlock(CONFLICT_BLOCK_COLOR, startIndex, endIndex, description);
                    scheduleBlock.DayOfWeek = item.Key;
                    scheduleBlocks.Add(scheduleBlock);
                }
            }
            return scheduleBlocks;
        }

        private List<ScheduleBlock> RenderScheduleBlock(SchoolClassModel schoolClassModel)
        {
            List<ScheduleBlock> scheduleBlocks = new();
            foreach (KeyValuePair<DayOfWeek, List<StudyTime>> item in schoolClassModel.Schedule.ScheduleTime)
            {
                foreach (StudyTime studyTime in item.Value)
                {
                    string description = schoolClassModel.SchoolClassName + " | " + schoolClassModel.SubjectName + "\n" + schoolClassModel.Schedule.ToString();
                    int startIndex = GetTimeIndex(studyTime.Start);
                    int endIndex = GetTimeIndex(studyTime.End);
                    ScheduleBlock scheduleBlock = GetScheduleBlock(schoolClassModel.Color, startIndex, endIndex, description);
                    scheduleBlock.DayOfWeek = item.Key;
                    scheduleBlock.SchoolClassModel = schoolClassModel;
                    scheduleBlocks.Add(scheduleBlock);
                }
            }
            return scheduleBlocks;
        }

        private static int GetTimeIndex(DateTime dateTime)
        {
            string[] timelines = new string[15]
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
                "21:00"
            };

            string time = dateTime.ToString("HH:mm");
            return Array.IndexOf(timelines, time);
        }

        private ScheduleBlock GetScheduleBlock(
            string blockColor, int startIndex, int endIndex,
            string description)
        {
            ScheduleBlock scheduleBlock = new();
            scheduleBlock.BlockColor = blockColor;
            scheduleBlock.StartIndex = startIndex;
            scheduleBlock.EndIndex = endIndex;
            scheduleBlock.Description = description;

            #region widthBinding
            Binding widthBinding = new();
            widthBinding.RelativeSource = new(RelativeSourceMode.FindAncestor, typeof(Canvas), 1);
            widthBinding.Path = new PropertyPath(ActualWidthProperty);
            widthBinding.Converter = new ScheduleBlockWidthConverter();
            #endregion


            #region heightMultiBinding
            MultiBinding heightMultiBinding = new();
            heightMultiBinding.Converter = new ScheduleBlockHeightConverter();

            Binding bindingCnvTimelineActualHeight = new();
            bindingCnvTimelineActualHeight.ElementName = "Canvas_Timelines";
            bindingCnvTimelineActualHeight.Path = new PropertyPath("ActualHeight");

            Binding bindingStartIndex2 = new();
            bindingStartIndex2.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            bindingStartIndex2.Path = new PropertyPath(ScheduleBlock.StartIndexProperty);

            Binding bindingEndIndex = new();
            bindingEndIndex.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            bindingEndIndex.Path = new PropertyPath(ScheduleBlock.EndIndexProperty);

            heightMultiBinding.Bindings.Add(bindingCnvTimelineActualHeight);
            heightMultiBinding.Bindings.Add(bindingStartIndex2);
            heightMultiBinding.Bindings.Add(bindingEndIndex);
            #endregion


            #region canvasTopMultiBinding
            MultiBinding canvasTopMultiBinding = new();
            canvasTopMultiBinding.Converter = new TopRangeConverter();

            Binding bindingTimelineCanvasHeight = new();
            bindingTimelineCanvasHeight.ElementName = "Canvas_Timelines";
            bindingTimelineCanvasHeight.Path = new PropertyPath("ActualHeight");

            Binding bindingStartIndex = new();
            bindingStartIndex.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            bindingStartIndex.Path = new PropertyPath(ScheduleBlock.StartIndexProperty);

            canvasTopMultiBinding.Bindings.Add(bindingTimelineCanvasHeight);
            canvasTopMultiBinding.Bindings.Add(bindingStartIndex);
            #endregion

            scheduleBlock.SetBinding(Canvas.WidthProperty, widthBinding);
            scheduleBlock.SetBinding(Canvas.HeightProperty, heightMultiBinding);
            scheduleBlock.SetBinding(Canvas.TopProperty, canvasTopMultiBinding);

            #region Events
            scheduleBlock.MouseEnter += ScheduleBlock_MouseEnter;
            scheduleBlock.MouseLeave += ScheduleBlock_MouseLeave;
            #endregion

            return scheduleBlock;
        }

        private void ScheduleBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            // Set mấy line vừa tô về màu cũ khi hover out

            // Hai line dư lúc vẽ bảng
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
            // Hạ tone màu của các line còn lại để làm nổi bật
            // line của block đang hover
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
