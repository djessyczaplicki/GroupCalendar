using GroupCalendar.Core;
using GroupCalendar.Data.Network;
using GroupCalendar.Data.Remote.Model;
using GroupCalendar.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace GroupCalendar.ViewModel
{
    internal partial class TimetableViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<EventModel> EventModels
        { get; set; }

        public ObservableCollection<EventModel> EventsToShow
        { get; set; }

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
            LoadGroup();
        }

        private async void LoadGroup()
        {
            EventModels = new ObservableCollection<EventModel>();
            EventsToShow = new ObservableCollection<EventModel>();
            var groupId = ApplicationState.GetValue<Guid>("group_id");
            var group = await Repository.GetGroupByIdAsync(groupId.ToString());
            var weekOffset = 0;
            group.Events.ForEach(e => EventModels.Add(e));
            var eventsToShow = GetWeek(group.Events, weekOffset);
            eventsToShow.ForEach(e => EventsToShow.Add(e));
        }

        private List<EventModel> GetWeek(List<EventModel> eventModels, int weekOffset)
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
