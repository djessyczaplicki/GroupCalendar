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
        public ICommand LeaveGroupCommand { get; set; }
        public ICommand JoinGroupCommand { get; set; }
        public string GroupInviteLink { get; set; }

        public bool IsAdmin
        { get; set; }

        public ObservableCollection<EventModel> EventModels
        { get; set; } = new ObservableCollection<EventModel>();

        public ObservableCollection<EventModel> EventsToShow
        { get; set; } = new ObservableCollection<EventModel>();

        int weekOffset = 0;
        private bool showAll;

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
            ApplicationState.PurgeValue("show_all");
            StaticResources.mainWindow.Frame.Content = new TimetablePage();
        }

        public DateTimeOffset FirstDay { get; set; } = DateTimeOffset.Now;
        public GroupModel Group { get; private set; }
        public UserModel User { get; private set; }

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
            showAll = ApplicationState.GetValue<bool>("show_all");
            LoadGroups();
            if (!showAll)
            {
                LoadAndShowGroup();
            }
            NextWeekCommand = new RelayCommand(o => ShowNextWeek());
            PrevWeekCommand = new RelayCommand(o => ShowPreviousWeek());
            ShowAllEventsCommand = new RelayCommand(o => ShowAllEvents());
            CreateGroupCommand = new RelayCommand(o => CreateGroup());
            DisconnectCommand = new RelayCommand(o => Disconnect());
            LeaveGroupCommand = new RelayCommand(o => LeaveGroup(o));
            JoinGroupCommand = new RelayCommand(o => JoinGroup());
        }

        private async void JoinGroup()
        {
            var groupId = GetGroupId();
            if (groupId == Guid.Empty)
            {
                MessageBox.Show("La invitación de grupo tiene un formato inválido");
                return;
            }
            GroupModel newGroup;
            try
            {
                newGroup = await Repository.GetGroupByIdAsync(groupId.ToString());
            }
            catch
            {
                MessageBox.Show("No se ha encontrado el grupo");
                return;
            }

            if (User.Groups.Contains(groupId))
            {
                MessageBox.Show("¡Ya eres miembro de este grupo!");
                return;
            }
            User.Groups.Add(groupId);
            User = await Repository.UpdateUserAsync(User);


            if (newGroup.Users.Contains(User.Id))
            {
                MessageBox.Show("¡Ya eres miembro de este grupo!");
                return;
            }

            newGroup.Users.Add(User.Id);
            await Repository.UpdateGroupAsync(newGroup);



            ApplicationState.SetValue("group_id", groupId);
            StaticResources.mainWindow.Frame.Content = new TimetablePage();
        }

        private Guid GetGroupId()
        {
            var parts = GroupInviteLink.Split('/');
            var reversedParts = parts.Reverse().ToList();
            try
            {
                return new Guid(reversedParts[0]);
            }
            catch
            {
                return Guid.Empty;
            }

        }

        private async void LeaveGroup(object o)
        {
            var groupId = (Guid)o;
            var group = Groups.Find(group => group.Id == groupId);
            var messageBoxResult = MessageBox.Show("¿Estás seguro de querer salir del grupo '" + group.Name + "'?", "Salir del grupo", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                var userId = ApplicationState.GetValue<string>("uid");
                group.Admins.Remove(userId);
                group.Users.Remove(userId);
                await Repository.UpdateGroupAsync(group);

                if (User != null)
                {
                    User.Groups.Remove(groupId);
                    User = await Repository.UpdateUserAsync(User);
                    ApplicationState.SetValue("group_id", User.Groups[0]);
                    StaticResources.mainWindow.Frame.Content = new TimetablePage();
                }
            }
        }

        private async void Disconnect()
        {
            await FirebaseUI.Instance.Client.SignOutAsync();
            StaticResources.mainWindow.Frame.Content = new LoginPage();
        }

        private void CreateGroup()
        {
            ApplicationState.SetValue("is_editing", false);
            StaticResources.mainWindow.Frame.Content = new EditGroupPage();
        }

        private void ShowAllEvents()
        {
            ApplicationState.SetValue("show_all", true);
            StaticResources.mainWindow.Frame.Content = new TimetablePage();
        }

        private async void LoadAndShowGroup()
        {
            var groupId = ApplicationState.GetValue<Guid>("group_id");
            Group = await Repository.GetGroupByIdAsync(groupId.ToString());
            Group.Events.ForEach(e => EventModels.Add(e));
            var uid = ApplicationState.GetValue<string>("uid");
            IsAdmin = Group.Admins.Contains(uid);
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(Group));
            LoadWeek();
        }
        private void ShowAllGroups()
        {
            Group = new GroupModel()
            {
                Events = Groups.Aggregate(new List<EventModel>(), (acc, curr) =>
                {
                    acc.AddRange(curr.Events);
                    return acc;
                }),
                Name = Groups.Aggregate("", (acc, curr) => acc + curr.Name + ", ")
            };
            Group.Events.ForEach(e => EventModels.Add(e));
            OnPropertyChanged("Group");
            LoadWeek();
        }


        private async void LoadGroups()
        {
            var uid = ApplicationState.GetValue<string>("uid");
            User = await Repository.GetUserByIdAsync(uid);
            if (User == null) throw new Exception("User does not exist in the DB");
            var groupTasks = User.Groups.ConvertAll(groupId => Repository.GetGroupByIdAsync(groupId.ToString()));
            Groups = new List<GroupModel>((await Task.WhenAll(groupTasks)).OrderBy(group => group.Name));
            Groups.ForEach(group =>
            {
                if (group.Admins.Contains(uid))
                {
                    group.CurrentUserIsAdmin = true;
                }
            });
            OnPropertyChanged("Groups");
            if (showAll)
            {
                ShowAllGroups();
            }
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

        private void LoadWeek()
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
