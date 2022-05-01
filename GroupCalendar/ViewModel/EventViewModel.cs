﻿using GroupCalendar.Core;
using GroupCalendar.Data.Local;
using GroupCalendar.Data.Network;
using GroupCalendar.Data.Remote.Model;
using GroupCalendar.ViewModel.Commands;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace GroupCalendar.ViewModel
{
    internal class EventViewModel
    {
        public Guid groupId = new Guid("4ab43a46-5baa-428f-92dc-f147f23dc81a");
        public ICommand ButtonCommand { get; set; }

        private EventModel eventModel = new EventModel
        {
            Name = "",
            Start = DateTimeOffset.Now,
            End = DateTimeOffset.Now.AddHours(1),
            Color = new CustomColorModel(255, 100, 100)
        };

        bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public bool IsRecurrent
        {
            get { return EventModel.RecurrenceId.HasValue; }
            set { if (value) EventModel.RecurrenceId = Guid.NewGuid(); else EventModel.RecurrenceId = null; }
        }

        public bool IsLoading { get; set; }
        public DateTime LastDate { get; set; } = DateTime.Now;

        private IEnumerable<DayOfWeek> days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
        public List<WeekDayCheck> WeekDaysCheck { get; set; } = new List<WeekDayCheck>();

        public EventViewModel()
        {
            LoadEvent();
            foreach (var day in days)
            {
                CultureInfo ci = new CultureInfo("Es-Es");
                WeekDaysCheck.Add(new WeekDayCheck()
                {
                    DayOfWeek = day,
                    Day = Capitalize(ci.DateTimeFormat.GetDayName(day)),
                    IsChecked = false
                });
            }
            WeekDaysCheck.Add(WeekDaysCheck[0]);
            WeekDaysCheck.RemoveAt(0);
            if (IsInDesignMode)
            {
                EventModel = new EventModel
                {
                    Name = "Evento de prueba",
                    Start = DateTimeOffset.Now,
                    End = DateTimeOffset.Now.AddHours(1),
                    Color = new CustomColorModel(255, 100, 100),
                    Description = "Descripción de prueba",
                    RecurrenceId = Guid.NewGuid(),
                };
            }
            ButtonCommand = new RelayCommand(o => UpdateEvent(), o => CheckEventOk());
        }

        public async void LoadEvent()
        {
            groupId = ApplicationState.GetValue<Guid>("group_id");
            if (groupId == Guid.Empty) throw new Exception("Y el group id donde está ?");
            var group = await Repository.GetGroupByIdAsync(groupId.ToString());
            var eventId = ApplicationState.GetValue<Guid>("event_id");
            if (eventId == Guid.Empty) return;
            EventModel = group.Events.Find(e => e.Id == eventId);
        }

        private string Capitalize(string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        private bool CheckEventOk()
        {
            return EventModel.Name != "" && EventModel.Start.Date.Year != 1 && CheckTimeOk() && CheckDateOk() && CheckWeekDayOk();
        }

        private bool CheckWeekDayOk()
        {
            return !IsRecurrent || WeekDaysCheck.Any(day => day.IsChecked);
        }

        private bool CheckDateOk()
        {
            return !IsRecurrent || LastDate.Year > EventModel.Start.Year || (
                LastDate.Year == EventModel.Start.Year && LastDate.Month > EventModel.Start.Month || (
                    LastDate.Month == EventModel.Start.Month && LastDate.Day >= EventModel.Start.Day
                )
            );
        }

        private bool CheckTimeOk()
        {
            var endHour = EventModel.End.TimeOfDay.Hours;
            var startHour = EventModel.Start.TimeOfDay.Hours;
            var startMinute = EventModel.Start.TimeOfDay.Minutes;
            var endMinute = EventModel.End.TimeOfDay.Minutes;

            return (endHour > startHour) || endHour == startHour && endMinute > startMinute;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EventModel EventModel
        {
            get { return eventModel; }
            set
            {
                eventModel = value;
                OnPropertyChanged("Event");
            }
        }

        public async void UpdateEvent()
        {
            IsLoading = true;
            var group = await Repository.GetGroupByIdAsync(groupId.ToString());

            if (IsRecurrent)
            {
                var day = EventModel.Start;
                var lastDay = LastDate.AddDays(1); // to adjust the time difference
                while (day <= lastDay)
                {
                    if (WeekDaysCheck.Any(weekDay => weekDay.DayOfWeek == day.DayOfWeek && weekDay.IsChecked))
                    {
                        var dayEventModel = new EventModel
                        {
                            Name = EventModel.Name,
                            Start = new DateTimeOffset(
                                day.Year,
                                day.Month,
                                day.Day,
                                EventModel.Start.Hour,
                                EventModel.Start.Minute,
                                0,
                                TimeZoneInfo.Utc.BaseUtcOffset
                            ),
                            End = new DateTimeOffset(
                                day.Year,
                                day.Month,
                                day.Day,
                                EventModel.End.Hour,
                                EventModel.End.Minute,
                                0,
                                TimeZoneInfo.Utc.BaseUtcOffset
                            ),
                            Description = EventModel.Description,
                            Color = EventModel.Color,
                            ConfirmedUsers = EventModel.ConfirmedUsers,
                            Id = Guid.NewGuid(),
                            RequireConfirmation = EventModel.RequireConfirmation,
                            RecurrenceId = EventModel.RecurrenceId
                        };
                        group.Events.Add(dayEventModel);
                    }
                    day = day.AddDays(1);
                }
            }
            else
            {
                group.Events.Add(eventModel);
            }

            try
            {
                await Repository.UpdateGroupEventsAsync(group);
                IsLoading = false;
                DialogHost.Close(null);
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Data.ToString());
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
