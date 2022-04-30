﻿using GroupCalendar.Data.Remote.Model;
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
        readonly int hourHeight = 60;
        double eventWidth;
        List<Border> horizontalLines = new List<Border>();
        List<Border> verticalLines = new List<Border>();
        public ObservableCollection<EventModel> EventModels
        {
            get { return (ObservableCollection<EventModel>)GetValue(EventModelsProperty); }
            set { SetValue(EventModelsProperty, value); }
        }
        public static readonly DependencyProperty EventModelsProperty =
            DependencyProperty.Register("EventModels", typeof(ObservableCollection<EventModel>), typeof(Schedule), new PropertyMetadata(new ObservableCollection<EventModel>()));



        public Schedule()
        {
            InitializeComponent();
            //EventModels.Add(new EventModel()
            //{
            //    Start = DateTimeOffset.Now,
            //    End = DateTimeOffset.Now.AddHours(1),
            //    Name = "Eventito"
            //});
            //EventModels.Add(new EventModel()
            //{

            //    Start = DateTimeOffset.Now.AddHours(2),
            //    End = DateTimeOffset.Now.AddHours(4),
            //    Name = "Otro evento"

            //});
            //EventModels.Add(new EventModel()
            //{

            //    Start = DateTimeOffset.Now.AddHours(-6),
            //    End = DateTimeOffset.Now.AddHours(-4),
            //    Name = "Último primer evento"

            //});
            //EventModels.Add(new EventModel()
            //{

            //    Start = DateTimeOffset.Now.AddDays(1),
            //    End = DateTimeOffset.Now.AddDays(1).AddHours(3),
            //    Color = new CustomColorModel(255, 100, 150),
            //    Name = "2o Día evento"

            //});
            //EventModels.Add(new EventModel()
            //{

            //    Start = DateTimeOffset.Now.AddDays(3),
            //    End = DateTimeOffset.Now.AddDays(3).AddHours(3),
            //    Color = new CustomColorModel(200, 100, 150),
            //    Name = "otro Día evento"

            //});
        }

        private void ScheduleLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
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
            var minEventHeight = hourHeight / 2;
            var eventTop = eventModel.Start.Hour * hourHeight + eventModel.Start.Minute * minuteHeight;
            var eventLeft = GetEventLeft(eventModel, eventWidth);
            var eventHeight = Math.Max((eventModel.End.Hour - eventModel.Start.Hour) * hourHeight + (eventModel.End.Minute - eventModel.Start.Minute) * minuteHeight, minEventHeight);
            Canvas.SetLeft(basicEvent, eventLeft);
            Canvas.SetTop(basicEvent, eventTop);
            basicEvent.Width = eventWidth;
            basicEvent.Height = eventHeight;
            return basicEvent;
        }

        private double GetEventLeft(EventModel eventModel, double eventWidth)
        {
            return (eventModel.Start - FirstDay).Days * eventWidth;
        }

        private void ScheduleSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            eventWidth = e.NewSize.Width / DaysToShow;
            foreach (object o in Canvas.Children)
            {
                var basicEvent = o as BasicEvent;
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