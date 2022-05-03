using GroupCalendar.Core;
using GroupCalendar.Data.Local;
using GroupCalendar.Data.Network;
using GroupCalendar.Data.Remote.Model;
using GroupCalendar.View;
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
    internal class EditEventViewModel : INotifyPropertyChanged
    {
        public Guid groupId = new Guid("4ab43a46-5baa-428f-92dc-f147f23dc81a");
        private Guid eventId;
        public ICommand SendCommand { get; set; }
        public ICommand BackCommand { get; set; }

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
            set { if (value) EventModel.RecurrenceId = Guid.NewGuid(); else EventModel.RecurrenceId = null; OnPropertyChanged("IsRecurrent"); }
        }

        private bool isEditing = false;
        public bool IsEditing
        {
            get { return isEditing; }
            set { isEditing = value; ButtonText = (value) ? "Editar evento" : "Crear evento"; OnPropertyChanged("IsEditing"); }
        }

        public bool ShowEditAll
        {
            get { return IsEditing && IsRecurrent; }
        }

        private string buttonText = "Crear evento";
        public string ButtonText { get => buttonText; set { buttonText = value; OnPropertyChanged("ButtonText"); } }
        public bool EditAll { get; set; } = false;


        public bool IsLoading { get; set; }
        public DateTime LastDate { get; set; } = DateTime.Now;

        private IEnumerable<DayOfWeek> days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

        public List<WeekDayCheck> WeekDaysCheck { get; set; } = new List<WeekDayCheck>();

        public EditEventViewModel()
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
                groupId = Guid.NewGuid();
            }
            SendCommand = new RelayCommand(o => UpdateEvent(), o => CheckEventOk());
            BackCommand = new RelayCommand(o => Back());
        }


        private async void UpdateEvent()
        {
            if (IsEditing)
            {
                if (IsRecurrent && EditAll)
                {
                    EditRecurrentEvent();
                }
                else
                {
                    EditEvent();
                }
            }
            else
            {
                CreateEvent();
            }
        }



        private void Back()
        {
            if (IsEditing)
            {
                StaticResources.mainWindow.Frame.Content = new EventPage();
            }
            else
            {
                DialogHost.Close(null);
            }
        }

        public async void LoadEvent()
        {
            groupId = ApplicationState.GetValue<Guid>("group_id");
            if (groupId == Guid.Empty) throw new Exception("GroupId is missing");
            var group = await Repository.GetGroupByIdAsync(groupId.ToString());
            eventId = ApplicationState.GetValue<Guid>("event_id");
            if (eventId == Guid.Empty) return;
            EventModel = group.Events.Find(e => e.Id == eventId);
            IsEditing = true;
            EditAll = true;
            OnPropertyChanged("EventModel");
            OnPropertyChanged("ShowEditAll");
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
            return !IsRecurrent || IsEditing || WeekDaysCheck.Any(day => day.IsChecked);
        }

        private bool CheckDateOk()
        {
            return !IsRecurrent || IsEditing || LastDate.Year > EventModel.Start.Year || (
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


        public async void CreateEvent()
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
                StaticResources.mainWindow.Frame.Content = new TimetablePage();
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Data.ToString());
            }
        }

        private async void EditEvent()
        {
            var group = await Repository.GetGroupByIdAsync(groupId.ToString());
            group.Events = group.Events.FindAll(e => e.Id != eventId);
            EventModel.RecurrenceId = null;
            group.Events.Add(EventModel);
            try
            {
                await Repository.UpdateGroupEventsAsync(group);
                IsLoading = false;
                StaticResources.mainWindow.Frame.Content = new TimetablePage();
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Data.ToString());
            }
        }

        private async void EditRecurrentEvent()
        {
            var group = await Repository.GetGroupByIdAsync(groupId.ToString());
            var recurrentEvents = group.Events.FindAll(e => e.RecurrenceId == EventModel.RecurrenceId);
            foreach (var recurrentEvent in recurrentEvents)
            {
                recurrentEvent.Name = EventModel.Name;
                recurrentEvent.Description = EventModel.Description;
                recurrentEvent.StartTime = EventModel.StartTime;
                recurrentEvent.EndTime = EventModel.EndTime;
                recurrentEvent.RequireConfirmation = EventModel.RequireConfirmation;
                recurrentEvent.Color = EventModel.Color;
            }
            try
            {
                await Repository.UpdateGroupEventsAsync(group);
                IsLoading = false;
                StaticResources.mainWindow.Frame.Content = new TimetablePage();
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
