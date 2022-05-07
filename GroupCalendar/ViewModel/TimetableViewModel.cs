using Firebase.Auth.UI;
using GroupCalendar.Core;
using GroupCalendar.Data.Network;
using GroupCalendar.Data.Remote.Model;
using GroupCalendar.Utils;
using GroupCalendar.View;
using GroupCalendar.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GroupCalendar.ViewModel
{
    internal partial class TimetableViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand NextWeekCommand { get; set; }
        public ICommand PrevWeekCommand { get; set; }
        public ICommand ShowAllEventsCommand { get; set; }
        public ICommand CreateGroupCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }

        public ObservableCollection<EventModel> EventModels
        { get; set; } = new ObservableCollection<EventModel>();

        public ObservableCollection<EventModel> EventsToShow
        { get; set; } = new ObservableCollection<EventModel>();

        int weekOffset = 0;

        public List<GroupModel> Groups { get; set; } = new List<GroupModel>();

        private GroupModel selectedGroup;

        public GroupModel SelectedGroup
        {
            get { return selectedGroup; }
            set { selectedGroup = value; ChangeGroup(value); }
        }

        private void ChangeGroup(GroupModel value)
        {
            ApplicationState.SetValue("group_id", value.Id);
            StaticResources.mainWindow.Frame.Content = new TimetablePage();
        }

        public DateTimeOffset FirstDay { get; set; } = DateTimeOffset.Now;
        public GroupModel Group { get; private set; }

        bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

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
            LoadGroups();
            NextWeekCommand = new RelayCommand(o => ShowNextWeek());
            PrevWeekCommand = new RelayCommand(o => ShowPreviousWeek());
            ShowAllEventsCommand = new RelayCommand(o => ShowAllEvents());
            CreateGroupCommand = new RelayCommand(o => CreateGroup());
            DisconnectCommand = new RelayCommand(o => Disconnect());
        }

        private async void Disconnect()
        {

            await FirebaseUI.Instance.Client.SignOutAsync();
            StaticResources.mainWindow.Frame.Content = new LoginPage();
        }

        private void CreateGroup()
        {

        }

        private void ShowAllEvents()
        {
            throw new NotImplementedException();
        }

        private async void LoadGroup()
        {
            var groupId = ApplicationState.GetValue<Guid>("group_id");
            Group = await Repository.GetGroupByIdAsync(groupId.ToString());
            Group.Events.ForEach(e => EventModels.Add(e));
            OnPropertyChanged("Group");
            LoadWeek();
        }

        private async void LoadGroups()
        {
            var uid = ApplicationState.GetValue<string>("uid");
            var user = await Repository.GetUserByIdAsync(uid);
            if (user == null) throw new Exception("User does not exist in the DB");
            var groupTasks = user.Groups.ConvertAll(groupId => Repository.GetGroupByIdAsync(groupId.ToString()));
            Groups = new List<GroupModel>((await Task.WhenAll(groupTasks)).OrderBy(group => group.Name));
            OnPropertyChanged("Groups");
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
