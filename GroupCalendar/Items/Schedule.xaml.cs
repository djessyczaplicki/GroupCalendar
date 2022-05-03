using GroupCalendar.Data.Remote.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GroupCalendar.Items
{
    /// <summary>
    /// Lógica de interacción para Schedule.xaml
    /// </summary>
    public partial class Schedule : UserControl
    {

        public DateTimeOffset FirstDay
        {
            get { return (DateTimeOffset)GetValue(FirstDayProperty); }
            set { SetValue(FirstDayProperty, value); }
        }
        public static readonly DependencyProperty FirstDayProperty =
           DependencyProperty.Register("FirstDay", typeof(DateTimeOffset), typeof(Schedule), new PropertyMetadata(DateTimeOffset.Now));



        public int DaysToShow = 7;
        readonly int hourHeight = 80;
        double eventWidth;
        List<Border> horizontalLines = new List<Border>();
        List<Border> verticalLines = new List<Border>();
        private List<BasicEvent> basicEvents = new List<BasicEvent>();

        public ObservableCollection<EventModel> EventModels
        {
            get { return (ObservableCollection<EventModel>)GetValue(EventModelsProperty); }
            set { SetValue(EventModelsProperty, value); }
        }
        public static readonly DependencyProperty EventModelsProperty =
            DependencyProperty.Register("EventModels", typeof(ObservableCollection<EventModel>), typeof(Schedule), new PropertyMetadata(new ObservableCollection<EventModel>()));

        public ObservableCollection<EventModel> events
        {
            get; set;
        }

        public Schedule()
        {
            InitializeComponent();

        }

        private void ScheduleLoaded(object sender, RoutedEventArgs e)
        {
            //events = ((TimetableViewModel)(DataContext)).EventsToShow;
            eventWidth = Canvas.ActualWidth / DaysToShow;
            Height = hourHeight * 24;
            for (int hour = 1; hour < 24; hour++)
            {
                var drawnLine = DrawHorizontalLine(hour);
                Canvas.Children.Add(drawnLine);
            }

            var now = DateTimeOffset.Now;
            var nowLine = DrawHorizontalLine(now.Hour + now.Minute / 60d);
            nowLine.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00FFFF");
            nowLine.Height = 2;
            Canvas.Children.Add(nowLine);

            for (int day = 1; day < DaysToShow; day++)
            {
                var drawnLine = DrawVerticalLine(day);
                Canvas.Children.Add(drawnLine);
            }


            foreach (EventModel eventModel in EventModels)
            {
                var drawnEvent = DrawEvent(eventModel);
                Canvas.Children.Add(drawnEvent);
            }
            EventModels.CollectionChanged += EventModels_CollectionChanged;
        }

        private void EventModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshUi();
        }

        private void RefreshUi()
        {
            basicEvents.ForEach(e => Canvas.Children.Remove(e));
            basicEvents.Clear();
            foreach (EventModel eventModel in EventModels)
            {
                var drawnEvent = DrawEvent(eventModel);
                Canvas.Children.Add(drawnEvent);
            }
            if (StaticResources.basicDayHeader != null)
            {
                StaticResources.basicDayHeader.RefreshUi();
            }
        }

        private Border DrawVerticalLine(int day)
        {
            Border border = new Border();
            border.Width = 1;
            border.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#DEDEDE");
            border.Height = Height;
            Canvas.SetLeft(border, day * eventWidth);
            verticalLines.Add(border);
            return border;
        }

        private Border DrawHorizontalLine(double hour)
        {
            Border border = new Border();
            border.Width = Canvas.ActualWidth;
            border.Height = 1;
            border.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#DEDEDE");
            Canvas.SetTop(border, hourHeight * hour);
            horizontalLines.Add(border);
            return border;
        }

        private BasicEvent DrawEvent(EventModel eventModel)
        {
            var basicEvent = new BasicEvent(eventModel);
            var minuteHeight = hourHeight / 60;
            var minEventHeight = hourHeight / 4;
            var eventTop = eventModel.Start.Hour * hourHeight + eventModel.Start.Minute * minuteHeight;
            var eventLeft = GetEventLeft(eventModel, eventWidth);
            var eventHeight = Math.Max((eventModel.End.Hour - eventModel.Start.Hour) * hourHeight + (eventModel.End.Minute - eventModel.Start.Minute) * minuteHeight, minEventHeight);
            Canvas.SetLeft(basicEvent, eventLeft);
            Canvas.SetTop(basicEvent, eventTop);
            basicEvent.Width = eventWidth;
            basicEvent.Height = eventHeight;
            basicEvents.Add(basicEvent);
            return basicEvent;
        }

        private double GetEventLeft(EventModel eventModel, double eventWidth)
        {
            return (eventModel.Start.Date - FirstDay.Date).Days * eventWidth;
        }

        private void ScheduleSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            RefreshUi();
            eventWidth = e.NewSize.Width / DaysToShow;
            foreach (BasicEvent basicEvent in basicEvents)
            {
                if (basicEvent != null)
                {
                    var eventLeft = GetEventLeft(basicEvent.EventModel, eventWidth);
                    Canvas.SetLeft(basicEvent, eventLeft);
                    basicEvent.Width = eventWidth;
                }
            }
            foreach (Border border in horizontalLines)
            {
                border.Width = e.NewSize.Width;
            }
            int i = 1;
            foreach (Border border in verticalLines)
            {
                Canvas.SetLeft(border, eventWidth * i);
                i++;
            }
        }
    }
}
