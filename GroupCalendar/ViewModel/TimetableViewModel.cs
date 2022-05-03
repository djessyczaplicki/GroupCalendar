using GroupCalendar.Core;
using GroupCalendar.Data.Network;
using GroupCalendar.Data.Remote.Model;
using GroupCalendar.Utils;
using GroupCalendar.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace GroupCalendar.ViewModel
{
    internal partial class TimetableViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand NextWeekCommand { get; set; }
        public ICommand PrevWeekCommand { get; set; }
        public ICommand CreateEventCommand { get; set; }

        int weekOffset = 0;
        public ObservableCollection<EventModel> EventModels
        { get; set; } = new ObservableCollection<EventModel>();

        public ObservableCollection<EventModel> EventsToShow
        { get; set; } = new ObservableCollection<EventModel>();

        public void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        public List<GroupModel> Groups { get; set; } = new List<GroupModel>();
        public DateTimeOffset FirstDay { get; set; } = DateTimeOffset.Now;

        bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public TimetableViewModel()
        {
            if (IsInDesignMode)
            {
                Groups.Add(new GroupModel() { Name = "Grupo 1" });
                Groups.Add(new GroupModel() { Name = "Grupo 2" });
                Groups.Add(new GroupModel() { Name = "Grupo 3" });
                return;
            }
            weekOffset = ApplicationState.GetValue<int>("weekOffset");
            LoadGroup();
            NextWeekCommand = new RelayCommand(o => ShowNextWeek());
            PrevWeekCommand = new RelayCommand(o => ShowPreviousWeek());
            CreateEventCommand = new RelayCommand(o => CreateNewEvent());
        }

        private void CreateNewEvent()
        {

        }

        private async void LoadGroup()
        {
            var groupId = ApplicationState.GetValue<Guid>("group_id");
            var group = await Repository.GetGroupByIdAsync(groupId.ToString());
            group.Events.ForEach(e => EventModels.Add(e));
            LoadWeek();
        }

        private void ShowNextWeek()
        {
            weekOffset++;
            ApplicationState.SetValue("weekOffset", weekOffset);
            LoadWeek();
        }

        private void ShowPreviousWeek()
        {
            weekOffset--;
            ApplicationState.SetValue("weekOffset", weekOffset);
            LoadWeek();
        }

        private async void LoadWeek()
        {
            EventsToShow.Clear();
            var eventsToShow = GetWeek(EventModels.ToList());
            eventsToShow.ForEach(e => EventsToShow.Add(e));
            OnPropertyChanged("EventsToShow");
        }

        private List<EventModel> GetWeek(List<EventModel> eventModels)
        {
            var week = new List<EventModel>();
            var day = DateTimeOffset.Now.AddDays(weekOffset * 7);
            var startOfWeek = day.StartOfWeek(DayOfWeek.Monday);
            FirstDay = startOfWeek;
            OnPropertyChanged("FirstDay");
            var endOfWeek = day.AddDays(7).StartOfWeek(DayOfWeek.Monday);
            week = eventModels.FindAll(eventModel => eventModel.Start > startOfWeek && eventModel.Start < endOfWeek);
            return week;
        }

    }
}
