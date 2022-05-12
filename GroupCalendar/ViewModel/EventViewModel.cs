using GroupCalendar.Core;
using GroupCalendar.Data.Network;
using GroupCalendar.Data.Remote.Model;
using GroupCalendar.View;
using GroupCalendar.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GroupCalendar.ViewModel
{
    public class EventViewModel : INotifyPropertyChanged
    {
        private Guid groupId;

        public GroupModel Group { get; private set; }

        private Guid eventId;

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand DeleteEventCommand { get; set; }
        public EventModel EventModel { get; private set; }
        public List<UserModel> Users { get; private set; }

        public ICommand ToggleAttendanceCommand { get; set; }

        public bool IsAttending { get; set; }

        public bool IsDeleting { get; private set; }
        public bool IsToggling { get; private set; }

        public bool IsAdmin { get; private set; }

        public EventViewModel()
        {
            LoadEvent();
            DeleteEventCommand = new RelayCommand(o => DeleteEvent(), o => !IsDeleting);
            ToggleAttendanceCommand = new RelayCommand(o => ToggleAttendance(), o => !IsToggling);
        }

        private async void ToggleAttendance()
        {
            IsToggling = true;
            await LoadEvent();
            var userId = ApplicationState.GetValue<string>("uid");
            if (IsAttending)
            {
                EventModel.ConfirmedUsers.Remove(userId);
            }
            else
            {
                EventModel.ConfirmedUsers.Add(userId);
            }
            await Repository.UpdateGroupEventsAsync(Group);
            await LoadEvent();
            IsToggling = false;
            CheckAttendance();
            OnPropertyChanged("IsToggling");
        }

        public async Task LoadEvent()
        {
            groupId = ApplicationState.GetValue<Guid>("group_id");
            if (groupId == Guid.Empty) throw new Exception("GroupId is missing");
            Group = await Repository.GetGroupByIdAsync(groupId.ToString());
            UpdateUserPermissions();
            eventId = ApplicationState.GetValue<Guid>("event_id");
            if (eventId == Guid.Empty) throw new Exception("EventId is missing");
            EventModel = Group.Events.Find(e => e.Id == eventId);
            OnPropertyChanged("EventModel");
            var userTasks = EventModel.ConfirmedUsers.ConvertAll(userId => Repository.GetUserByIdAsync(userId));
            Users = new List<UserModel>(await Task.WhenAll(userTasks));
            OnPropertyChanged("Users");
            CheckAttendance();
        }

        private void UpdateUserPermissions()
        {
            var uid = ApplicationState.GetValue<string>("uid");
            IsAdmin = Group.Admins.Contains(uid);
            OnPropertyChanged(nameof(IsAdmin));
        }

        private void CheckAttendance()
        {
            IsAttending = Users.Find(u => u.Id == ApplicationState.GetValue<string>("uid")) != null;
            OnPropertyChanged("IsAttending");
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void DeleteEvent()
        {
            IsDeleting = true;
            var group = await Repository.GetGroupByIdAsync(groupId.ToString());
            if (EventModel.RecurrenceId == null || EventModel.RecurrenceId == Guid.Empty)
            {
                group.Events = group.Events.FindAll(e => e.Id != eventId);
            }
            else
            {
                string sMessageBoxText = "¿Quieres borrar todas las repeticiones?\nO pulsa NO para borrar solo este evento.";
                string sCaption = "Borrar evento recurrente";

                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Question;

                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        group.Events = group.Events.FindAll(e => e.RecurrenceId != EventModel.RecurrenceId);
                        break;

                    case MessageBoxResult.No:
                        group.Events = group.Events.FindAll(e => e.Id != eventId);
                        break;

                    case MessageBoxResult.Cancel:
                        IsDeleting = false;
                        return;
                }
            }
            await Repository.UpdateGroupEventsAsync(group);
            IsDeleting = false;
            ApplicationState.PurgeValue("event_id");
            StaticResources.mainWindow.Frame.Content = new TimetablePage();
        }

    }
}
